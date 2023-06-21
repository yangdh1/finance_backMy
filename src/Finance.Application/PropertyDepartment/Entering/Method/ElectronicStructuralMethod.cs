using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using Abp.UI;
using Finance.Authorization.Users;
using Finance.Entering;
using Finance.Entering.Model;
using Finance.FinanceMaintain;
using Finance.Infrastructure;
using Finance.PriceEval;
using Finance.ProductDevelopment;
using Finance.PropertyDepartment.Entering.Dto;
using Finance.PropertyDepartment.Entering.Model;
using Finance.Roles.Dto;
using Finance.Users;
using Finance.Users.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Finance.Authorization.Roles.StaticRoleNames;

namespace Finance.PropertyDepartment.Entering.Method
{
    /// <summary>
    /// 
    /// </summary>
    public class ElectronicStructuralMethod : AsyncCrudAppService<User, UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>, ISingletonDependency
    {
        /// <summary>
        /// 财务字典表明细
        /// </summary>
        private static IRepository<FinanceDictionaryDetail, string> _resourceFinanceDictionaryDetail;
        /// <summary>
        /// 产品开发部电子BOM输入信息
        /// </summary>
        private static IRepository<ElectronicBomInfo, long> _resourceElectronicBomInfo;
        /// <summary>
        /// 产品开发部结构BOM输入信息
        /// </summary>
        private static IRepository<StructureBomInfo, long> _resourceStructureBomInfo;
        /// <summary>
        /// 模组数量
        /// </summary>
        private static IRepository<ModelCount, long> _resourceModelCount;
        /// <summary>
        /// 模组数量年份
        /// </summary>
        private static IRepository<ModelCountYear, long> _resourceModelCountYear;
        /// <summary>
        /// 基础单价库实体类
        /// </summary>
        private static IRepository<UInitPriceForm, long> _configUInitPriceForm;
        /// <summary>
        ///  财务维护 汇率表
        /// </summary>
        private static IRepository<ExchangeRate, long> _configExchangeRate;
        /// <summary>
        /// 资源部电子物料录入
        /// </summary>
        private static IRepository<EnteringElectronic, long> _configEnteringElectronic;
        /// <summary>
        /// 资源部结构物料录入
        /// </summary>
        private static IRepository<StructureElectronic, long> _configStructureElectronic;
        /// <summary>
        /// 实体映射
        /// </summary>
        private static new IObjectMapper ObjectMapper;
        /// <summary>
        /// 
        /// </summary>
        private static UserManager _userManager;
        /// <summary>
        /// 电子BOM两次上传差异化表
        /// </summary>
        private static IRepository<ElecBomDifferent, long> _configElecBomDifferent;
        /// <summary>
        /// 结构BOM两次上传差异化表
        /// </summary>
        private static IRepository<StructBomDifferent, long> _configStructBomDifferent;
        private readonly IUserAppService _userAppService;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="resourceFinanceDictionaryDetail"></param>
        /// <param name="electronicBomInfo"></param>
        /// <param name="structureBomInfo"></param>
        /// <param name="objectMapper"></param>
        /// <param name="modelCount"></param>
        /// <param name="modelCountYear"></param>
        /// <param name="uInitPriceForm"></param>
        /// <param name="exchangeRate"></param>
        /// <param name="enteringElectronic"></param>
        /// <param name="userManager"></param>
        /// <param name="repository"></param>
        /// <param name="structureElectronic"></param>
        /// <param name="elecBomDifferent"></param>
        /// <param name="structBomDifferent"></param>
        /// <param name="userAppService"></param>
        public ElectronicStructuralMethod(
            IRepository<FinanceDictionaryDetail,
            string> resourceFinanceDictionaryDetail,
            IRepository<ElectronicBomInfo, long> electronicBomInfo,
            IRepository<StructureBomInfo, long> structureBomInfo,
            IObjectMapper objectMapper,
            IRepository<ModelCount, long> modelCount,
            IRepository<ModelCountYear, long> modelCountYear,
            IRepository<UInitPriceForm, long> uInitPriceForm,
            IRepository<ExchangeRate, long> exchangeRate,
            IRepository<EnteringElectronic, long> enteringElectronic,
            UserManager userManager,
            IRepository<User, long> repository,
            IRepository<StructureElectronic, long> structureElectronic,
            IRepository<ElecBomDifferent, long> elecBomDifferent,
            IRepository<StructBomDifferent, long> structBomDifferent,
            IUserAppService userAppService) : base(repository)
        {
            _resourceFinanceDictionaryDetail = resourceFinanceDictionaryDetail;
            _resourceElectronicBomInfo = electronicBomInfo;
            ObjectMapper = objectMapper;
            _resourceStructureBomInfo = structureBomInfo;
            _resourceModelCount = modelCount;
            _resourceModelCountYear = modelCountYear;
            _configUInitPriceForm = uInitPriceForm;
            _configExchangeRate = exchangeRate;
            _configEnteringElectronic = enteringElectronic;
            _userManager = userManager;
            _configStructureElectronic = structureElectronic;
            _configElecBomDifferent = elecBomDifferent;
            _configStructBomDifferent = structBomDifferent;
            _userAppService = userAppService;
        }

        /// <summary>
        /// resourceDepartmentDto.PartDtoList零件的计算
        /// </summary>
        internal List<PartModel> PartDtoList(List<ModelCount> price)
        {
            List<PartModel> partModel = (from a in price
                                         select new PartModel
                                         {
                                             ProductId = a.Id,
                                             ParName = a.Product,
                                         }).ToList();
            return partModel;
        }
        /// <summary>
        /// resourceDepartmentDto.ElectronicBomList电子BOM的计算   总共的零件   流程表id
        /// </summary>
        /// <param name="price">总共的零件</param>
        /// <param name="Id">流程表id</param>
        /// <returns></returns>
        internal async Task<List<ElectronicDto>> ElectronicBomList(List<PartModel> price, long Id)
        {
            List<ElectronicDto> electronicBomList = new List<ElectronicDto>();
            //p总共的零件
            foreach (PartModel item in price)
            {
                //通过零件号获取 模组数量中的 年度模组数量以及年份               
                List<ModelCountYear> modelCountYearList = (from a in await _resourceModelCountYear.GetAllListAsync(p => p.ModelCountId.Equals(item.ProductId))
                                                           select new ModelCountYear
                                                           {
                                                               ModelCountId = a.ModelCountId,
                                                               Year = a.Year,
                                                               Quantity = a.Quantity,
                                                           }).ToList();
                List<ElectronicBomInfo> electronicBomInfo = await _resourceElectronicBomInfo.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(item.ProductId) && p.IsInvolveItem.Equals("是"));
                //循环查询到的 电子料BOM表单
                foreach (ElectronicBomInfo BomInfo in electronicBomInfo)
                {
                    ElectronicDto electronicDto = new();
                    //将电子料BOM映射到ElectronicDto
                    electronicDto = ObjectMapper.Map<ElectronicDto>(BomInfo);
                    //通过 流程id  零件id  物料表单 id  查询数据库是否有信息,如果有信息就说明以及确认过了,然后就拿去之前确认过的信息
                    EnteringElectronic enteringElectronic = await _configEnteringElectronic.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(item.ProductId) && p.ElectronicId.Equals(BomInfo.Id));
                    if (enteringElectronic != null)
                    {
                        //将电子料BOM映射到ElectronicDto
                        electronicDto = ObjectMapper.Map<ElectronicDto>(enteringElectronic);
                        electronicDto.CategoryName = BomInfo.CategoryName;//物料大类
                        electronicDto.TypeName = BomInfo.TypeName;//物料种类
                        electronicDto.SapItemNum = BomInfo.SapItemNum;//物料编号
                        electronicDto.SapItemName = BomInfo.SapItemName;//材料名称                        
                        var user = GetAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = enteringElectronic.PeopleId });
                        if (user.Result is not null) electronicDto.PeopleName = user.Result.Name;
                        electronicBomList.Add(electronicDto);
                        continue;//直接进行下一个循环
                    }
                    //项目物料的使用量
                    electronicDto.MaterialsUseCount = new List<YearOrValueMode>();
                    foreach (ModelCountYear modelCountYear in modelCountYearList)
                    {
                        YearOrValueMode yearOrValueMode = new YearOrValueMode { Year = modelCountYear.Year, Value = (decimal)BomInfo.AssemblyQuantity * modelCountYear.Quantity };
                        electronicDto.MaterialsUseCount.Add(yearOrValueMode);
                    }
                    //取基础单价库  查询条件 物料编码 冻结状态  有效结束日期
                    List<UInitPriceForm> uInitPriceForms = await _configUInitPriceForm.GetAllListAsync(p => p.StockNumber.Equals(BomInfo.SapItemNum) && !p.FrozenState && p.EffectiveEndDate > DateTime.Now);//&&!p.FrozenState&&p.EffectiveEndDate>DateTime.Now
                    //通过优先级筛选
                    List<UInitPriceForm> uInitPriceFormsPriority = uInitPriceForms.Where(p => p.Priority).ToList();
                    List<UInitPriceForm> uInitPriceForm = new(); //取价格比较低的供应商
                    if (uInitPriceFormsPriority.Count > 0)
                    {
                        //如果有有 核心的商家 取价格比较低的
                        uInitPriceForm = uInitPriceFormsPriority.OrderBy(p => p.NetPrice).ToList();
                    }
                    else
                    {
                        //如果没有 核心的商家 取现货/临时 价格比较低的商家
                        uInitPriceForm = uInitPriceForms.OrderBy(p => p.NetPrice).ToList();
                    }
                    electronicDto.ECCNCode = uInitPriceForm.Count != 0 ? uInitPriceForm.Select(s => s.ECCNCode).First() : "待定";//获取ECCN码
                    electronicDto.Currency = uInitPriceForm.Count != 0 ? uInitPriceForm.Select(s => s.Currencies).First() : "";//获取币种
                    electronicDto.SystemiginalCurrency = CalculateUnitPrice(modelCountYearList, uInitPriceForm, electronicDto.MaterialsUseCount);//系统单价（原币）
                    electronicDto.InTheRate = CalculateUnitPrice(modelCountYearList, new List<UInitPriceForm>(), new List<YearOrValueMode>());//项目物料的年将率
                    electronicDto.IginalCurrency = electronicDto.InTheRate = CalculateUnitPrice(modelCountYearList, new List<UInitPriceForm>(), new List<YearOrValueMode>());//原币
                    electronicDto.StandardMoney = electronicDto.InTheRate = CalculateUnitPrice(modelCountYearList, new List<UInitPriceForm>(), new List<YearOrValueMode>());//本位币
                    electronicBomList.Add(electronicDto);
                }
            }
            return electronicBomList;
        }
        /// <summary>
        /// 单个 物料编号的计算
        /// </summary>
        /// <param name="price"></param>
        /// <param name="Id"></param>
        /// <param name="ElectronicId"></param>
        /// <returns></returns>
        internal async Task<ElectronicDto> ElectronicBom(long ProductId, long Id, long ElectronicId)
        {
            ElectronicBomInfo electronicBomInfo = await _resourceElectronicBomInfo.FirstOrDefaultAsync(p => p.Id.Equals(ElectronicId) && p.AuditFlowId.Equals(Id) && p.ProductId.Equals(ProductId) && p.IsInvolveItem.Equals("是"));
            //通过零件号获取 模组数量中的 年度模组数量以及年份               
            List<ModelCountYear> modelCountYearList = (from a in await _resourceModelCountYear.GetAllListAsync(p => p.ModelCountId.Equals(ProductId))
                                                       select new ModelCountYear
                                                       {
                                                           ModelCountId = a.ModelCountId,
                                                           Year = a.Year,
                                                           Quantity = a.Quantity,
                                                       }).ToList();
            ElectronicDto electronicDto = new();
            //将电子料BOM映射到ElectronicDto
            electronicDto = ObjectMapper.Map<ElectronicDto>(electronicBomInfo);
            //项目物料的使用量
            electronicDto.MaterialsUseCount = new List<YearOrValueMode>();
            foreach (ModelCountYear modelCountYear in modelCountYearList)
            {
                YearOrValueMode yearOrValueMode = new YearOrValueMode { Year = modelCountYear.Year, Value = (decimal)electronicBomInfo.AssemblyQuantity * modelCountYear.Quantity };
                electronicDto.MaterialsUseCount.Add(yearOrValueMode);
            }
            //取基础单价库  查询条件 物料编码 冻结状态  有效结束日期
            List<UInitPriceForm> uInitPriceForms = await _configUInitPriceForm.GetAllListAsync(p => p.StockNumber.Equals(electronicBomInfo.SapItemNum) && !p.FrozenState && p.EffectiveEndDate > DateTime.Now);//&&!p.FrozenState&&p.EffectiveEndDate>DateTime.Now
                                                                                                                                                                                                               //通过优先级筛选
            List<UInitPriceForm> uInitPriceFormsPriority = uInitPriceForms.Where(p => p.Priority).ToList();
            List<UInitPriceForm> uInitPriceForm = new(); //取价格比较低的供应商
            if (uInitPriceFormsPriority.Count > 0)
            {
                //如果有有 核心的商家 取价格比较低的
                uInitPriceForm = uInitPriceFormsPriority.OrderBy(p => p.NetPrice).ToList();
            }
            else
            {
                //如果没有 核心的商家 取现货/临时 价格比较低的商家
                uInitPriceForm = uInitPriceForms.OrderBy(p => p.NetPrice).ToList();
            }
            electronicDto.ECCNCode = uInitPriceForm.Count != 0 ? uInitPriceForm.Select(s => s.ECCNCode).First() : "待定";//ECCN
            electronicDto.Currency = uInitPriceForm.Count != 0 ? uInitPriceForm.Select(s => s.Currencies).First() : "无";//获取币种
            electronicDto.SystemiginalCurrency = CalculateUnitPrice(modelCountYearList, uInitPriceForm, electronicDto.MaterialsUseCount);//系统单价（原币）
            electronicDto.InTheRate = CalculateUnitPrice(modelCountYearList, new List<UInitPriceForm>(), new List<YearOrValueMode>());//项目物料的年奖励
            electronicDto.IginalCurrency = electronicDto.InTheRate = CalculateUnitPrice(modelCountYearList, new List<UInitPriceForm>(), new List<YearOrValueMode>());//原币
            electronicDto.StandardMoney = electronicDto.InTheRate = CalculateUnitPrice(modelCountYearList, new List<UInitPriceForm>(), new List<YearOrValueMode>());//本位币

            return electronicDto;
        }
        /// <summary>
        /// resourceDepartmentDto.ElectronicBomList电子BOM的计算   总共的零件   流程表id
        /// </summary>
        /// <param name="price">总共的零件</param>
        /// <param name="Id">流程表id</param>
        /// <returns></returns>
        internal async Task<List<ElectronicDto>> BOMElectronicBomList(List<PartModel> price, long Id)
        {

            ListResultDto<RoleDto> Roles = await _userAppService.GetRolesByUserId(AbpSession.GetUserId());
            List<RoleDto> PM = Roles.Items.Where(p => p.Name.Equals(Host.ProjectManager)).ToList();//项目经理
            List<ElectronicDto> electronicBomList = new List<ElectronicDto>();
            //p总共的零件
            foreach (PartModel item in price)
            {
                //获取电子料bom表单  根据流程主键id 和 每一个零件的id  
                List<ElectronicBomInfo> electronicBomInfo = await _resourceElectronicBomInfo.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(item.ProductId) && p.IsInvolveItem.Equals("是"));
                //循环查询到的 电子料BOM表单
                foreach (ElectronicBomInfo BomInfo in electronicBomInfo)
                {
                    ElectronicDto electronicDto = new();
                    //将电子料BOM映射到ElectronicDto
                    electronicDto = ObjectMapper.Map<ElectronicDto>(BomInfo);
                    //通过 流程id  零件id  物料表单 id  查询数据库是否有信息,如果有信息就说明以及确认过了,然后就拿去之前确认过的信息
                    EnteringElectronic enteringElectronic = await _configEnteringElectronic.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(item.ProductId) && p.ElectronicId.Equals(BomInfo.Id) && p.IsSubmit);
                    if (enteringElectronic != null)
                    {
                        //将电子料BOM映射到ElectronicDto
                        electronicDto = ObjectMapper.Map<ElectronicDto>(enteringElectronic);
                        electronicDto.CategoryName = BomInfo.CategoryName;//物料大类
                        electronicDto.TypeName = BomInfo.TypeName;//物料种类
                        electronicDto.SapItemNum = BomInfo.SapItemNum;//物料编号
                        electronicDto.SapItemName = BomInfo.SapItemName;//材料名称
                        var user = GetAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = enteringElectronic.PeopleId });
                        if (user.Result is not null) electronicDto.PeopleName = user.Result.Name;
                        if (PM.Count is not 0)
                        {
                            electronicDto.RebateMoney = null;
                        }
                        electronicBomList.Add(electronicDto);
                        continue;//直接进行下一个循环
                    }

                }
            }
            return electronicBomList;
        }
        /// <summary>
        /// 结构件单价录入 返回
        /// </summary>
        /// <param name="price"></param>
        /// <param name="Id"></param>
        /// <param name="longs"></param>
        /// <returns></returns>
        internal async Task<List<ConstructionDto>> ConstructionBomList(List<PartModel> price, long Id, List<long> longs, List<long> longs1)
        {
            List<ConstructionDto> constructionDtos = new List<ConstructionDto>();

            foreach (PartModel item in price)//循环模组
            {
                //通过零件号获取 模组数量中的 及年份               
                List<YearOrValueMode> yearOrValueMode = (from a in await _resourceModelCountYear.GetAllListAsync(p => p.ModelCountId.Equals(item.ProductId))
                                                         select new YearOrValueMode
                                                         {
                                                             Year = a.Year,
                                                         }).ToList();
                //通过零件号获取 模组数量中的 年度模组数量以及年份               
                List<ModelCountYear> modelCountYearList = (from a in await _resourceModelCountYear.GetAllListAsync(p => p.ModelCountId.Equals(item.ProductId))
                                                           select new ModelCountYear
                                                           {
                                                               ModelCountId = a.ModelCountId,
                                                               Year = a.Year,
                                                               Quantity = a.Quantity,
                                                           }).ToList();
                List<StructureBomInfo> structureBomInfos = _resourceStructureBomInfo.GetAllList(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(item.ProductId) && p.IsInvolveItem.Contains("是"));
                List<string> structureBomInfosGr = structureBomInfos.GroupBy(p => p.SuperTypeName).Select(c => c.First()).Select(s => s.SuperTypeName).ToList(); //根据超级大类 去重
                foreach (string SuperTypeName in structureBomInfosGr)//超级大种类  结构料 胶水等辅材 SMT外协 包材
                {
                    List<StructureBomInfo> StructureMaterialnfp = structureBomInfos.Where(p => p.SuperTypeName.Equals(SuperTypeName)).ToList(); //查找属于这一超级大类的
                    List<ConstructionModel> constructionModels = ObjectMapper.Map<List<ConstructionModel>>(StructureMaterialnfp);// 结构BOM表单 模型

                    foreach (ConstructionModel construction in constructionModels)
                    {
                        int count = longs.Where(p => p.Equals(construction.StructureId)).Count();//如果改id删除了就跳过
                        if (count != 0)
                        {
                            continue;//直接进行下一个循环
                        }
                        #region 获取物料使用量
                        construction.MaterialsUseCount = new List<YearOrValueMode>();
                        foreach (ModelCountYear modelCountYear in modelCountYearList)
                        {
                            YearOrValueMode yearOrValue = new YearOrValueMode { Year = modelCountYear.Year, Value = (decimal)construction.AssemblyQuantity * modelCountYear.Quantity };
                            construction.MaterialsUseCount.Add(yearOrValue);
                        }
                        #endregion
                        #region 获取ECCN/系统单价（原币)/币种
                        //取基础单价库  查询条件 物料编码 冻结状态  有效结束日期
                        List<UInitPriceForm> uInitPriceForms = await _configUInitPriceForm.GetAllListAsync(p => p.StockNumber.Equals(construction.SapItemNum) && !p.FrozenState && p.EffectiveEndDate > DateTime.Now);//&&!p.FrozenState&&p.EffectiveEndDate>DateTime.Now
                                                                                                                                                                                                                      //通过优先级筛选
                        List<UInitPriceForm> uInitPriceFormsPriority = uInitPriceForms.Where(p => p.Priority).ToList();
                        List<UInitPriceForm> uInitPriceForm = new(); //取价格比较低的供应商
                        if (uInitPriceFormsPriority.Count > 0)
                        {
                            //如果有有 核心的商家 取价格比较低的
                            uInitPriceForm = uInitPriceFormsPriority.OrderBy(p => p.NetPrice).ToList();
                        }
                        else
                        {
                            //如果没有 核心的商家 取现货/临时 价格比较低的商家
                            uInitPriceForm = uInitPriceForms.OrderBy(p => p.NetPrice).ToList();
                        }
                        construction.SystemiginalCurrency = CalculateUnitPrice(modelCountYearList, uInitPriceForm, construction.MaterialsUseCount);//系统单价（原币）
                        construction.Currency = uInitPriceForm.Count != 0 ? uInitPriceForm.Select(s => s.Currencies).First() : "";//获取币种
                        #endregion
                        //通过 流程id  零件id  物料表单 id  查询数据库是否有信息,如果有信息就说明以及确认过了,然后就拿去之前确认过的信息
                        StructureElectronic structureElectronic = await _configStructureElectronic.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(item.ProductId) && p.StructureId.Equals(construction.StructureId));
                        if (structureElectronic != null)
                        {
                            construction.ECCNCode = structureElectronic.ECCNCode;//ECCN
                            construction.Currency = structureElectronic.Currency;//币种                          
                            construction.Sop = JsonConvert.DeserializeObject<List<YearOrValueMode>>(structureElectronic.Sop);//Sop
                            //SOP 根据年份去重
                            construction.Sop = construction.Sop.GroupBy(p => p.Year).Select(item => item.First()).ToList();
                            construction.IginalCurrency = JsonConvert.DeserializeObject<List<YearOrValueMode>>(structureElectronic.IginalCurrency);//原币
                            //原币 根据年份去重
                            construction.IginalCurrency = construction.IginalCurrency.GroupBy(p => p.Year).Select(item => item.First()).ToList();
                            construction.StandardMoney = JsonConvert.DeserializeObject<List<YearOrValueMode>>(structureElectronic.StandardMoney);//本位币
                            //本位币 根据年份去重
                            construction.StandardMoney = construction.StandardMoney.GroupBy(p => p.Year).Select(item => item.First()).ToList();
                            if (structureElectronic.InTheRate is not null)
                            {
                                construction.InTheRate = JsonConvert.DeserializeObject<List<YearOrValueMode>>(structureElectronic.InTheRate);//项目物料的年降率
                            }
                            else
                            {
                                construction.InTheRate = yearOrValueMode;//年降
                            }
                            construction.RebateMoney = structureElectronic.RebateMoney;//物料可返回金额
                            construction.PeopleId = structureElectronic.PeopleId;//确认人
                            construction.IsSubmit = structureElectronic.IsSubmit;//是否提交
                            construction.IsEntering = structureElectronic.IsEntering;//是否录入
                            construction.MOQ = structureElectronic.MOQ;//MOQ
                            construction.Remark = structureElectronic.Remark;//备注
                            var user = GetAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = structureElectronic.PeopleId });
                            if (user.Result is not null) construction.PeopleName = user.Result.Name;
                            int countUp = longs1.Where(p => p.Equals(construction.StructureId)).Count();//如果修改了,重置参数
                            if (countUp != 0)
                            {
                                construction.Sop = yearOrValueMode;//sop
                                construction.IginalCurrency = yearOrValueMode;//原币
                                construction.StandardMoney = yearOrValueMode;//本位币
                                construction.InTheRate = yearOrValueMode;//年降
                                construction.IsEntering = false;//确认重置
                                construction.IsSubmit = false;//修改重置
                                continue;//直接进行下一个循环
                            }
                            continue;//直接进行下一个循环
                        }
                        construction.ECCNCode = uInitPriceForm.Count != 0 ? uInitPriceForm.Select(s => s.ECCNCode).First() : "待定";//ECCN
                        construction.Sop = yearOrValueMode;//sop                      
                        construction.IginalCurrency = yearOrValueMode;//原币
                        construction.StandardMoney = yearOrValueMode;//本位币
                        construction.InTheRate = yearOrValueMode;//年降
                    }
                    ConstructionDto constructionDto = new ConstructionDto()
                    {
                        ProductId = item.ProductId,
                        SuperTypeName = SuperTypeName,
                        StructureMaterial = constructionModels,
                    };

                    constructionDtos.Add(constructionDto);
                }
            }
            return constructionDtos;
        }
        /// <summary>
        /// 结构件单价录入 返回
        /// </summary>
        /// <param name="price">总共的零件</param>
        /// <param name="Id">流程表id</param>
        /// <returns></returns>
        internal async Task<List<ConstructionDto>> BOMConstructionBomList(List<PartModel> price, long Id)
        {
            ListResultDto<RoleDto> Roles = await _userAppService.GetRolesByUserId(AbpSession.GetUserId());
            List<RoleDto> PM = Roles.Items.Where(p => p.Name.Equals(Host.ProjectManager)).ToList();//项目经理
            List<ConstructionDto> constructionDtos = new List<ConstructionDto>();
            foreach (PartModel item in price)//循环模组
            {
                List<StructureBomInfo> structureBomInfos = _resourceStructureBomInfo.GetAllList(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(item.ProductId) && p.IsInvolveItem.Contains("是"));
                List<string> structureBomInfosGr = structureBomInfos.GroupBy(p => p.SuperTypeName).Select(c => c.First()).Select(s => s.SuperTypeName).ToList(); //根据超级大类 去重
                foreach (string SuperTypeName in structureBomInfosGr)//超级大种类  结构料 胶水等辅材 SMT外协 包材
                {
                    List<StructureBomInfo> StructureMaterialnfp = structureBomInfos.Where(p => p.SuperTypeName.Equals(SuperTypeName)).ToList(); //查找属于这一超级大类的
                    List<ConstructionModel> constructionModels = ObjectMapper.Map<List<ConstructionModel>>(StructureMaterialnfp);// 结构BOM表单 模型

                    List<ConstructionModel> RemoveconstructionModels = new List<ConstructionModel>();
                    foreach (ConstructionModel construction in constructionModels)
                    {
                        //通过 流程id  零件id  物料表单 id  查询数据库是否有信息,如果有信息就说明以及确认过了,然后就拿去之前确认过的信息
                        StructureElectronic structureElectronic = await _configStructureElectronic.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(item.ProductId) && p.StructureId.Equals(construction.StructureId) && p.IsSubmit);
                        if (structureElectronic != null)
                        {
                            construction.Id = structureElectronic.Id;//id
                            construction.ECCNCode = structureElectronic.ECCNCode;//ECCN
                            construction.Currency = structureElectronic.Currency;//币种                          
                            construction.Sop = JsonConvert.DeserializeObject<List<YearOrValueMode>>(structureElectronic.Sop);//Sop
                            if (structureElectronic.MaterialsUseCount is not null)
                            {
                                construction.MaterialsUseCount = JsonConvert.DeserializeObject<List<YearOrValueMode>>(structureElectronic.MaterialsUseCount);//项目物料的使用量 
                            }
                            else
                            {
                                construction.MaterialsUseCount = JsonConvert.DeserializeObject<List<YearOrValueMode>>(structureElectronic.Sop);
                            }
                            if (structureElectronic.SystemiginalCurrency is not null)
                            {
                                construction.SystemiginalCurrency = JsonConvert.DeserializeObject<List<YearOrValueMode>>(structureElectronic.SystemiginalCurrency);//系统单价（原币）

                            }
                            else
                            {
                                construction.SystemiginalCurrency = JsonConvert.DeserializeObject<List<YearOrValueMode>>(structureElectronic.Sop);
                            }
                            if (structureElectronic.InTheRate is not null)
                            {
                                construction.InTheRate = JsonConvert.DeserializeObject<List<YearOrValueMode>>(structureElectronic.InTheRate);//项目物料的年降率 
                            }
                            else
                            {
                                construction.InTheRate = JsonConvert.DeserializeObject<List<YearOrValueMode>>(structureElectronic.Sop);
                            }
                            construction.IginalCurrency = JsonConvert.DeserializeObject<List<YearOrValueMode>>(structureElectronic.IginalCurrency);//原币 
                            construction.StandardMoney = JsonConvert.DeserializeObject<List<YearOrValueMode>>(structureElectronic.StandardMoney);//本位币 
                            if (PM.Count is not 0)
                            {
                                construction.RebateMoney = null;
                            }
                            else
                            {
                                construction.RebateMoney = structureElectronic.RebateMoney;//物料可返回金额
                            }
                            construction.MOQ = structureElectronic.MOQ;//MOQ
                            construction.PeopleId = structureElectronic.PeopleId;//确认人
                            construction.Remark = structureElectronic.Remark;//备注
                            var user = GetAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = structureElectronic.PeopleId });
                            if (user.Result is not null) construction.PeopleName = user.Result.Name;
                            continue;//直接进行下一个循环
                        }else
                        {
                            RemoveconstructionModels.Add(construction);                         
                        }
                    }
                    foreach (var Removeconstruction in RemoveconstructionModels)
                    {
                        constructionModels.Remove(Removeconstruction);  
                    }
                    ConstructionDto constructionDto = new ConstructionDto()
                    {
                        ProductId = item.ProductId,
                        SuperTypeName = SuperTypeName,
                        StructureMaterial = constructionModels,
                    };
                    constructionDtos.Add(constructionDto);
                }
            }
            return constructionDtos;
        }
        /// <summary>
        /// 计算单价  年份  价格比较低的供应商 物料的使用量
        /// </summary>
        internal static List<YearOrValueMode> CalculateUnitPrice(List<ModelCountYear> p, List<UInitPriceForm> m, List<YearOrValueMode> y)
        {
            try
            {
                List<YearOrValueMode> yearOrValueModes = new List<YearOrValueMode>();
                //判断是否存在阶梯价
                if (m.Count > 1)
                {
                    //阶梯价
                    foreach (ModelCountYear item in p) //循环年份
                    {
                        foreach (YearOrValueMode yearOrValue in y)//循环物料使用量
                        {
                            if (item.Year == yearOrValue.Year)
                            {
                                List<UInitPriceForm> price = m.Where(p => p.Upper > yearOrValue.Value && p.Floor < yearOrValue.Value).ToList();//阶梯单价                  
                                if (price.Count is not 0)
                                {
                                    UInitPriceForm priceFirst = price.OrderBy(p => p.NetPrice).FirstOrDefault();  //没值取价格最低的
                                    YearOrValueMode yearOrValueMode = new YearOrValueMode() { Year = item.Year, Value = priceFirst.NetPrice / 1000 };//没值取价格最低的
                                    yearOrValueModes.Add(yearOrValueMode);
                                }
                                else
                                {
                                    UInitPriceForm priceFirst = m.OrderBy(p => p.NetPrice).FirstOrDefault();  //没值取价格最低
                                    yearOrValueModes.Add(new YearOrValueMode() { Year = item.Year, Value = priceFirst.NetPrice / 1000 });//有值 取 上限 和 下限之间的 有且有一个
                                }
                            }
                        }
                    }
                }
                else if (m.Count == 1)
                {
                    //单个价格 无阶梯价
                    foreach (ModelCountYear item in p)
                    {
                        YearOrValueMode yearOrValueMode = new YearOrValueMode() { Year = item.Year, Value = m.First().NetPrice / 1000 };//单个价格
                        yearOrValueModes.Add(yearOrValueMode);
                    }
                }
                else
                {
                    //没有单价
                    foreach (ModelCountYear item in p)
                    {
                        YearOrValueMode yearOrValueMode = new YearOrValueMode() { Year = item.Year, Value = 0.0M };
                        yearOrValueModes.Add(yearOrValueMode);
                    }
                }
                return yearOrValueModes;
            }
            catch (Exception e)
            {

                throw new UserFriendlyException(e.Message);
            }

        }
        /// <summary>
        /// 计算电子料单价录入 ==>根据年将率计算
        /// </summary>
        internal async Task<List<ElectronicDto>> ElectronicBomCalculateList(List<ElectronicDto> Price)
        {
            try
            {
                List<ElectronicDto> list = new List<ElectronicDto>();
                //循环电子BOM单价表单
                foreach (var item in Price)
                {
                    //实体
                    List<YearOrValueMode> YearOrValueMode = new List<YearOrValueMode>();
                    //循环过的 年将率
                    List<decimal> priceAll = new();
                    //循环 系统单价
                    foreach (YearOrValueMode yearOrValue in item.SystemiginalCurrency)
                    {
                        //手工录入该年份的物料年将率  按 % 百分制来算
                        decimal price = item.InTheRate.Where(m => m.Year.Equals(yearOrValue.Year)).Select(m => m.Value).First();
                        priceAll.Add(price);
                        //原币
                        decimal Value = yearOrValue.Value;
                        priceAll.ForEach(item =>
                        {
                            Value = Value * (1 - item / 100);
                        });
                        //YearOrValueMode yearOrValueMode = new() { Year=yearOrValue.Year, Value=(yearOrValue.Value)*(1-price/100) };
                        YearOrValueMode yearOrValueMode = new() { Year = yearOrValue.Year, Value = Value };
                        if (yearOrValueMode is not null) YearOrValueMode.Add(yearOrValueMode);
                    }
                    //添加计算后的原币
                    item.IginalCurrency = JsonConvert.DeserializeObject<List<YearOrValueMode>>(JsonConvert.SerializeObject(YearOrValueMode));
                    //获取汇率
                    ExchangeRate exchangeRate = await _configExchangeRate.FirstOrDefaultAsync(p => p.ExchangeRateKind.Equals(item.Currency));
                    //本位币实体
                    //计算本位币
                    if (exchangeRate is not null)
                    {
                        List<YearOrValueMode> IginalCurrency = JsonConvert.DeserializeObject<List<YearOrValueMode>>(JsonConvert.SerializeObject(YearOrValueMode));
                        YearOrValueMode.Clear();
                        List<YearOrValueMode> yearOrValueModes11 = new List<YearOrValueMode>();
                        foreach (YearOrValueMode elc in IginalCurrency)
                        {

                            yearOrValueModes11 = JsonExchangeRateValue(exchangeRate.ExchangeRateValue);
                            //每个年份的汇率
                            YearOrValueMode exchangeRateModel = new();
                            if (yearOrValueModes11.Count is not 0) exchangeRateModel = yearOrValueModes11.FirstOrDefault(p => p.Year.Equals(elc.Year));
                            //本位币
                            decimal value = 0.0M;
                            if (exchangeRateModel is not null)
                            {
                                value = elc.Value * exchangeRateModel.Value;
                            }
                            else
                            {
                                exchangeRateModel = yearOrValueModes11.Last();
                                if (exchangeRateModel is not null) value = elc.Value * exchangeRateModel.Value;
                            }
                            YearOrValueMode yearOrValueMode = new() { Year = elc.Year, Value = value };
                            if (yearOrValueMode is not null) YearOrValueMode.Add(yearOrValueMode);
                        }
                        item.StandardMoney = JsonConvert.DeserializeObject<List<YearOrValueMode>>(JsonConvert.SerializeObject(YearOrValueMode));
                    }
                }
                return Price;
            }
            catch (Exception e)
            {

                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 计算电子料单价录入 ==>根据原币计算
        /// </summary>
        public async Task<List<ElectronicDto>> ToriginalCurrencyCalculate(List<ElectronicDto> Price)
        {
            try
            {
                List<ElectronicDto> list = new List<ElectronicDto>();
                //循环电子BOM单价表单
                foreach (var item in Price)
                {
                    //实体
                    List<YearOrValueMode> YearOrValueMode = new List<YearOrValueMode>();

                    //获取汇率
                    ExchangeRate exchangeRate = await _configExchangeRate.FirstOrDefaultAsync(p => p.ExchangeRateKind.Equals(item.Currency));
                    //本位币实体
                    //计算本位币
                    if (exchangeRate is not null)
                    {
                        List<YearOrValueMode> IginalCurrency = JsonConvert.DeserializeObject<List<YearOrValueMode>>(JsonConvert.SerializeObject(item.IginalCurrency));
                        YearOrValueMode.Clear();
                        List<YearOrValueMode> yearOrValueModes11 = new List<YearOrValueMode>();
                        foreach (YearOrValueMode elc in IginalCurrency)
                        {

                            yearOrValueModes11 = JsonExchangeRateValue(exchangeRate.ExchangeRateValue);
                            //每个年份的汇率
                            YearOrValueMode exchangeRateModel = new();
                            if (yearOrValueModes11.Count is not 0) exchangeRateModel = yearOrValueModes11.FirstOrDefault(p => p.Year.Equals(elc.Year));
                            //本位币
                            decimal value = 0.0M;
                            if (exchangeRateModel is not null)
                            {
                                value = elc.Value * exchangeRateModel.Value;
                            }
                            else
                            {
                                exchangeRateModel = yearOrValueModes11.Last();
                                if (exchangeRateModel is not null) value = elc.Value * exchangeRateModel.Value;
                            }
                            YearOrValueMode yearOrValueMode = new() { Year = elc.Year, Value = value };
                            if (yearOrValueMode is not null) YearOrValueMode.Add(yearOrValueMode);
                        }
                        item.StandardMoney = JsonConvert.DeserializeObject<List<YearOrValueMode>>(JsonConvert.SerializeObject(YearOrValueMode));
                    }
                }
                return Price;
            }
            catch (Exception e)
            {

                throw new UserFriendlyException(e.Message);
            }
        }

        /// <summary>
        /// 计算结构料单价录入 ==>根据年降计算
        /// </summary>
        public async Task<List<ConstructionModel>> StructuralBomCalculateList(List<ConstructionModel> Price)
        {
            try
            {
                List<ConstructionModel> list = new List<ConstructionModel>();
                //循环电子BOM单价表单
                foreach (var item in Price)
                {
                    //实体
                    List<YearOrValueMode> YearOrValueMode = new List<YearOrValueMode>();
                    //循环过的 年将率
                    List<decimal> priceAll = new();
                    //循环 系统单价
                    foreach (YearOrValueMode yearOrValue in item.SystemiginalCurrency)
                    {
                        //手工录入该年份的物料年将率  按 % 百分制来算
                        decimal price = item.InTheRate.Where(m => m.Year.Equals(yearOrValue.Year)).Select(m => m.Value).First();
                        priceAll.Add(price);
                        //原币
                        decimal Value = yearOrValue.Value;
                        priceAll.ForEach(item =>
                        {
                            Value = Value * (1 - item / 100);
                        });
                        //YearOrValueMode yearOrValueMode = new() { Year=yearOrValue.Year, Value=(yearOrValue.Value)*(1-price/100) };
                        YearOrValueMode yearOrValueMode = new() { Year = yearOrValue.Year, Value = Value };
                        if (yearOrValueMode is not null) YearOrValueMode.Add(yearOrValueMode);
                    }
                    //添加计算后的原币
                    item.IginalCurrency = JsonConvert.DeserializeObject<List<YearOrValueMode>>(JsonConvert.SerializeObject(YearOrValueMode));
                    //获取汇率
                    ExchangeRate exchangeRate = await _configExchangeRate.FirstOrDefaultAsync(p => p.ExchangeRateKind.Equals(item.Currency));
                    //本位币实体
                    //计算本位币
                    if (exchangeRate is not null)
                    {
                        List<YearOrValueMode> IginalCurrency = JsonConvert.DeserializeObject<List<YearOrValueMode>>(JsonConvert.SerializeObject(YearOrValueMode));
                        YearOrValueMode.Clear();
                        List<YearOrValueMode> yearOrValueModes11 = new List<YearOrValueMode>();
                        foreach (YearOrValueMode elc in IginalCurrency)
                        {

                            yearOrValueModes11 = JsonExchangeRateValue(exchangeRate.ExchangeRateValue);
                            //每个年份的汇率
                            YearOrValueMode exchangeRateModel = new();
                            if (yearOrValueModes11.Count is not 0) exchangeRateModel = yearOrValueModes11.FirstOrDefault(p => p.Year.Equals(elc.Year));
                            //本位币
                            decimal value = 0.0M;
                            if (exchangeRateModel is not null)
                            {
                                value = elc.Value * exchangeRateModel.Value;
                            }
                            else
                            {
                                exchangeRateModel = yearOrValueModes11.Last();
                                if (exchangeRateModel is not null) value = elc.Value * exchangeRateModel.Value;
                            }
                            YearOrValueMode yearOrValueMode = new() { Year = elc.Year, Value = value };
                            if (yearOrValueMode is not null) YearOrValueMode.Add(yearOrValueMode);
                        }
                        item.StandardMoney = JsonConvert.DeserializeObject<List<YearOrValueMode>>(JsonConvert.SerializeObject(YearOrValueMode));
                    }
                }
                return Price;
            }
            catch (Exception e)
            {

                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 计算结构料单价录入 ==>根据原币计算
        /// </summary>
        public async Task<List<ConstructionModel>> ToriginalCurrencyStructural(List<ConstructionModel> Price)
        {
            try
            {
                List<StructuralMaterialModel> list = new List<StructuralMaterialModel>();
                //循环电子BOM单价表单
                foreach (var item in Price)
                {
                    //实体
                    List<YearOrValueMode> YearOrValueMode = new List<YearOrValueMode>();

                    //获取汇率
                    ExchangeRate exchangeRate = await _configExchangeRate.FirstOrDefaultAsync(p => p.ExchangeRateKind.Equals(item.Currency));
                    //本位币实体
                    //计算本位币
                    if (exchangeRate is not null)
                    {
                        List<YearOrValueMode> IginalCurrency = JsonConvert.DeserializeObject<List<YearOrValueMode>>(JsonConvert.SerializeObject(item.IginalCurrency));
                        YearOrValueMode.Clear();
                        List<YearOrValueMode> yearOrValueModes11 = new List<YearOrValueMode>();
                        foreach (YearOrValueMode elc in IginalCurrency)
                        {

                            yearOrValueModes11 = JsonExchangeRateValue(exchangeRate.ExchangeRateValue);
                            //每个年份的汇率
                            YearOrValueMode exchangeRateModel = new();
                            if (yearOrValueModes11.Count is not 0) exchangeRateModel = yearOrValueModes11.FirstOrDefault(p => p.Year.Equals(elc.Year));
                            //本位币
                            decimal value = 0.0M;
                            if (exchangeRateModel is not null)
                            {
                                value = elc.Value * exchangeRateModel.Value;
                            }
                            else
                            {
                                exchangeRateModel = yearOrValueModes11.Last();
                                if (exchangeRateModel is not null) value = elc.Value * exchangeRateModel.Value;
                            }
                            YearOrValueMode yearOrValueMode = new() { Year = elc.Year, Value = value };
                            if (yearOrValueMode is not null) YearOrValueMode.Add(yearOrValueMode);
                        }
                        item.StandardMoney = JsonConvert.DeserializeObject<List<YearOrValueMode>>(JsonConvert.SerializeObject(YearOrValueMode));
                    }
                }
                return Price;
            }
            catch (Exception e)
            {

                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 电子料单价录入
        /// </summary>
        public async Task ElectronicMaterialEntering(SubmitElectronicDto submitElectronicDto)
        {
            foreach (ElectronicDto item in submitElectronicDto.ElectronicDtoList)
            {
                #region 校验
                if (string.IsNullOrWhiteSpace(item.Currency))
                {
                    throw new FriendlyException("币种不能为空");
                }
                if (item.RebateMoney is null)
                {
                    throw new FriendlyException("物料返利金额不能为空");
                }
                if (string.IsNullOrWhiteSpace(item.ECCNCode))
                {
                    throw new FriendlyException("ECCN不能为空");
                }
                #endregion
                try
                {
                    EnteringElectronic enteringElectronic = await _configEnteringElectronic.FirstOrDefaultAsync(p => p.ProductId.Equals(item.ProductId) && p.AuditFlowId.Equals(submitElectronicDto.AuditFlowId) && p.ElectronicId.Equals(item.ElectronicId));
                    if (enteringElectronic is null)
                    {
                        //添加
                        enteringElectronic = new();
                        enteringElectronic = ObjectMapper.Map<EnteringElectronic>(item);
                        enteringElectronic.AuditFlowId = submitElectronicDto.AuditFlowId;//流程的id
                        enteringElectronic.PeopleId = AbpSession.GetUserId(); //确认人 Id
                        enteringElectronic.IsEntering = true;//确认录入           
                        await _configEnteringElectronic.InsertAsync(enteringElectronic);
                    }
                    else
                    {
                        enteringElectronic.MOQ = item.MOQ;//MOQ
                        enteringElectronic.ElectronicId = item.ElectronicId;//电子BOM表单的Id
                        enteringElectronic.ProductId = item.ProductId; //零件的id
                        enteringElectronic.AuditFlowId = submitElectronicDto.AuditFlowId;//流程的id
                        enteringElectronic.RebateMoney = (decimal)item.RebateMoney;//物料可返利金额
                        enteringElectronic.PeopleId = AbpSession.GetUserId(); //确认人 Id
                        enteringElectronic.AvailableStock = item.AvailableStock;//可用库存
                        enteringElectronic.IsEntering = true;//确认录入          
                        enteringElectronic.Currency = item.Currency;//币种
                        enteringElectronic.ECCNCode = item.ECCNCode;//ECCNCode
                        enteringElectronic.Remark = item.Remark;//备注
                        enteringElectronic.MaterialsUseCount = JsonConvert.SerializeObject(item.MaterialsUseCount);//物料使用量
                        enteringElectronic.InTheRate = JsonConvert.SerializeObject(item.InTheRate);//年将率
                        enteringElectronic.IginalCurrency = JsonConvert.SerializeObject(item.IginalCurrency);//原币
                        enteringElectronic.StandardMoney = JsonConvert.SerializeObject(item.StandardMoney);//本位币
                        await _configEnteringElectronic.UpdateAsync(enteringElectronic);
                    }
                    //删除电子差异表中的数据                 
                    await _configElecBomDifferent.DeleteAsync(p => p.AuditFlowId.Equals(submitElectronicDto.AuditFlowId) && p.ProductId.Equals(item.ProductId) && p.ElectronicId.Equals(item.ElectronicId));
                }
                catch (Exception e)
                {
                    throw new UserFriendlyException(e.Message);
                }
            }
        }
        /// <summary>
        /// 结构件单价录入 有则添加无则修改
        /// </summary>
        /// <returns></returns>
        public async Task SubmitElectronicMaterialEntering(SubmitElectronicDto submitElectronicDto)
        {
            try
            {
                //循环 资源部 填写的 电子BOM 表达那实体类
                foreach (ElectronicDto electronic in submitElectronicDto.ElectronicDtoList)
                {
                    EnteringElectronic enteringElectronic = await _configEnteringElectronic.FirstOrDefaultAsync(p => p.ProductId.Equals(electronic.ProductId) && p.AuditFlowId.Equals(submitElectronicDto.AuditFlowId) && p.ElectronicId.Equals(electronic.ElectronicId));
                    if (enteringElectronic is not null)
                    {
                        enteringElectronic.IsSubmit = true;
                        await _configEnteringElectronic.UpdateAsync(enteringElectronic);
                    }
                }
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 结构件单价录入提交 有则添加无则修改
        /// </summary>
        /// <returns></returns>
        public async Task StructuralMemberEntering(StructuralMemberEnteringModel structuralMemberEntering)
        {
            foreach (StructuralMaterialModel item in structuralMemberEntering.StructuralMaterialEntering)
            {
                #region 校验
                if (string.IsNullOrWhiteSpace(item.Currency))
                {
                    throw new FriendlyException("币种不能为空");
                }
                if (string.IsNullOrWhiteSpace(item.ECCNCode))
                {
                    throw new FriendlyException("ECCN不能为空");
                }
                #endregion
                // _configStructureElectronic
                try
                {
                    StructureElectronic structureElectronic = await _configStructureElectronic.FirstOrDefaultAsync(p => p.ProductId.Equals(item.ProductId) && p.AuditFlowId.Equals(structuralMemberEntering.AuditFlowId) && p.StructureId.Equals(item.StructureId));
                    if (structureElectronic is null)
                    {
                        //添加
                        structureElectronic = new();
                        structureElectronic = ObjectMapper.Map<StructureElectronic>(item);
                        structureElectronic.AuditFlowId = structuralMemberEntering.AuditFlowId;//流程的id
                        structureElectronic.PeopleId = AbpSession.GetUserId(); //确认人 Id
                        structureElectronic.IsEntering = true; //确认录入
                        await _configStructureElectronic.InsertAsync(structureElectronic);
                    }
                    else
                    {
                        structureElectronic.RebateMoney = item.RebateMoney;//物料返利金额
                        structureElectronic.MOQ = item.MOQ;//MOQ
                        structureElectronic.PeopleId = AbpSession.GetUserId(); //确认人 Id
                        structureElectronic.Currency = item.Currency;//币种
                        structureElectronic.AvailableStock = item.AvailableStock;//可用库存
                        structureElectronic.IsEntering = true; //确认录入
                        structureElectronic.ECCNCode = item.ECCNCode;//ECCN
                        structureElectronic.Remark = item.Remark; //备注
                        structureElectronic.Sop = JsonConvert.SerializeObject(item.Sop);//Sop
                        structureElectronic.IginalCurrency = JsonConvert.SerializeObject(item.IginalCurrency);//原币
                        structureElectronic.StandardMoney = JsonConvert.SerializeObject(item.StandardMoney);//本位币
                        structureElectronic.MaterialsUseCount = JsonConvert.SerializeObject(item.MaterialsUseCount);//项目物料的使用量
                        structureElectronic.SystemiginalCurrency = JsonConvert.SerializeObject(item.SystemiginalCurrency);//系统单价（原币）
                        structureElectronic.InTheRate = JsonConvert.SerializeObject(item.InTheRate);//项目物料的年降率
                        await _configStructureElectronic.UpdateAsync(structureElectronic);
                    }
                    //删除结构差异表中的数据  
                    await _configStructBomDifferent.DeleteAsync(p => p.AuditFlowId.Equals(structuralMemberEntering.AuditFlowId) && p.ProductId.Equals(item.ProductId) && p.StructureId.Equals(item.StructureId));
                }
                catch (Exception e)
                {
                    throw new UserFriendlyException(e.Message);
                }
            }
        }
        /// <summary>
        /// 结构件单价提交 有则添加无则修改
        /// </summary>
        /// <returns></returns>
        public async Task SubmitStructuralMemberEntering(StructuralMemberEnteringModel structuralMemberEntering)
        {
            try
            {
                foreach (StructuralMaterialModel item in structuralMemberEntering.StructuralMaterialEntering)
                {
                    // _configStructureElectronic
                    StructureElectronic structureElectronic = await _configStructureElectronic.FirstOrDefaultAsync(p => p.ProductId.Equals(item.ProductId) && p.AuditFlowId.Equals(structuralMemberEntering.AuditFlowId) && p.StructureId.Equals(item.StructureId));
                    if (structureElectronic is not null)
                    {
                        //添加
                        structureElectronic.IsSubmit = true;
                        await _configStructureElectronic.UpdateAsync(structureElectronic);
                    }

                }
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 将json 转成  List YearOrValueMode>
        /// </summary>
        internal static List<YearOrValueMode> JsonExchangeRateValue(string price)
        {
            return JsonConvert.DeserializeObject<List<YearOrValueMode>>(price);
        }
    }
}
