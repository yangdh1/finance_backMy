using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Finance.Audit;
using Finance.Dto;
using Finance.Entering.Model;
using Finance.Infrastructure;
using Finance.PriceEval;
using Finance.ProductDevelopment;
using Finance.PropertyDepartment.Entering;
using Finance.PropertyDepartment.Entering.Dto;
using Finance.PropertyDepartment.Entering.Method;
using Finance.PropertyDepartment.Entering.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Finance.Authorization.Roles.StaticRoleNames;

namespace Finance.Entering
{
    /// <summary>
    /// 资源部录入API
    /// </summary>
    [AbpAuthorize]
    public class ResourceEnteringAppService : FinanceAppServiceBase, IResourceEnteringAppService
    {
        private readonly IRepository<ModelCount, long> _resourceModelCount;
        private readonly IRepository<ModelCountYear, long> _resourceModelCountYear;
        private readonly IRepository<FinanceDictionaryDetail, string> _resourceFinanceDictionaryDetail;
        /// <summary>
        /// 产品开发部电子BOM输入信息
        /// </summary>
        private readonly IRepository<ElectronicBomInfo, long> _resourceElectronicBomInfo;
        private readonly ElectronicStructuralMethod _resourceElectronicStructuralMethod;
        /// <summary>
        /// 资源部电子物料录入
        /// </summary>
        private static IRepository<EnteringElectronic, long> _configEnteringElectronic;
        /// <summary>
        /// 资源部结构物料录入
        /// </summary>
        private static IRepository<StructureElectronic, long> _configStructureElectronic;
        /// <summary>
        /// 电子BOM两次上传差异化表
        /// </summary>
        private static IRepository<ElecBomDifferent, long> _configElecBomDifferent;
        /// <summary>
        /// 结构BOM两次上传差异化表
        /// </summary>
        private static IRepository<StructBomDifferent, long> _configStructBomDifferent;
        /// <summary>
        /// 流程流转服务
        /// </summary>
        private readonly AuditFlowAppService _flowAppService;      
        /// <summary>
        /// 构造函数
        /// </summary>
        public ResourceEnteringAppService(IRepository<ModelCount, long> modelCount,
            IRepository<ModelCountYear, long> modelCountYear,
            IRepository<FinanceDictionaryDetail, string> financeDictionaryDetail,
            ElectronicStructuralMethod electronicStructuralMethod,
            IRepository<ElectronicBomInfo, long> electronicBomInfo,
            AuditFlowAppService flowAppService,
            IRepository<EnteringElectronic, long> enteringElectronic,
            IRepository<StructureElectronic, long> structureElectronic,
            IRepository<ElecBomDifferent, long> elecBomDifferent,
            IRepository<StructBomDifferent, long> structBomDifferent)
        {
            _resourceModelCount = modelCount;
            _resourceModelCountYear = modelCountYear;
            _resourceFinanceDictionaryDetail = financeDictionaryDetail;
            _resourceElectronicStructuralMethod = electronicStructuralMethod;
            _resourceElectronicBomInfo = electronicBomInfo;
            _flowAppService = flowAppService;
            _configEnteringElectronic= enteringElectronic;
            _configStructureElectronic= structureElectronic;
            _configElecBomDifferent= elecBomDifferent;
            _configStructBomDifferent= structBomDifferent;           
        }

        /// <summary>
        /// 资源部输入时,加载电子料初始值
        /// </summary>
        /// <param name="Id">流程表主键</param>
        /// <returns></returns> 
        public async Task<InitialElectronicDto> GetElectronic(long Id)
        {
            //根据报价核价需求录入表主键取模组数量
            List<ModelCount> modelCount = await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            //总共的零件
            List<PartModel> partList = _resourceElectronicStructuralMethod.PartDtoList(modelCount);
            InitialElectronicDto initialElectronicDto = new();
            //从字典明细表取 零件名称和id
            initialElectronicDto.PartDtoList=partList;
            initialElectronicDto.ElectronicBomList=await _resourceElectronicStructuralMethod.ElectronicBomList(partList, Id);// 电子BOM单价表单           
            #region 电子BOM退回差异处理
            foreach (var item in partList)//循环所有零件
            {
                List<ElecBomDifferent> elecBomDifferents = await _configElecBomDifferent.GetAllListAsync(p => p.AuditFlowId.Equals(Id)&&p.ProductId.Equals(item.ProductId));
                if (elecBomDifferents.Count is not 0)//有差异
                {
                    foreach (ElecBomDifferent elecBom in elecBomDifferents)
                    {
                        //判断差异类型
                        if (elecBom.ModifyTypeValue.Equals(MODIFYTYPE.DELNEWDATA))//删除
                        {
                            //删除存在数据库里的数据和返回数据中的数据即可
                            //1.删除数据库中的数据
                            await _configEnteringElectronic.HardDeleteAsync(p => p.AuditFlowId.Equals(Id)&&p.ProductId.Equals(item.ProductId)&&p.ElectronicId.Equals(elecBom.ElectronicId));
                            //2.删除返回数据库中的数据
                            ElectronicDto electronicDto = initialElectronicDto.ElectronicBomList.Where(p => p.ProductId.Equals(item.ProductId)&&p.ElectronicId.Equals(elecBom.ElectronicId)).FirstOrDefault();
                            initialElectronicDto.ElectronicBomList.Remove(electronicDto);
                        }
                        else if (elecBom.ModifyTypeValue.Equals(MODIFYTYPE.MODIFYNEWDATA))//修改
                        {
                            //重新加载项目使用量和系统单价
                            ElectronicDto electronicDto = initialElectronicDto.ElectronicBomList.Where(p => p.ElectronicId.Equals(elecBom.ElectronicId)).FirstOrDefault();
                            //索引
                            if (electronicDto is not null)
                            {
                                int index = initialElectronicDto.ElectronicBomList.FindIndex(p => p.ElectronicId.Equals(elecBom.ElectronicId));
                                electronicDto = await _resourceElectronicStructuralMethod.ElectronicBom(item.ProductId, Id, elecBom.ElectronicId);
                                initialElectronicDto.ElectronicBomList[index] = electronicDto;
                            }                               
                        }
                    }
                }
            }
            #endregion
            return initialElectronicDto;
        }
        /// <summary>
        /// 资源部输入时,加载电子料初始值(单个零件)
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<List<ElectronicDto>> GetElectronicSingle(long auditFlowId, long productId)
        {
            List<ElectronicDto> electronicDtos = new();
            //根据报价核价需求录入表主键取模组数量
            List<ModelCount> modelCount = await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId)&&p.Id.Equals(productId));
            //总共的零件
            List<PartModel> partList = _resourceElectronicStructuralMethod.PartDtoList(modelCount);
            electronicDtos=await _resourceElectronicStructuralMethod.ElectronicBomList(partList, auditFlowId);// 电子BOM单价表单;
            #region 电子BOM退回差异处理
            foreach (var item in partList)//循环所有零件
            {
                List<ElecBomDifferent> elecBomDifferents = await _configElecBomDifferent.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId)&&p.ProductId.Equals(item.ProductId));
                if (elecBomDifferents.Count is not 0)//有差异
                {
                    foreach (ElecBomDifferent elecBom in elecBomDifferents)
                    {
                        //判断差异类型
                        if (elecBom.ModifyTypeValue.Equals(MODIFYTYPE.DELNEWDATA))//删除
                        {
                            //删除存在数据库里的数据和返回数据中的数据即可
                            //1.删除数据库中的数据
                            await _configEnteringElectronic.HardDeleteAsync(p => p.AuditFlowId.Equals(auditFlowId)&&p.ProductId.Equals(item.ProductId)&&p.ElectronicId.Equals(elecBom.ElectronicId));
                            //2.删除返回数据库中的数据
                            ElectronicDto electronicDto = electronicDtos.Where(p => p.ProductId.Equals(item.ProductId)&&p.ElectronicId.Equals(elecBom.ElectronicId)).FirstOrDefault();
                            electronicDtos.Remove(electronicDto);
                        }
                        else if (elecBom.ModifyTypeValue.Equals(MODIFYTYPE.MODIFYNEWDATA))//修改
                        {
                            //重新加载项目使用量和系统单价
                            ElectronicDto electronicDto = electronicDtos.Where(p => p.ElectronicId.Equals(elecBom.ElectronicId)).FirstOrDefault();                            //索引
                            if(electronicDto is not null)
                            {
                                int index = electronicDtos.FindIndex(p => p.ElectronicId.Equals(elecBom.ElectronicId));
                                electronicDto = await _resourceElectronicStructuralMethod.ElectronicBom(item.ProductId, auditFlowId, elecBom.ElectronicId);
                                electronicDtos[index] = electronicDto;
                            }                         
                        }
                    }
                }
            }
            #endregion
            return electronicDtos;
        }
        /// <summary>
        /// BOM单价审核 加载电子料初始值
        /// </summary>
        /// <param name="Id">流程表主键</param>
        /// <returns></returns> 
        public async Task<InitialElectronicDto> GetBOMElectronic(long Id)
        {
            //根据报价核价需求录入表主键取模组数量
            List<ModelCount> modelCount = await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            //总共的零件
            List<PartModel> partList = _resourceElectronicStructuralMethod.PartDtoList(modelCount);
            InitialElectronicDto initialElectronicDto = new();
            //从字典明细表取 零件名称和id
            initialElectronicDto.PartDtoList=partList;
            initialElectronicDto.ElectronicBomList=await _resourceElectronicStructuralMethod.BOMElectronicBomList(partList, Id);// 电子BOM单价表单           

            return initialElectronicDto;
        }
        /// <summary>
        ///  BOM单价审核 加载电子料初始值(单个零件)
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<IsALLElectronicDto> GetBOMElectronicSingle(long auditFlowId, long productId)
        {
            IsALLElectronicDto isALLElectronicDto = new();
            //根据报价核价需求录入表主键取模组数量
            List<ModelCount> modelCount = await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId)&&p.Id.Equals(productId));
            //总共的零件
            List<PartModel> partList = _resourceElectronicStructuralMethod.PartDtoList(modelCount);
            isALLElectronicDto.ElectronicDtos= await _resourceElectronicStructuralMethod.BOMElectronicBomList(partList, auditFlowId);// 电子BOM单价表单     ;            
            isALLElectronicDto.isAll = await this.GetElectronicIsAllEntering(auditFlowId);
            return isALLElectronicDto;
        }
        /// <summary>
        /// 电子料单价录入  判断是否全部提交完毕  true 所有零件已录完   false  没有录完(仅仅审核页面查询方法)
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetElectronicIsAllEntering(long Id)
        {
            InitialElectronicDto initialElectronicDto = await GetElectronic(Id);
            int AllCount = initialElectronicDto.ElectronicBomList.Count;//应该录入数据库这么多条数据
            //根据报价核价需求录入表主键取模组数量
            List<ModelCount> modelCount = await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            int Count = 0;
            foreach (var model in modelCount)
            {
                Count += await _configEnteringElectronic.CountAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(model.Id) && p.IsSubmit);//数据库实际提交的条数
            }
            return AllCount == Count;
        }
        /// <summary>
        /// 资源部输入时,加载结构料初始值
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<InitialStructuralDto> GetStructural(long Id)
        {
            //根据报价核价需求录入表 流程主键取模组数量 
            List<ModelCount> modelCount = await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            //总共的零件
            List<PartModel> partList = _resourceElectronicStructuralMethod.PartDtoList(modelCount);
            //删除的结构BOMid
            List<long> longs = new();
            //修改的结构BOMid
            List<long> longs1 = new();
            #region 结构BOM退回差异处理
            foreach (var item in partList)//循环所有零件
            {
                List<StructBomDifferent> structBomDifferents = await _configStructBomDifferent.GetAllListAsync(p => p.AuditFlowId.Equals(Id)&&p.ProductId.Equals(item.ProductId));
                if (structBomDifferents.Count  is not 0)//有差异
                {
                    foreach (StructBomDifferent structBom in structBomDifferents)
                    {
                        //判断差异类型
                        if (structBom.ModifyTypeValue.Equals(MODIFYTYPE.DELNEWDATA))//删除
                        {
                            //删除存在数据库里的数据和返回数据中的数据即可
                            //1.删除数据库中的数据
                            await _configStructureElectronic.HardDeleteAsync(p => p.AuditFlowId.Equals(Id)&&p.ProductId.Equals(item.ProductId)&&p.StructureId.Equals(structBom.StructureId));
                            longs.Add(structBom.StructureId);
                        }
                        else if (structBom.ModifyTypeValue.Equals(MODIFYTYPE.MODIFYNEWDATA))//修改
                        {
                            longs1.Add(structBom.StructureId);
                        }
                    }
                }
            }
            #endregion
            InitialStructuralDto initialStructuralDto = new();
            initialStructuralDto.PartDtoList = partList;
            initialStructuralDto.ConstructionBomList=await _resourceElectronicStructuralMethod.ConstructionBomList(partList, Id, longs, longs1);// 结构料BOM单价表单
            return initialStructuralDto;
        }
        /// <summary>
        ///  资源部输入时,加载结构料初始值(单个零件)
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<List<ConstructionDto>> GetStructuralSingle(long auditFlowId, long productId)
        {
            //根据报价核价需求录入表 流程主键取模组数量 
            List<ModelCount> modelCount = await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId)&&p.Id.Equals(productId));
            //删除的结构BOMid
            List<long> longs = new();
            //修改的结构BOMid
            List<long> longs1 = new();
            //总共的零件
            List<PartModel> partList = _resourceElectronicStructuralMethod.PartDtoList(modelCount);
            #region 结构BOM退回差异处理
            foreach (var item in partList)//循环所有零件
            {
                List<StructBomDifferent> structBomDifferents = await _configStructBomDifferent.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId)&&p.ProductId.Equals(item.ProductId));
                if (structBomDifferents.Count is not 0)//有差异
                {
                    foreach (StructBomDifferent structBom in structBomDifferents)
                    {
                        //判断差异类型
                        if (structBom.ModifyTypeValue.Equals(MODIFYTYPE.DELNEWDATA))//删除
                        {
                            //删除存在数据库里的数据和返回数据中的数据即可
                            //1.删除数据库中的数据
                            await _configStructureElectronic.HardDeleteAsync(p => p.AuditFlowId.Equals(auditFlowId)&&p.ProductId.Equals(item.ProductId)&&p.StructureId.Equals(structBom.StructureId));
                            longs.Add(structBom.StructureId);
                        }
                        else if (structBom.ModifyTypeValue.Equals(MODIFYTYPE.MODIFYNEWDATA))//修改
                        {
                            longs1.Add(structBom.StructureId);
                        }
                    }
                }
            }
            #endregion
            List<ConstructionDto> prop = await _resourceElectronicStructuralMethod.ConstructionBomList(partList, auditFlowId, longs, longs1);// 结构料BOM单价表单           
                   
            return prop;
        }
        /// <summary>
        ///  BOM单价审核  加载结构料初始值
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<InitialStructuralDto> GetBOMStructural(long Id)
        {
            //根据报价核价需求录入表主键取模组数量 ===>这里暂时用的是核价表主键,以后要改
            List<ModelCount> modelCount = await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            //总共的零件
            List<PartModel> partList = _resourceElectronicStructuralMethod.PartDtoList(modelCount);
            InitialStructuralDto initialStructuralDto = new();
            initialStructuralDto.PartDtoList = partList;
            initialStructuralDto.ConstructionBomList=await _resourceElectronicStructuralMethod.BOMConstructionBomList(partList, Id);// 结构料BOM单价表单
            return initialStructuralDto;
        }
        /// <summary>
        ///  BOM单价审核  加载结构料初始值(单个零件)
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<IsALLConstructionDto> GetBOMStructuralSingle(long auditFlowId, long productId)
        {
            IsALLConstructionDto isALLConstructionDto = new();
            //根据报价核价需求录入表主键取模组数量 ===>这里暂时用的是核价表主键,以后要改
            List<ModelCount> modelCount = await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId)&&p.Id.Equals(productId));
            isALLConstructionDto.isAll = await GetStructuralIsAllEntering(auditFlowId);
            //总共的零件
            List<PartModel> partList = _resourceElectronicStructuralMethod.PartDtoList(modelCount);
            List<ConstructionDto> prop = await _resourceElectronicStructuralMethod.BOMConstructionBomList(partList, auditFlowId);// 结构料BOM单价表单
            isALLConstructionDto.ConstructionDtos = prop;
            return isALLConstructionDto;
        }
        /// <summary>
        /// 结构件单价录入  判断是否全部提交完毕  true 所有零件已录完   false  没有录完(仅仅加载结构料初始值(单个零件)可用)
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetStructuralIsAllEntering(long Id)
        {
            InitialStructuralDto initialElectronicDto = await GetStructural(Id);
            int AllCount = 0;//应该录入数据库这么多条数据
            initialElectronicDto.ConstructionBomList.ForEach(p => { AllCount += p.StructureMaterial.Count(); });
            //根据报价核价需求录入表主键取模组数量
            List<ModelCount> modelCount = await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            int Count = 0;
            foreach (var model in modelCount)
            {
                Count += await _configStructureElectronic.CountAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(model.Id) && p.IsSubmit);//数据库实际提交的条数
            }
            return AllCount == Count;
        }
        /// <summary>
        /// 计算电子料单价录入  根据汇率计算
        /// </summary>
        /// <param name="electronicBomList"></param>
        /// <returns></returns>
        public async Task<List<ElectronicDto>> PostElectronicMaterialCalculate(List<ElectronicDto> electronicBomList)
        {
            //计算电子料单价录入
            var electronicDtos = await _resourceElectronicStructuralMethod.ElectronicBomCalculateList(electronicBomList);
            return electronicDtos;
        }
        /// <summary>
        /// 计算电子料单价录入  根据汇率计算(单条数据)
        /// </summary>
        /// <param name="electronicBomList"></param>
        /// <returns></returns>
        public async Task<ElectronicDto> PostElectronicMaterialCalculateSingle(ElectronicDto electronicBomList)
        {
            List<ElectronicDto> electronicDtos = new();
            electronicDtos.Add(electronicBomList);
            //计算电子料单价录入
            List<ElectronicDto> electronicDtoLists = await _resourceElectronicStructuralMethod.ElectronicBomCalculateList(electronicDtos);
            return electronicDtoLists.FirstOrDefault();
        } 
        /// <summary>
        /// 计算电子料单价录入  根据原币计算
        /// </summary>
        /// <param name="electronicBomList"></param>
        /// <returns></returns>
        public async Task<List<ElectronicDto>> PosToriginalCurrencyCalculate(List<ElectronicDto> electronicBomList)
        {
            //计算电子料单价录入
            var electronicDtos = await _resourceElectronicStructuralMethod.ToriginalCurrencyCalculate(electronicBomList);
            return electronicDtos;
        }
        /// <summary>
        /// 计算电子料单价录入  根据原币计算(单条数据)
        /// </summary>
        /// <param name="electronicBomList"></param>
        /// <returns></returns>
        public async Task<ElectronicDto> PosToriginalCurrencyCalculateSingle(ElectronicDto electronicBomList)
        {
            List<ElectronicDto> electronicDtos = new();
            electronicDtos.Add(electronicBomList);
            //计算电子料单价录入          
            List<ElectronicDto> electronicDtoLists = await _resourceElectronicStructuralMethod.ToriginalCurrencyCalculate(electronicDtos);
            return electronicDtoLists.FirstOrDefault();
        }
        /// <summary>
        /// 电子料单价录入确认/提交 有则添加无则修改
        /// </summary>
        /// <param name="electronicDto"></param>
        /// <returns></returns>
        public async Task PostElectronicMaterialEntering(SubmitElectronicDto electronicDto)
        {
            if (electronicDto.IsSubmit)
            {
                //限制用户提交过快,数据库数据没有更新,导致插入多条权限数据
                if (!ResourceSubmitLog.IsPostElectronic.Contains(electronicDto.AuditFlowId))
                {
                    ResourceSubmitLog.IsPostElectronic.Add(electronicDto.AuditFlowId);
                    // 插入指定权限
                    await _flowAppService.InsertAssignJurisdiction(new AuditFlowRight()
                    {
                        AuditFlowId = electronicDto.AuditFlowId,
                        ProcessIdentifier = AuditFlowConsts.AF_ElecBomPriceAudit,

                    }, Host.ElectronicsPriceAuditor);
                }
                await _resourceElectronicStructuralMethod.SubmitElectronicMaterialEntering(electronicDto);

              
                if (await this.GetElectronicIsAllEntering(electronicDto.AuditFlowId, electronicDto))
                {
                    if (AbpSession.UserId is null)
                    {
                        throw new FriendlyException("请先登录");
                    }
                    ReturnDto retDto = await _flowAppService.UpdateAuditFlowInfo(new Audit.Dto.AuditFlowDetailDto()
                    {
                        AuditFlowId = electronicDto.AuditFlowId,
                        ProcessIdentifier = AuditFlowConsts.AF_ElectronicPriceInput,
                        UserId = AbpSession.UserId.Value,
                        Opinion = OPINIONTYPE.Submit_Agreee
                    });
                    ResourceSubmitLog.IsPostElectronic.Remove(electronicDto.AuditFlowId);
                }
            }
            else
            {                
                //电子料单价录入确认
                await _resourceElectronicStructuralMethod.ElectronicMaterialEntering(electronicDto);
               
            }

        }
        /// <summary>
        /// 电子料单价录入  判断是否全部提交完毕  true 所有零件已录完   false  没有录完
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetElectronicIsAllEntering(long Id, SubmitElectronicDto electronicDto)
        {
            InitialElectronicDto initialElectronicDto = await GetElectronic(Id);
            int AllCount = initialElectronicDto.ElectronicBomList.Count;//应该录入数据库这么多条数据
            //根据报价核价需求录入表主键取模组数量
            List<ModelCount> modelCount = await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            int Count = 0;
            foreach (var model in modelCount)
            {
                Count += await _configEnteringElectronic.CountAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(model.Id) && p.IsSubmit);//数据库实际提交的条数
            }

            List<ElectronicDto> electronicDtos = electronicDto.ElectronicDtoList;
            List<EnteringElectronic> enteringElectronics = new();
            foreach (ElectronicDto item in electronicDtos)
            {
                EnteringElectronic enteringElectronic = await _configEnteringElectronic.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(Id)&&p.IsEntering && p.ProductId.Equals(item.ProductId) && p.ElectronicId.Equals(item.ElectronicId));
                if (enteringElectronic is not null) enteringElectronics.Add(enteringElectronic);
            }
            electronicDtos=electronicDtos.Where(a => enteringElectronics.Where(t => a.ProductId == t.ProductId&&a.ElectronicId.Equals(t.ElectronicId)).Any()).ToList();
            Count+= electronicDtos.Count();

            return AllCount == Count;
        }
        /// <summary>
        ///  电子料单价录入  退回重置状态
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task GetElectronicConfigurationState(long Id)
        {
            List<EnteringElectronic> enteringElectronics = await _configEnteringElectronic.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            foreach (EnteringElectronic item in enteringElectronics)
            {
                item.IsSubmit=false;
                await _configEnteringElectronic.UpdateAsync(item);
            }
        }
        /// <summary>
        /// 电子料单价录入  退回重置状态 根据电子单价录入表id进行重置
        /// </summary>
        /// <param name="ElectronicId">电子单价录入表id</param>
        /// <returns></returns>
        public async Task GetElectronicConfigurationStateCertain(List<long> ElectronicId)
        {
            List<EnteringElectronic> enteringElectronics = await _configEnteringElectronic.GetAllListAsync(p => ElectronicId.Contains(p.Id));
            foreach (EnteringElectronic item in enteringElectronics)
            {
                item.IsSubmit = false;
                await _configEnteringElectronic.UpdateAsync(item);
            }
        }
        /// <summary>
        /// 结构件单价录入确认/提交 有则添加无则修改
        /// </summary>
        /// <param name="structuralMemberEnteringModel"></param>
        /// <returns></returns>        
        public async Task PostStructuralMemberEntering(StructuralMemberEnteringModel structuralMemberEnteringModel)
        {
            if (structuralMemberEnteringModel.IsSubmit)
            {
                //限制用户提交过快,数据库数据没有更新,导致插入多条权限数据
                if (!ResourceSubmitLog.IsPostStructural.Contains(structuralMemberEnteringModel.AuditFlowId))
                {
                    ResourceSubmitLog.IsPostStructural.Add(structuralMemberEnteringModel.AuditFlowId);
                    // 插入指定权限
                    await _flowAppService.InsertAssignJurisdiction(new AuditFlowRight()
                    {
                        AuditFlowId = structuralMemberEnteringModel.AuditFlowId,
                        ProcessIdentifier = AuditFlowConsts.AF_StructBomPriceAudit,

                    }, Host.StructuralPriceAuditor);                 
                }
                await _resourceElectronicStructuralMethod.SubmitStructuralMemberEntering(structuralMemberEnteringModel);               
                if (await this.GetStructuralIsAllEntering(structuralMemberEnteringModel.AuditFlowId, structuralMemberEnteringModel))
                {
                    if (AbpSession.UserId is null)
                    {
                        throw new FriendlyException("请先登录");
                    }
                    ReturnDto retDto = await _flowAppService.UpdateAuditFlowInfo(new Audit.Dto.AuditFlowDetailDto()
                    {
                        AuditFlowId = structuralMemberEnteringModel.AuditFlowId,
                        ProcessIdentifier = AuditFlowConsts.AF_StructPriceInput,
                        UserId = AbpSession.UserId.Value,
                        Opinion = OPINIONTYPE.Submit_Agreee
                    });
                    ResourceSubmitLog.IsPostStructural.Remove(structuralMemberEnteringModel.AuditFlowId);
                }
               
            }
            else
            {
                await _resourceElectronicStructuralMethod.StructuralMemberEntering(structuralMemberEnteringModel);
            }
        } 
     

        /// <summary>
        /// 结构件单价录入  判断是否全部提交完毕  true 所有零件已录完   false  没有录完
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetStructuralIsAllEntering(long Id, StructuralMemberEnteringModel structuralMemberEnteringModel)
        {
            InitialStructuralDto initialElectronicDto = await GetStructural(Id);
            int AllCount = 0;//应该录入数据库这么多条数据
            initialElectronicDto.ConstructionBomList.ForEach(p => { AllCount+=p.StructureMaterial.Count(); });
            //根据报价核价需求录入表主键取模组数量
            List<ModelCount> modelCount = await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            int Count = 0;
            foreach (var model in modelCount)
            {
                Count += await _configStructureElectronic.CountAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(model.Id) && p.IsSubmit);//数据库实际提交的条数
            }

            List<StructuralMaterialModel> structuralMaterialModels = structuralMemberEnteringModel.StructuralMaterialEntering;
            List<StructureElectronic> structureElectronics = new();
            foreach (StructuralMaterialModel item in structuralMaterialModels)
            {
                StructureElectronic structureElectronic = await _configStructureElectronic.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(Id)&&p.IsEntering&&p.ProductId.Equals(item.ProductId) && p.StructureId.Equals(item.StructureId));
                if (structureElectronic is not null) structureElectronics.Add(structureElectronic);
            }
            structuralMaterialModels=structuralMaterialModels.Where(a => structureElectronics.Where(t => a.ProductId == t.ProductId&&a.StructureId.Equals(t.StructureId)).Any()).ToList();
            Count+= structuralMaterialModels.Count();
            return AllCount==Count;
        }
        /// <summary>
        ///  结构件单价录入  退回重置状态
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task GetStructuralConfigurationState(long Id)
        {
            List<StructureElectronic> enteringElectronics = await _configStructureElectronic.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            foreach (StructureElectronic item in enteringElectronics)
            {
                item.IsSubmit=false;
                await _configStructureElectronic.UpdateAsync(item);
            }
        }
        /// <summary>
        /// 结构件单价录入  退回重置状态  根据结构单价表的id 进行重置
        /// </summary>
        /// <param name="StructuralId"></param>
        /// <returns></returns>
        public async Task GetStructuralConfigurationStateCertain(List<long> StructuralId)
        {
            List<StructureElectronic> enteringElectronics = await _configStructureElectronic.GetAllListAsync(p => StructuralId.Contains(p.Id));
            foreach (StructureElectronic item in enteringElectronics)
            {
                item.IsSubmit = false;
                await _configStructureElectronic.UpdateAsync(item);
            }
        }
        /// <summary>
        ///  计算结构料单价录入 ==>根据原币计算
        /// </summary>
        /// <param name="structuralMaterialModels"></param>
        /// <returns></returns>
        public async Task<List<ConstructionModel>> PosToriginalCurrencyStructural(List<ConstructionModel> structuralMaterialModels)
        {
            //计算电子料单价录入
            var structuralDto = await _resourceElectronicStructuralMethod.ToriginalCurrencyStructural(structuralMaterialModels);
            return structuralDto;
        }
        /// <summary>
        ///  计算结构料单价录入 ==>根据原币计算(单条数据)
        /// </summary>
        /// <param name="structuralMaterialModels"></param>
        /// <returns></returns>
        public async Task<ConstructionModel> PosToriginalCurrencyStructuralSingle(ConstructionModel structuralMaterialModels)
        {
            List<ConstructionModel> constructionModels = new();
            constructionModels.Add(structuralMaterialModels);
            //计算电子料单价录入
            List<ConstructionModel> structuralDto = await _resourceElectronicStructuralMethod.ToriginalCurrencyStructural(constructionModels);
            return structuralDto.FirstOrDefault();
        }
        /// <summary>
        ///  计算结构料单价录入 ==>根据年降计算
        /// </summary>
        /// <param name="structuralMaterialModels"></param>
        /// <returns></returns>
        public async Task<List<ConstructionModel>> PostStructuralMaterialCalculate(List<ConstructionModel> structuralMaterialModels)
        {       
            //计算电子料单价录入
            List<ConstructionModel> structuralDto = await _resourceElectronicStructuralMethod.StructuralBomCalculateList(structuralMaterialModels);
            return structuralDto;
        }
        /// <summary>
        ///  计算结构料单价录入 ==>根据年降计算(单条数据)
        /// </summary>
        /// <param name="structuralMaterialModel"></param>
        /// <returns></returns>
        public async Task<ConstructionModel> PostStructuralMaterialCalculateSingle(ConstructionModel structuralMaterialModel)
        {
            List<ConstructionModel> constructionModels = new();
            constructionModels.Add(structuralMaterialModel);
            //计算电子料单价录入
            List<ConstructionModel> structuralDto = await _resourceElectronicStructuralMethod.StructuralBomCalculateList(constructionModels);
            return structuralDto.FirstOrDefault();
        }
        /// <summary>
        /// 查看项目走量(获取某流程的项目走量)
        /// </summary>
        /// <param name="Id">流程id</param>
        /// <returns></returns>
        public async Task<List<ModuleNumberDto>> GetProjectGoQuantity(long Id)
        {
            try
            {
                //模组数量
                var ResourceModelCount = await _resourceModelCount.GetAllListAsync(a => a.AuditFlowId == Id);
                //财务字典表明细
                List<FinanceDictionaryDetail> ResourceFinanceDictionaryDetail = await _resourceFinanceDictionaryDetail.GetAllListAsync();
                List<ModuleNumberDto> price = (from a in ResourceModelCount
                                               join d in ResourceFinanceDictionaryDetail on a.ProductType equals d.Id into d1
                                               from e_d1 in d1.DefaultIfEmpty()
                                               select new ModuleNumberDto
                                               {
                                                   ModelCountId=a.Id,
                                                   PartNumber=a.PartNumber,
                                                   ProductName=a.Product,
                                                   ProductTypeName=(e_d1 is null) ? "" : e_d1.DisplayName,
                                                   MarketShare=a.MarketShare,
                                                   ModuleCarryingRate=a.ModuleCarryingRate,
                                                   SingleCarProductsQuantity=a.SingleCarProductsQuantity,
                                                   ModelTotal=a.ModelTotal,
                                               }).ToList();
                foreach (var item in price)
                {
                    item.ModelCountYear= (from b in _resourceModelCountYear.GetAllList(p => p.ModelCountId.Equals(item.ModelCountId))
                                          select new YearOrValueMode
                                          {
                                              Year=b.Year,
                                              Value=b.Quantity
                                          }).ToList();
                }
                return price;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }

        }


    }

}
