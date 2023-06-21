using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Abp.UI;
using Finance.Audit;
using Finance.Dto;
using Finance.EngineeringDepartment;
using Finance.Entering;
using Finance.FinanceMaintain;
using Finance.Infrastructure;
using Finance.MakeOffers.AnalyseBoard.Method;
using Finance.MakeOffers.AnalyseBoard.Model;
using Finance.Nre;
using Finance.NrePricing;
using Finance.NrePricing.Dto;
using Finance.NrePricing.Method;
using Finance.NrePricing.Model;
using Finance.PriceEval;
using Finance.ProductDevelopment;
using Finance.PropertyDepartment.Entering.Dto;
using Finance.PropertyDepartment.Entering.Method;
using Finance.PropertyDepartment.Entering.Model;
using Finance.PropertyDepartment.UnitPriceLibrary.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using Newtonsoft.Json;
using NPOI.HPSF;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using NPOI.XSSF.Streaming.Values;
using Org.BouncyCastle.Utilities;
using Spire.Pdf.Exporting.XPS.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Finance.MakeOffers.AnalyseBoard.Method.AnalysisBoardMethod;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace Finance.NerPricing
{
    /// <summary>
    /// Nre核价api
    /// </summary>
    public class NrePricingAppService : FinanceAppServiceBase, INrePricingAppService
    {
        /// <summary>
        /// 模组数量
        /// </summary>
        private readonly IRepository<ModelCount, long> _resourceModelCount;
        private readonly ElectronicStructuralMethod _resourceElectronicStructuralMethod;
        /// <summary>
        /// Nre 项目管理部 手板件实体类
        /// </summary>
        private readonly IRepository<HandPieceCost, long> _resourceHandPieceCost;
        /// <summary>
        /// Nre 项目管理部 其他费用实体类
        /// </summary>
        private readonly IRepository<RestsCost, long> _resourceRestsCost;
        /// <summary>
        /// Nre 项目管理部 差旅费实体类
        /// </summary>
        private readonly IRepository<TravelExpense, long> _resourceTravelExpense;
        /// <summary> 
        /// Nre 资源部 模具清单实体类
        /// </summary>
        private readonly IRepository<MouldInventory, long> _resourceMouldInventory;
        /// <summary> 
        /// Nre 产品部-电子工程师 实验费 实体类
        /// </summary>
        private readonly IRepository<LaboratoryFee, long> _resourceLaboratoryFee;
        /// <summary>
        /// Nre 方法 类
        /// </summary>
        private readonly NrePricingMethod _resourceNrePricingMethod;
        /// <summary>
        /// Nre 品保录入 实验项目 实体类
        /// </summary>
        private readonly IRepository<QADepartmentTest, long> _resourceQADepartmentTest;
        /// <summary>
        ///Nre 品保录入  项目制程QC量检具表  实体类
        /// </summary>
        private readonly IRepository<QADepartmentQC, long> _resourceQADepartmentQC;
        /// <summary>
        ///Nre 营销部录入  总价表  实体类
        /// </summary>
        private readonly IRepository<InitialResourcesManagement, long> _resourceInitialResourcesManagement;
        /// <summary>
        /// 设备部分表
        /// </summary>
        private readonly IRepository<EquipmentInfo, long> _resourceEquipmentInfo;
        /// <summary>
        /// 追溯部分表(硬件及软件开发费用)
        /// </summary>
        private readonly IRepository<TraceInfo, long> _resourceTraceInfo;
        /// <summary>
        /// 工时工序静态字段表
        /// </summary>
        private readonly IRepository<WorkingHoursInfo, long> _resourceWorkingHoursInfo;
        /// <summary>
        /// 字典明细表
        /// </summary>
        private readonly IRepository<FinanceDictionaryDetail, string> _financeDictionaryDetailRepository;
        /// <summary>
        /// 核价需求录入表
        /// </summary>
        private readonly IRepository<PriceEvaluation, long> _resourcePriceEvaluation;
        /// <summary>
        /// 流程流转服务
        /// </summary>
        private readonly AuditFlowAppService _flowAppService;
        /// <summary>
        ///  Nre  零件是否全部录入 依据实体类
        /// </summary>
        private readonly IRepository<NreIsSubmit, long> _resourceNreIsSubmit;
        /// <summary>
        /// 模组数量年份
        /// </summary>
        private readonly IRepository<ModelCountYear, long> _resourceModelCountYear;
        /// <summary>
        ///  财务维护 汇率表
        /// </summary>
        private static IRepository<ExchangeRate, long> _configExchangeRate;
        /// <summary>
        /// 结构BOM两次上传差异化表
        /// </summary>
        private static IRepository<StructBomDifferent, long> _configStructBomDifferent;
        /// <summary>
        /// 
        /// </summary>
        public NrePricingAppService(
            IRepository<ModelCount, long> modelCount,
            ElectronicStructuralMethod electronicStructuralMethod,
            IRepository<HandPieceCost, long> handPieceCost,
            IRepository<RestsCost, long> restsCost,
            IRepository<TravelExpense, long> travelExpense,
            IRepository<MouldInventory, long> mouldInventory,
            IRepository<LaboratoryFee, long> laboratoryFee,
            NrePricingMethod nrePricingMethod,
            IRepository<QADepartmentTest, long> qADepartmentTest,
            IRepository<QADepartmentQC, long> qADepartmentQC,
            IRepository<InitialResourcesManagement, long> initialResourcesManagement,
            IRepository<EquipmentInfo, long> equipmentInfo,
            IRepository<TraceInfo, long> traceInfo,
            IRepository<WorkingHoursInfo, long> workingHoursInfo,
            IRepository<FinanceDictionaryDetail, string> financeDictionaryDetail,
            IRepository<PriceEvaluation, long> priceEvaluation,
            AuditFlowAppService flowAppService,
            IRepository<NreIsSubmit, long> nreIsSubmit,
            IRepository<ModelCountYear, long> resourceModelCountYear,
            IRepository<ExchangeRate, long> exchangeRate,
            IRepository<StructBomDifferent, long> structBomDifferent)
        {
            _resourceModelCount = modelCount;
            _resourceElectronicStructuralMethod = electronicStructuralMethod;
            _resourceHandPieceCost = handPieceCost;
            _resourceRestsCost = restsCost;
            _resourceTravelExpense = travelExpense;
            _resourceMouldInventory = mouldInventory;
            _resourceLaboratoryFee = laboratoryFee;
            _resourceNrePricingMethod = nrePricingMethod;
            _resourceQADepartmentTest = qADepartmentTest;
            _resourceQADepartmentQC = qADepartmentQC;
            _resourceInitialResourcesManagement = initialResourcesManagement;
            _resourceEquipmentInfo = equipmentInfo;
            _resourceTraceInfo = traceInfo;
            _resourceWorkingHoursInfo = workingHoursInfo;
            _financeDictionaryDetailRepository = financeDictionaryDetail;
            _resourcePriceEvaluation = priceEvaluation;
            _flowAppService = flowAppService;
            _resourceNreIsSubmit = nreIsSubmit;
            _resourceModelCountYear = resourceModelCountYear;
            _configExchangeRate = exchangeRate;
            _configStructBomDifferent = structBomDifferent;
        }
        /// <summary>
        /// 获取 零件
        /// </summary>
        /// <param name="Id">流程id(long类型)</param>
        /// <returns></returns>
        public async Task<List<PartModel>> GetPart(long Id)
        {
            //根据流程表主键取模组数量
            List<ModelCount> modelCount = await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            //总共的零件
            List<PartModel> partList = _resourceElectronicStructuralMethod.PartDtoList(modelCount);
            return partList;
        }
        /// <summary>
        /// 项目管理部录入
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task PostProjectManagement(ProjectManagementDto price)
        {
            //录入手板件费用
            foreach (ProjectManagementModel item in price.projectManagements)
            {
                //判断 该零件 是否已经录入
                List<NreIsSubmit> nreIsSubmits = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.ProductId.Equals(item.ProductId) && p.EnumSole.Equals(NreIsSubmitDto.ProjectManagement.ToString()));
                if (nreIsSubmits.Count is not 0)
                {
                    throw new FriendlyException(item.ProductId + ":该零件id已经提交过了");
                }
                try
                {
                    List<HandPieceCost> handPieceCosts = ObjectMapper.Map<List<HandPieceCost>>(item.HandPieceCost);
                    //删除原数据
                    await _resourceHandPieceCost.DeleteAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.ProductId.Equals(item.ProductId));
                    foreach (HandPieceCost handPieceCost in handPieceCosts)
                    {
                        handPieceCost.AuditFlowId = price.AuditFlowId;
                        handPieceCost.ProductId = item.ProductId;
                        await _resourceHandPieceCost.InsertOrUpdateAsync(handPieceCost); //录入手板费用
                    }
                    List<RestsCost> restsCosts = ObjectMapper.Map<List<RestsCost>>(item.RestsCost);
                    //删除原数据
                    await _resourceRestsCost.DeleteAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.ProductId.Equals(item.ProductId));
                    foreach (RestsCost restsCost in restsCosts)
                    {
                        restsCost.AuditFlowId = price.AuditFlowId;
                        restsCost.ProductId = item.ProductId;
                        await _resourceRestsCost.InsertOrUpdateAsync(restsCost);//录入其他费用
                    }
                    List<TravelExpense> travelExpenses = ObjectMapper.Map<List<TravelExpense>>(item.TravelExpense);
                    //删除原数据
                    await _resourceTravelExpense.DeleteAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.ProductId.Equals(item.ProductId));
                    foreach (TravelExpense travel in travelExpenses)
                    {
                        travel.AuditFlowId = price.AuditFlowId;
                        travel.ProductId = item.ProductId;
                        await _resourceTravelExpense.InsertOrUpdateAsync(travel);//录入差旅费
                    }
                    #region 录入完成之后
                    await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = price.AuditFlowId, ProductId = item.ProductId, EnumSole = NreIsSubmitDto.ProjectManagement.ToString() });
                    #endregion
                }
                catch (Exception e)
                {
                    throw new UserFriendlyException(e.Message);
                }
            }
        }
        /// <summary>
        /// 项目管理部录入(单个零件)
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task PostProjectManagementSingle(ProjectManagementDtoSingle price)
        {
            ProjectManagementModel projectManagementModel = new();
            projectManagementModel = price.projectManagement;
            //判断 该零件 是否已经录入
            List<NreIsSubmit> nreIsSubmits = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.ProductId.Equals(projectManagementModel.ProductId) && p.EnumSole.Equals(NreIsSubmitDto.ProjectManagement.ToString()));
            if (nreIsSubmits.Count is not 0)
            {

                throw new FriendlyException("该零件已经提交过了");
            }
            try
            {

                List<HandPieceCost> handPieceCosts = ObjectMapper.Map<List<HandPieceCost>>(projectManagementModel.HandPieceCost);
                //删除原数据
                await _resourceHandPieceCost.DeleteAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.ProductId.Equals(projectManagementModel.ProductId));
                foreach (HandPieceCost handPieceCost in handPieceCosts)
                {
                    handPieceCost.AuditFlowId = price.AuditFlowId;
                    handPieceCost.ProductId = projectManagementModel.ProductId;
                    await _resourceHandPieceCost.InsertOrUpdateAsync(handPieceCost); //录入手板费用
                }
                List<RestsCost> restsCosts = ObjectMapper.Map<List<RestsCost>>(projectManagementModel.RestsCost);
                //删除原数据
                await _resourceRestsCost.DeleteAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.ProductId.Equals(projectManagementModel.ProductId));
                foreach (RestsCost restsCost in restsCosts)
                {
                    restsCost.AuditFlowId = price.AuditFlowId;
                    restsCost.ProductId = projectManagementModel.ProductId;
                    await _resourceRestsCost.InsertOrUpdateAsync(restsCost);//录入其他费用
                }
                List<TravelExpense> travelExpenses = ObjectMapper.Map<List<TravelExpense>>(projectManagementModel.TravelExpense);
                //删除原数据
                await _resourceTravelExpense.DeleteAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.ProductId.Equals(projectManagementModel.ProductId));
                foreach (TravelExpense travel in travelExpenses)
                {
                    travel.AuditFlowId = price.AuditFlowId;
                    travel.ProductId = projectManagementModel.ProductId;
                    await _resourceTravelExpense.InsertOrUpdateAsync(travel);//录入差旅费
                }
                #region 录入完成之后
                await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = price.AuditFlowId, ProductId = projectManagementModel.ProductId, EnumSole = NreIsSubmitDto.ProjectManagement.ToString() });
                #endregion
                if (await this.GetProjectManagement(price.AuditFlowId))
                {
                    if (AbpSession.UserId is null)
                    {
                        throw new FriendlyException("请先登录");
                    }
                    ReturnDto retDto = await _flowAppService.UpdateAuditFlowInfo(new Audit.Dto.AuditFlowDetailDto()
                    {
                        AuditFlowId = price.AuditFlowId,
                        ProcessIdentifier = AuditFlowConsts.AF_NreInputOther,
                        UserId = AbpSession.UserId.Value,
                        Opinion = OPINIONTYPE.Submit_Agreee
                    });
                }
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 项目管理部录入  判断是否全部提交完毕  true 所有零件已录完   false  没有录完
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetProjectManagement(long Id)
        {
            //获取 总共的零件
            List<PartModel> partModels = await GetPart(Id);
            int AllCount = partModels.Count();
            //获取 已经提交的零件
            int Count = await _resourceNreIsSubmit.CountAsync(p => p.AuditFlowId.Equals(Id) && p.EnumSole.Equals(NreIsSubmitDto.ProjectManagement.ToString())) + 1;
            return AllCount == Count;
        }
        /// <summary>
        /// 项目管理部录入  退回重置状态
        /// </summary>
        /// <returns></returns>
        public async Task GetProjectManagementConfigurationState(long Id)
        {
            List<NreIsSubmit> nreAre = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.EnumSole.Equals(NreIsSubmitDto.ProjectManagement.ToString()));
            foreach (NreIsSubmit item in nreAre)
            {
                _resourceNreIsSubmit.HardDelete(item);
            }
        }
        /// <summary>
        /// Nre项目管理部 获取版本录入过的值
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<List<ProjectManagementModel>> GetReturnProjectManagement(long Id)
        {
            try
            {
                List<ProjectManagementModel> projectManagementModels = new();
                //所有的零件
                List<PartModel> partModels = new();
                partModels = await GetPart(Id);
                foreach (PartModel partModel in partModels)
                {
                    ProjectManagementModel projectManagementModel = new();
                    projectManagementModel.ProductId = partModel.ProductId;

                    List<HandPieceCostModel> handPieceCostModels = new();
                    List<HandPieceCost> handPieceCosts = await _resourceHandPieceCost.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(partModel.ProductId));
                    if (handPieceCosts is not null) handPieceCostModels = ObjectMapper.Map<List<HandPieceCostModel>>(handPieceCosts);
                    projectManagementModel.HandPieceCost = new();
                    projectManagementModel.HandPieceCost = handPieceCostModels;//手板费用

                    List<RestsCostModel> restsCostModels = new();
                    List<RestsCost> restsCosts = await _resourceRestsCost.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(partModel.ProductId));
                    if (restsCosts is not null) restsCostModels = ObjectMapper.Map<List<RestsCostModel>>(restsCosts);
                    projectManagementModel.RestsCost = new();
                    projectManagementModel.RestsCost = restsCostModels;//其他费用

                    List<TravelExpenseModel> travelExpenseModels = new();
                    List<TravelExpense> travelExpenses = await _resourceTravelExpense.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(partModel.ProductId));
                    if (travelExpenses is not null) travelExpenseModels = ObjectMapper.Map<List<TravelExpenseModel>>(travelExpenses);
                    projectManagementModel.TravelExpense = new();
                    projectManagementModel.TravelExpense = travelExpenseModels;//差旅费
                    int Count = await _resourceNreIsSubmit.CountAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(partModel.ProductId) && p.EnumSole.Equals(NreIsSubmitDto.ProjectManagement.ToString()));
                    projectManagementModel.IsSubmit = Count > 0;
                    projectManagementModels.Add(projectManagementModel);
                }
                return projectManagementModels;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// Nre项目管理部 获取版本录入过的值(单个零件)
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<ProjectManagementModel> GetReturnProjectManagementSingle(long auditFlowId, long productId)
        {
            try
            {
                List<ProjectManagementModel> projectManagementModels = new();
                //所有的零件
                List<PartModel> partModels = new();
                partModels = await GetPart(auditFlowId);
                partModels = partModels.Where(p => p.ProductId.Equals(productId)).ToList();
                foreach (PartModel partModel in partModels)
                {
                    ProjectManagementModel projectManagementModel = new();
                    projectManagementModel.ProductId = partModel.ProductId;

                    List<HandPieceCostModel> handPieceCostModels = new();
                    List<HandPieceCost> handPieceCosts = await _resourceHandPieceCost.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.ProductId.Equals(partModel.ProductId));
                    if (handPieceCosts is not null) handPieceCostModels = ObjectMapper.Map<List<HandPieceCostModel>>(handPieceCosts);
                    projectManagementModel.HandPieceCost = new();
                    projectManagementModel.HandPieceCost = handPieceCostModels;//手板费用

                    List<RestsCostModel> restsCostModels = new();
                    List<RestsCost> restsCosts = await _resourceRestsCost.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.ProductId.Equals(partModel.ProductId));
                    if (restsCosts is not null) restsCostModels = ObjectMapper.Map<List<RestsCostModel>>(restsCosts);
                    projectManagementModel.RestsCost = new();
                    projectManagementModel.RestsCost = restsCostModels;//其他费用

                    List<TravelExpenseModel> travelExpenseModels = new();
                    List<TravelExpense> travelExpenses = await _resourceTravelExpense.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.ProductId.Equals(partModel.ProductId));
                    if (travelExpenses is not null) travelExpenseModels = ObjectMapper.Map<List<TravelExpenseModel>>(travelExpenses);
                    projectManagementModel.TravelExpense = new();
                    projectManagementModel.TravelExpense = travelExpenseModels;//差旅费
                    //判断 该零件 是否已经录入
                    int Count = await _resourceNreIsSubmit.CountAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.ProductId.Equals(partModel.ProductId) && p.EnumSole.Equals(NreIsSubmitDto.ProjectManagement.ToString()));
                    projectManagementModel.IsSubmit = Count > 0;
                    projectManagementModels.Add(projectManagementModel);
                }
                return projectManagementModels.FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 资源部录入初始值
        /// </summary>
        /// <param name="Id">流程id(long类型)</param>
        /// <returns></returns>
        public async Task<InitialResourcesManagementDto> GetInitialResourcesManagement(long Id)
        {
            InitialResourcesManagementDto initialResourcesManagementDto = new();
            List<PartModel> partModels = await GetPart(Id);// 获取 零件
            initialResourcesManagementDto.partModels = partModels;//零件
            List<MouldInventoryPartModel> mouldInventoryPartModels = new();// Nre核价 带 零件 id 的模具清单 模型  
            //循环每一个零件
            foreach (PartModel part in partModels)
            {

                MouldInventoryPartModel mouldInventoryPartModel = new();//  Nre核价 模组清单模型
                mouldInventoryPartModel.ProductId = part.ProductId;//零件的 Id
                //判断 该零件 是否已经录入
                int Count = await _resourceNreIsSubmit.CountAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(part.ProductId) && p.EnumSole.Equals(NreIsSubmitDto.ResourcesManagement.ToString()));
                mouldInventoryPartModel.IsSubmit = Count > 0;
                //删除的结构BOMid
                List<long> longs = new();
                List<StructBomDifferent> structBomDifferents = await _configStructBomDifferent.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(part.ProductId));
                if (structBomDifferents.Count is not 0)//有差异
                {
                    foreach (StructBomDifferent structBom in structBomDifferents)
                    {
                        //判断差异类型
                        if (structBom.ModifyTypeValue.Equals(MODIFYTYPE.DELNEWDATA))//删除
                        {
                            //删除存在数据库里的数据和返回数据中的数据即可
                            //1.删除数据库中的数据
                            await _resourceMouldInventory.HardDeleteAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(part.ProductId) && p.StructuralId.Equals(structBom.StructureId));
                            longs.Add(structBom.StructureId);
                        }
                    }
                }
                //获取初始值的时候如果数据库里有,直接拿数据库中的
                List<MouldInventory> mouldInventory = await _resourceMouldInventory.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(part.ProductId));
                mouldInventory = (from a in mouldInventory
                                  join b in longs on a.StructuralId! equals b
                                  select a).ToList();
                mouldInventoryPartModel.MouldInventoryModels = await _resourceNrePricingMethod.MouldInventoryModels(Id, part.ProductId);//传流程id和零件号的id
                foreach (MouldInventoryModel item in mouldInventoryPartModel.MouldInventoryModels)
                {
                    MouldInventory mouldInventory1 = mouldInventory.FirstOrDefault(p => p.StructuralId.Equals(item.StructuralId));
                    if (mouldInventory1 is not null)
                    {
                        item.Id = mouldInventory1.Id;
                        item.MoldCavityCount = mouldInventory1.MoldCavityCount;//摸穴数
                        item.ModelNumber = mouldInventory1.ModelNumber;//摸次数
                        item.Count = mouldInventory1.Count;//数量
                        item.UnitPrice = mouldInventory1.UnitPrice;//单价
                        item.Cost = mouldInventory1.Cost;//费用
                    }
                }
                mouldInventoryPartModels.Add(mouldInventoryPartModel);
            }
            initialResourcesManagementDto.mouldInventoryModels = mouldInventoryPartModels;
            return initialResourcesManagementDto;
        }
        /// <summary>
        /// 资源部录入初始值(单个零件)
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<MouldInventoryPartModel> GetInitialResourcesManagementSingle(long auditFlowId, long productId)
        {
            List<PartModel> partModels = await GetPart(auditFlowId);// 获取 零件
            partModels = partModels.Where(p => p.ProductId.Equals(productId)).ToList();
            List<MouldInventoryPartModel> mouldInventoryPartModels = new();// Nre核价 带 零件 id 的模具清单 模型  
            //循环每一个零件
            foreach (PartModel part in partModels)
            {
                MouldInventoryPartModel mouldInventoryPartModel = new();//  Nre核价 模组清单模型
                mouldInventoryPartModel.ProductId = part.ProductId;//零件的 Id
                //判断 该零件 是否已经录入
                int Count = await _resourceNreIsSubmit.CountAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.ProductId.Equals(part.ProductId) && p.EnumSole.Equals(NreIsSubmitDto.ResourcesManagement.ToString()));
                mouldInventoryPartModel.IsSubmit = Count > 0;
                //获取初始值的时候如果数据库里有,直接拿数据库中的

                //删除的结构BOMid
                List<long> longs = new();
                List<StructBomDifferent> structBomDifferents = await _configStructBomDifferent.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.ProductId.Equals(part.ProductId));
                if (structBomDifferents.Count is not 0)//有差异
                {
                    foreach (StructBomDifferent structBom in structBomDifferents)
                    {
                        //判断差异类型
                        if (structBom.ModifyTypeValue.Equals(MODIFYTYPE.DELNEWDATA))//删除
                        {
                            //删除存在数据库里的数据和返回数据中的数据即可
                            //1.删除数据库中的数据
                            await _resourceMouldInventory.HardDeleteAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.ProductId.Equals(part.ProductId) && p.StructuralId.Equals(structBom.StructureId));
                            longs.Add(structBom.StructureId);
                        }
                    }
                }
                //获取初始值的时候如果数据库里有,直接拿数据库中的
                List<MouldInventory> mouldInventory = await _resourceMouldInventory.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.ProductId.Equals(part.ProductId));
                List<MouldInventory> mouldInventoryEquals = (from a in mouldInventory
                                                             join b in longs on a.StructuralId equals b
                                                             select a).ToList();//相等的

                mouldInventory = mouldInventory.Except(mouldInventoryEquals).Distinct().ToList();//差集
                mouldInventoryPartModel.MouldInventoryModels = await _resourceNrePricingMethod.MouldInventoryModels(auditFlowId, part.ProductId);//传流程id和零件号的id
                foreach (MouldInventoryModel item in mouldInventoryPartModel.MouldInventoryModels)
                {
                    MouldInventory mouldInventory1 = mouldInventory.FirstOrDefault(p => p.StructuralId.Equals(item.StructuralId));
                    if (mouldInventory1 is not null)
                    {
                        item.Id = mouldInventory1.Id;
                        item.MoldCavityCount = mouldInventory1.MoldCavityCount;//摸穴数
                        item.ModelNumber = mouldInventory1.ModelNumber;//摸次数
                        item.Count = mouldInventory1.Count;//数量
                        item.UnitPrice = mouldInventory1.UnitPrice;//单价
                        item.Cost = mouldInventory1.Cost;//费用
                    }
                }
                mouldInventoryPartModels.Add(mouldInventoryPartModel);
            }
            return mouldInventoryPartModels.FirstOrDefault();
        }
        /// <summary>
        /// 计算 模具清单的 数量以及费用
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        public async Task<List<ResourcesManagementModel>> PostCalculateMouldInventory(ResourcesManagementDto resources)
        {
            foreach (ResourcesManagementModel item in resources.ResourcesManagementModels)
            {
                foreach (MouldInventoryModel mould in item.MouldInventory)
                {
                    mould.Count = await _resourceNrePricingMethod.CalculateCount(item.ProductId, mould);//计算数量
                    double count = mould.Count;
                    mould.Cost = count != 0 ? (decimal)count : 0 * mould.UnitPrice;//计算费用
                }

            }
            return resources.ResourcesManagementModels;
        }
        /// <summary>
        /// 计算 模具清单的 数量以及费用(单个零件)
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        public async Task<ResourcesManagementModel> PostCalculateMouldInventorySingle(ResourcesManagementModel resources)
        {
            foreach (MouldInventoryModel mould in resources.MouldInventory)
            {
                mould.Count = await _resourceNrePricingMethod.CalculateCount(resources.ProductId, mould);//计算数量
                double count = mould.Count;
                mould.Cost = count != 0 ? (decimal)count : 0 * mould.UnitPrice;//计算费用
            }
            return resources;
        }
        /// <summary>
        /// 资源部录入
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public async Task PostResourcesManagement(ResourcesManagementDto price)
        {

            foreach (ResourcesManagementModel item in price.ResourcesManagementModels)
            {
                List<MouldInventory> MouldInventorys = ObjectMapper.Map<List<MouldInventory>>(item.MouldInventory);
                //判断 该零件 是否已经录入
                List<NreIsSubmit> nreIsSubmits = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.ProductId.Equals(item.ProductId) && p.EnumSole.Equals(NreIsSubmitDto.ResourcesManagement.ToString()));
                if (nreIsSubmits.Count is not 0)
                {

                    throw new FriendlyException(item.ProductId + ":该零件id已经提交过了");
                }
                //删除原数据
                await _resourceMouldInventory.DeleteAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.ProductId.Equals(item.ProductId));
                foreach (MouldInventory MouldInventory in MouldInventorys)
                {
                    MouldInventory.AuditFlowId = price.AuditFlowId;
                    MouldInventory.ProductId = item.ProductId;
                    await _resourceMouldInventory.InsertOrUpdateAsync(MouldInventory);//录入模具清单
                }
                #region 录入完成之后
                await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = price.AuditFlowId, ProductId = item.ProductId, EnumSole = NreIsSubmitDto.ResourcesManagement.ToString() });
                #endregion
            }
        }
        /// <summary>
        /// 资源部录入(单个零件)
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public async Task PostResourcesManagementSingle(ResourcesManagementSingleDto price)
        {

            ResourcesManagementModel resourcesManagementModel = new();
            resourcesManagementModel = price.ResourcesManagementModels;
            //判断 该零件 是否已经录入
            List<NreIsSubmit> nreIsSubmits = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.ProductId.Equals(resourcesManagementModel.ProductId) && p.EnumSole.Equals(NreIsSubmitDto.ResourcesManagement.ToString()));
            if (nreIsSubmits.Count is not 0)
            {

                throw new FriendlyException("该零件id已经提交过了");
            }
            List<MouldInventory> MouldInventorys = ObjectMapper.Map<List<MouldInventory>>(resourcesManagementModel.MouldInventory);
            //删除原数据
            await _resourceMouldInventory.DeleteAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.ProductId.Equals(resourcesManagementModel.ProductId));
            foreach (MouldInventory MouldInventory in MouldInventorys)
            {
                MouldInventory.AuditFlowId = price.AuditFlowId;
                MouldInventory.ProductId = resourcesManagementModel.ProductId;
                await _resourceMouldInventory.InsertOrUpdateAsync(MouldInventory);//录入模具清单
            }

            #region 录入完成之后
            await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = price.AuditFlowId, ProductId = resourcesManagementModel.ProductId, EnumSole = NreIsSubmitDto.ResourcesManagement.ToString() });
            #endregion
            if (await this.GetResourcesManagement(price.AuditFlowId))
            {
                if (AbpSession.UserId is null)
                {
                    throw new FriendlyException("请先登录");
                }
                ReturnDto retDto = await _flowAppService.UpdateAuditFlowInfo(new Audit.Dto.AuditFlowDetailDto()
                {
                    AuditFlowId = price.AuditFlowId,
                    ProcessIdentifier = AuditFlowConsts.AF_NreInputMould,
                    UserId = AbpSession.UserId.Value,
                    Opinion = OPINIONTYPE.Submit_Agreee
                });
            }

        }
        /// <summary>
        /// 资源部录入  判断是否全部提交完毕  true 所有零件已录完   false  没有录完
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetResourcesManagement(long Id)
        {
            //获取 总共的零件
            List<PartModel> partModels = await GetPart(Id);
            int AllCount = partModels.Count();
            //获取 已经提交的零件
            int Count = await _resourceNreIsSubmit.CountAsync(p => p.AuditFlowId.Equals(Id) && p.EnumSole.Equals(NreIsSubmitDto.ResourcesManagement.ToString())) + 1;
            return AllCount == Count;
        }
        /// <summary>
        /// 资源部录入  退回重置状态
        /// </summary>
        /// <returns></returns>
        public async Task GetResourcesManagementConfigurationState(long Id)
        {
            List<NreIsSubmit> nreAre = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.EnumSole.Equals(NreIsSubmitDto.ResourcesManagement.ToString()));
            foreach (NreIsSubmit item in nreAre)
            {
                _resourceNreIsSubmit.HardDelete(item);
            }
        }
        /// <summary>
        /// 产品部-电子工程师 录入
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task PostProductDepartment(ProductDepartmentDto price)
        {

            foreach (ProductDepartmentModel Product in price.ProductDepartmentModels)
            {
                List<LaboratoryFee> laboratoryFees = ObjectMapper.Map<List<LaboratoryFee>>(Product.laboratoryFeeModels);
                //判断 该零件 是否已经录入
                List<NreIsSubmit> nreIsSubmits = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.ProductId.Equals(Product.ProductId) && p.EnumSole.Equals(NreIsSubmitDto.ProductDepartment.ToString()));
                if (nreIsSubmits.Count is not 0)
                {

                    throw new FriendlyException(Product.ProductId + "该零件id已经提交过了");
                }
                //删除原数据
                await _resourceLaboratoryFee.DeleteAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.ProductId.Equals(Product.ProductId));
                foreach (LaboratoryFee laboratoryFee in laboratoryFees)
                {
                    laboratoryFee.AuditFlowId = price.AuditFlowId;
                    laboratoryFee.ProductId = Product.ProductId;
                    await _resourceLaboratoryFee.InsertOrUpdateAsync(laboratoryFee);
                }
                #region 录入完成之后
                await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = price.AuditFlowId, ProductId = Product.ProductId, EnumSole = NreIsSubmitDto.ProductDepartment.ToString() });
                #endregion
            }
        }
        /// <summary>
        ///产品部-电子工程师录入  判断是否全部提交完毕  true 所有零件已录完   false  没有录完
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetProductDepartment(long Id)
        {
            //获取 总共的零件
            List<PartModel> partModels = await GetPart(Id);
            int AllCount = partModels.Count();
            //获取 已经提交的零件
            int Count = await _resourceNreIsSubmit.CountAsync(p => p.AuditFlowId.Equals(Id) && p.EnumSole.Equals(NreIsSubmitDto.ProductDepartment.ToString())) + 1;
            return AllCount == Count;
        }
        /// <summary>
        /// 产品部-电子工程师入  退回重置状态
        /// </summary>
        /// <returns></returns>
        public async Task GetProductDepartmentConfigurationState(long Id)
        {
            List<NreIsSubmit> nreAre = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.EnumSole.Equals(NreIsSubmitDto.ProductDepartment.ToString()));
            foreach (NreIsSubmit item in nreAre)
            {
                _resourceNreIsSubmit.HardDelete(item);
            }
        }
        /// <summary>
        /// 产品部-电子工程师 录入(单个零件)
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task PostProductDepartmentSingle(ProductDepartmentSingleDto price)
        {
            ProductDepartmentModel productDepartmentModel = new();
            productDepartmentModel = price.ProductDepartmentModels;
            //判断 该零件 是否已经录入
            List<NreIsSubmit> nreIsSubmits = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.ProductId.Equals(productDepartmentModel.ProductId) && p.EnumSole.Equals(NreIsSubmitDto.ProductDepartment.ToString()));
            if (nreIsSubmits.Count is not 0)
            {

                throw new FriendlyException("该零件id已经提交过了");
            }
            List<LaboratoryFee> laboratoryFees = ObjectMapper.Map<List<LaboratoryFee>>(productDepartmentModel.laboratoryFeeModels);
            //删除原数据
            await _resourceLaboratoryFee.DeleteAsync(p => p.AuditFlowId.Equals(price.AuditFlowId) && p.ProductId.Equals(productDepartmentModel.ProductId));
            foreach (LaboratoryFee laboratoryFee in laboratoryFees)
            {
                laboratoryFee.AuditFlowId = price.AuditFlowId;
                laboratoryFee.ProductId = productDepartmentModel.ProductId;
                await _resourceLaboratoryFee.InsertOrUpdateAsync(laboratoryFee);
            }
            #region 录入完成之后
            await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = price.AuditFlowId, ProductId = productDepartmentModel.ProductId, EnumSole = NreIsSubmitDto.ProductDepartment.ToString() });
            #endregion
            if (await this.GetProductDepartment(price.AuditFlowId))
            {
                if (AbpSession.UserId is null)
                {
                    throw new FriendlyException("请先登录");
                }
                ReturnDto retDto = await _flowAppService.UpdateAuditFlowInfo(new Audit.Dto.AuditFlowDetailDto()
                {
                    AuditFlowId = price.AuditFlowId,
                    ProcessIdentifier = AuditFlowConsts.AF_NreInputEmc,
                    UserId = AbpSession.UserId.Value,
                    Opinion = OPINIONTYPE.Submit_Agreee
                });
            }
        }
        /// <summary>
        /// Nre 产品部-电子工程师 导入数据(不提交)(Excel 单个零件解析数据)
        /// </summary>
        /// <returns></returns>
        public async Task<List<LaboratoryFeeModel>> PostProductDepartmentSingleExcel(IFormFile filename)
        {
            if (System.IO.Path.GetExtension(filename.FileName) is not ".xlsx") throw new FriendlyException("模板文件类型不正确");
            using (var memoryStream = new MemoryStream())
            {
                await filename.CopyToAsync(memoryStream);
                List<LaboratoryFeeExcelModel> rowExcls = memoryStream.Query<LaboratoryFeeExcelModel>(startCell: "A2").ToList();
                if (rowExcls.Count is 0) throw new FriendlyException("模板数据为空/未使用标准模板");
                List<LaboratoryFeeModel> rows = ObjectMapper.Map<List<LaboratoryFeeModel>>(rowExcls);
                return rows;
            }
        }
        /// <summary>
        ///  产品部-电子工程师  录入过的值(单个零件)
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<IsSubmitLaboratoryFeeModel> GetProductDepartmentSingle(long auditFlowId, long productId)
        {
            IsSubmitLaboratoryFeeModel isSubmitLaboratoryFeeModel = new();
            isSubmitLaboratoryFeeModel.ProductId = productId;
            //判断 该零件 是否已经录入
            int Count = await _resourceNreIsSubmit.CountAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.ProductId.Equals(productId) && p.EnumSole.Equals(NreIsSubmitDto.QRA1.ToString()));
            isSubmitLaboratoryFeeModel.IsSubmit = Count > 0;
            List<LaboratoryFee> laboratoryFees = await _resourceLaboratoryFee.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.ProductId.Equals(productId));
            List<LaboratoryFeeModel> laboratoryFeeModels = ObjectMapper.Map<List<LaboratoryFeeModel>>(laboratoryFees);
            isSubmitLaboratoryFeeModel.laboratoryFeeModels = laboratoryFeeModels;
            return isSubmitLaboratoryFeeModel;
        }

        /// <summary>
        /// Nre 品保部 录入
        /// </summary>
        /// <param name="qADepartmentDto"></param>
        /// <returns></returns>
        public async Task PostQADepartment(QADepartmentDto qADepartmentDto)
        {
            foreach (QADepartmentPartModel qad in qADepartmentDto.QADepartments)
            {
                //判断 该零件 是否已经录入
                List<NreIsSubmit> nreIsSubmitsQRA1 = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(qADepartmentDto.AuditFlowId) && p.ProductId.Equals(qad.ProductId) && p.EnumSole.Equals(NreIsSubmitDto.QRA1.ToString()));
                List<NreIsSubmit> nreIsSubmitsQRA2 = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(qADepartmentDto.AuditFlowId) && p.ProductId.Equals(qad.ProductId) && p.EnumSole.Equals(NreIsSubmitDto.QRA2.ToString()));
                if (nreIsSubmitsQRA1.Count is not 0 && nreIsSubmitsQRA2.Count is not 0)
                {

                    throw new FriendlyException(qad.ProductId + "该零件id已经提交过了");
                }
                try
                {
                    List<QADepartmentTest> qADepartments = ObjectMapper.Map<List<QADepartmentTest>>(qad.QATestDepartments);
                    //删除原数据
                    await _resourceQADepartmentTest.DeleteAsync(p => p.AuditFlowId.Equals(qADepartmentDto.AuditFlowId) && p.ProductId.Equals(qad.ProductId));
                    //试验项目表 模型
                    foreach (var item in qADepartments)
                    {
                        item.AuditFlowId = qADepartmentDto.AuditFlowId;
                        item.ProductId = qad.ProductId;
                        await _resourceQADepartmentTest.InsertOrUpdateAsync(item);
                    }
                    List<QADepartmentQC> qADepartmentQCs = ObjectMapper.Map<List<QADepartmentQC>>(qad.QAQCDepartments);
                    //删除原数据
                    await _resourceQADepartmentQC.DeleteAsync(p => p.AuditFlowId.Equals(qADepartmentDto.AuditFlowId) && p.ProductId.Equals(qad.ProductId));
                    //项目制程QC量检具表 模型
                    foreach (var item in qADepartmentQCs)
                    {
                        item.AuditFlowId = qADepartmentDto.AuditFlowId;
                        item.ProductId = qad.ProductId;
                        await _resourceQADepartmentQC.InsertOrUpdateAsync(item);
                    }
                    #region 录入完成之后
                    await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = qADepartmentDto.AuditFlowId, ProductId = qad.ProductId, EnumSole = NreIsSubmitDto.QRA1.ToString() });
                    await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = qADepartmentDto.AuditFlowId, ProductId = qad.ProductId, EnumSole = NreIsSubmitDto.QRA2.ToString() });
                    #endregion
                }
                catch (Exception e)
                {
                    throw new UserFriendlyException(e.Message);
                }
            }
        }
        /// <summary>
        ///Nre 品保部  判断是否全部提交完毕  true 所有零件已录完   false  没有录完
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetQADepartment(long Id)
        {
            //获取 总共的零件
            List<PartModel> partModels = await GetPart(Id);
            int AllCount = partModels.Count();
            //获取 已经提交的零件
            int Count1 = await _resourceNreIsSubmit.CountAsync(p => p.AuditFlowId.Equals(Id) && p.EnumSole.Equals(NreIsSubmitDto.QRA1.ToString()));
            int Count2 = await _resourceNreIsSubmit.CountAsync(p => p.AuditFlowId.Equals(Id) && p.EnumSole.Equals(NreIsSubmitDto.QRA2.ToString()));
            return AllCount <= Count1 && AllCount <= Count2;
        }
        /// <summary>
        /// Nre 品保部  退回重置状态
        /// </summary>
        /// <returns></returns>
        public async Task GetQADepartmentConfigurationState(long Id)
        {
            List<NreIsSubmit> nreAre1 = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.EnumSole.Equals(NreIsSubmitDto.QRA1.ToString()));
            foreach (NreIsSubmit item in nreAre1)
            {
                _resourceNreIsSubmit.HardDelete(item);
            }
            List<NreIsSubmit> nreAre2 = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.EnumSole.Equals(NreIsSubmitDto.QRA2.ToString()));
            foreach (NreIsSubmit item in nreAre2)
            {
                _resourceNreIsSubmit.HardDelete(item);
            }
        }
        /// <summary>
        /// Nre 品保部=>试验项目 录入
        /// </summary>
        /// <returns></returns>
        public async Task PostExperimentItems(ExperimentItemsDto experimentItems)
        {
            foreach (ExperimentItemsModel qad in experimentItems.ExperimentItems)
            {
                //判断 该零件 是否已经录入
                List<NreIsSubmit> nreIsSubmits = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(experimentItems.AuditFlowId) && p.ProductId.Equals(qad.ProductId) && p.EnumSole.Equals(NreIsSubmitDto.QRA1.ToString()));
                if (nreIsSubmits.Count is not 0)
                {

                    throw new FriendlyException(qad.ProductId + "该零件id已经提交过了");
                }
                try
                {
                    List<QADepartmentTest> qADepartments = ObjectMapper.Map<List<QADepartmentTest>>(qad.QATestDepartments);
                    //删除原数据
                    await _resourceQADepartmentTest.DeleteAsync(p => p.AuditFlowId.Equals(experimentItems.AuditFlowId) && p.ProductId.Equals(qad.ProductId));
                    //试验项目表 模型
                    foreach (var item in qADepartments)
                    {
                        item.AuditFlowId = experimentItems.AuditFlowId;
                        item.ProductId = qad.ProductId;
                        await _resourceQADepartmentTest.InsertOrUpdateAsync(item);
                    }
                    #region 录入完成之后
                    await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = experimentItems.AuditFlowId, ProductId = qad.ProductId, EnumSole = NreIsSubmitDto.QRA1.ToString() });

                    #endregion
                }
                catch (Exception e)
                {
                    throw new UserFriendlyException(e.Message);
                }
            }
        }
        /// <summary>
        ///Nre 品保部=>试验项目 录入  判断是否全部提交完毕  true 所有零件已录完   false  没有录完
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetExperimentItems(long Id)
        {
            //获取 总共的零件
            List<PartModel> partModels = await GetPart(Id);
            int AllCount = partModels.Count();
            //获取 已经提交的零件
            int Count = await _resourceNreIsSubmit.CountAsync(p => p.AuditFlowId.Equals(Id) && p.EnumSole.Equals(NreIsSubmitDto.QRA1.ToString())) + 1;
            return AllCount == Count;
        }
        /// <summary>
        /// Nre 品保部=>试验项目 录入  退回重置状态
        /// </summary>
        /// <returns></returns>
        public async Task GetExperimentItemsConfigurationState(long Id)
        {
            List<NreIsSubmit> nreAre1 = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.EnumSole.Equals(NreIsSubmitDto.QRA1.ToString()));
            foreach (NreIsSubmit item in nreAre1)
            {
                _resourceNreIsSubmit.HardDelete(item);
            }
        }
        /// <summary>
        /// Nre 品保部=>试验项目 录入(单个零件)
        /// </summary>
        /// <returns></returns>
        public async Task PostExperimentItemsSingle(ExperimentItemsSingleDto experimentItems)
        {
            ExperimentItemsModel experimentItemsModel = new();
            experimentItemsModel = experimentItems.ExperimentItems;
            //判断 该零件 是否已经录入
            List<NreIsSubmit> nreIsSubmits = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(experimentItems.AuditFlowId) && p.ProductId.Equals(experimentItemsModel.ProductId) && p.EnumSole.Equals(NreIsSubmitDto.QRA1.ToString()));
            if (nreIsSubmits.Count is not 0)
            {

                throw new FriendlyException("该零件id已经提交过了");
            }
            try
            {
                List<QADepartmentTest> qADepartments = ObjectMapper.Map<List<QADepartmentTest>>(experimentItemsModel.QATestDepartments);
                //删除原数据
                await _resourceQADepartmentTest.DeleteAsync(p => p.AuditFlowId.Equals(experimentItems.AuditFlowId) && p.ProductId.Equals(experimentItemsModel.ProductId));
                //试验项目表 模型
                foreach (var item in qADepartments)
                {
                    item.AuditFlowId = experimentItems.AuditFlowId;
                    item.ProductId = experimentItemsModel.ProductId;
                    await _resourceQADepartmentTest.InsertOrUpdateAsync(item);
                }
                #region 录入完成之后
                await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = experimentItems.AuditFlowId, ProductId = experimentItemsModel.ProductId, EnumSole = NreIsSubmitDto.QRA1.ToString() });
                #endregion
                if (await this.GetExperimentItems(experimentItems.AuditFlowId))
                {
                    if (AbpSession.UserId is null)
                    {
                        throw new FriendlyException("请先登录");
                    }
                    ReturnDto retDto = await _flowAppService.UpdateAuditFlowInfo(new Audit.Dto.AuditFlowDetailDto()
                    {
                        AuditFlowId = experimentItems.AuditFlowId,
                        ProcessIdentifier = AuditFlowConsts.AF_NreInputTest,
                        UserId = AbpSession.UserId.Value,
                        Opinion = OPINIONTYPE.Submit_Agreee
                    });
                }
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        ///  Nre 品保部=>试验项目 产品开发部-NRE 下载 Excel 解析数据模板
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public IActionResult PostExperimentItemsSingleDownloadExcel(string FileName = "NRE实验费模板下载")
        {
            try
            {
                string templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\wwwroot\Excel\NRE实验费模板.xlsx";
                return new FileStreamResult(File.OpenRead(templatePath), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"{FileName}.xlsx"
                };
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// Nre 品保部=>试验项目 导入数据(不提交)(Excel 单个零件解析数据)
        /// </summary>
        /// <returns></returns>
        public async Task<List<QADepartmentTestModel>> PostExperimentItemsSingleExcel(IFormFile filename)
        {
            if (System.IO.Path.GetExtension(filename.FileName) is not ".xlsx") throw new FriendlyException("模板文件类型不正确");
            using (var memoryStream = new MemoryStream())
            {
                await filename.CopyToAsync(memoryStream);
                List<QADepartmentTestExcelModel> rowExcls = memoryStream.Query<QADepartmentTestExcelModel>(startCell: "A2").ToList();
                if (rowExcls.Count is 0) throw new FriendlyException("模板数据为空/未使用标准模板");
                List<QADepartmentTestModel> rows = ObjectMapper.Map<List<QADepartmentTestModel>>(rowExcls);
                return rows;
            }
        }
        /// <summary>
        /// Nre 品保部=>试验项目 版本录入过的值
        /// </summary>
        /// <returns></returns>
        public async Task<List<ExperimentItemsModel>> GetReturnExperimentItems(long Id)
        {
            try
            {
                List<ExperimentItemsModel> experimentItemsModels = new();
                //所有的零件
                List<PartModel> partModels = new();
                partModels = await GetPart(Id);
                foreach (PartModel partModel in partModels)
                {
                    ExperimentItemsModel experimentItemsModel = new();
                    //判断 该零件 是否已经录入
                    int Count = await _resourceNreIsSubmit.CountAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(partModel.ProductId) && p.EnumSole.Equals(NreIsSubmitDto.QRA1.ToString()));
                    experimentItemsModel.IsSubmit = Count > 0;
                    experimentItemsModel.ProductId = partModel.ProductId;
                    experimentItemsModel.QATestDepartments = new();
                    List<QADepartmentTest> qADepartmentTests = await _resourceQADepartmentTest.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(partModel.ProductId));
                    if (qADepartmentTests is not null) experimentItemsModel.QATestDepartments = ObjectMapper.Map<List<QADepartmentTestModel>>(qADepartmentTests);
                    experimentItemsModels.Add(experimentItemsModel);

                }
                return experimentItemsModels;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// Nre 品保部=>试验项目 版本录入过的值(单个零件)
        /// </summary>
        /// <returns></returns>
        public async Task<ExperimentItemsModel> GetReturnExperimentItemsSingle(long auditFlowId, long productId)
        {
            try
            {
                List<ExperimentItemsModel> experimentItemsModels = new();
                //所有的零件
                List<PartModel> partModels = new();
                partModels = await GetPart(auditFlowId);
                partModels = partModels.Where(p => p.ProductId.Equals(productId)).ToList();
                foreach (PartModel partModel in partModels)
                {
                    ExperimentItemsModel experimentItemsModel = new();
                    //判断 该零件 是否已经录入
                    int Count = await _resourceNreIsSubmit.CountAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.ProductId.Equals(partModel.ProductId) && p.EnumSole.Equals(NreIsSubmitDto.QRA1.ToString()));
                    experimentItemsModel.IsSubmit = Count > 0;
                    experimentItemsModel.ProductId = partModel.ProductId;
                    experimentItemsModel.QATestDepartments = new();
                    List<QADepartmentTest> qADepartmentTests = await _resourceQADepartmentTest.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.ProductId.Equals(partModel.ProductId));
                    if (qADepartmentTests is not null) experimentItemsModel.QATestDepartments = ObjectMapper.Map<List<QADepartmentTestModel>>(qADepartmentTests);
                    experimentItemsModels.Add(experimentItemsModel);
                }
                return experimentItemsModels.FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// Nre 品保部=>项目制程QC量检具 录入
        /// </summary>
        /// <returns></returns>
        public async Task PostQcGauge(QcGaugeDto qcGaugeDto)
        {

            foreach (QcGaugeDtoModel qad in qcGaugeDto.QcGaugeDtoModels)
            {
                //判断 该零件 是否已经录入
                List<NreIsSubmit> nreIsSubmits = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(qcGaugeDto.AuditFlowId) && p.ProductId.Equals(qad.ProductId) && p.EnumSole.Equals(NreIsSubmitDto.QRA2.ToString()));
                if (nreIsSubmits.Count is not 0)
                {

                    throw new FriendlyException(qad.ProductId + "该零件id已经提交过了");
                }
                try
                {
                    List<QADepartmentQC> qADepartmentQCs = ObjectMapper.Map<List<QADepartmentQC>>(qad.QAQCDepartments);
                    //删除原数据
                    await _resourceQADepartmentQC.DeleteAsync(p => p.AuditFlowId.Equals(qcGaugeDto.AuditFlowId) && p.ProductId.Equals(qad.ProductId));
                    //项目制程QC量检具表 模型
                    foreach (var item in qADepartmentQCs)
                    {
                        item.AuditFlowId = qcGaugeDto.AuditFlowId;
                        item.ProductId = qad.ProductId;
                        await _resourceQADepartmentQC.InsertOrUpdateAsync(item);
                    }
                    #region 录入完成之后
                    await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = qcGaugeDto.AuditFlowId, ProductId = qad.ProductId, EnumSole = NreIsSubmitDto.QRA2.ToString() });

                    #endregion
                }
                catch (Exception e)
                {
                    throw new UserFriendlyException(e.Message);
                }
            }
        }
        /// <summary>
        ///Nre 品保部=>项目制程QC量检具 录入  判断是否全部提交完毕  true 所有零件已录完   false  没有录完
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetQcGauge(long Id)
        {
            //获取 总共的零件
            List<PartModel> partModels = await GetPart(Id);
            int AllCount = partModels.Count();
            //获取 已经提交的零件
            int Count = await _resourceNreIsSubmit.CountAsync(p => p.AuditFlowId.Equals(Id) && p.EnumSole.Equals(NreIsSubmitDto.QRA2.ToString())) + 1;
            return AllCount == Count;
        }
        /// <summary>
        ///Nre 品保部=>项目制程QC量检具 录入  退回重置状态
        /// </summary>
        /// <returns></returns>
        public async Task GetQcGaugeConfigurationState(long Id)
        {
            List<NreIsSubmit> nreAre1 = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.EnumSole.Equals(NreIsSubmitDto.QRA2.ToString()));
            foreach (NreIsSubmit item in nreAre1)
            {
                _resourceNreIsSubmit.HardDelete(item);
            }
        }
        /// <summary>
        /// Nre 品保部=>项目制程QC量检具 录入(单个零件)
        /// </summary>
        /// <returns></returns>
        public async Task PostQcGaugeSingle(QcGaugeDtoSingle qcGaugeDto)
        {
            QcGaugeDtoModel qcGaugeDtoModel = new();
            qcGaugeDtoModel = qcGaugeDto.QcGaugeDtoModels;
            //判断 该零件 是否已经录入
            List<NreIsSubmit> nreIsSubmits = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(qcGaugeDto.AuditFlowId) && p.ProductId.Equals(qcGaugeDtoModel.ProductId) && p.EnumSole.Equals(NreIsSubmitDto.QRA2.ToString()));
            if (nreIsSubmits.Count is not 0)
            {
                throw new FriendlyException("该零件id已经提交过了");
            }
            try
            {
                List<QADepartmentQC> qADepartmentQCs = ObjectMapper.Map<List<QADepartmentQC>>(qcGaugeDtoModel.QAQCDepartments);
                //删除原数据
                await _resourceQADepartmentQC.DeleteAsync(p => p.AuditFlowId.Equals(qcGaugeDto.AuditFlowId) && p.ProductId.Equals(qcGaugeDtoModel.ProductId));
                //项目制程QC量检具表 模型
                foreach (var item in qADepartmentQCs)
                {
                    item.AuditFlowId = qcGaugeDto.AuditFlowId;
                    item.ProductId = qcGaugeDtoModel.ProductId;
                    await _resourceQADepartmentQC.InsertOrUpdateAsync(item);
                }
                #region 录入完成之后
                await _resourceNreIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = qcGaugeDto.AuditFlowId, ProductId = qcGaugeDtoModel.ProductId, EnumSole = NreIsSubmitDto.QRA2.ToString() });

                #endregion
                if (await this.GetQcGauge(qcGaugeDto.AuditFlowId))
                {
                    if (AbpSession.UserId is null)
                    {
                        throw new FriendlyException("请先登录");
                    }
                    ReturnDto retDto = await _flowAppService.UpdateAuditFlowInfo(new Audit.Dto.AuditFlowDetailDto()
                    {
                        AuditFlowId = qcGaugeDto.AuditFlowId,
                        ProcessIdentifier = AuditFlowConsts.AF_NreInputGage,
                        UserId = AbpSession.UserId.Value,
                        Opinion = OPINIONTYPE.Submit_Agreee
                    });
                }
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// Nre 品保部=>项目制程QC量检具 录入过的值
        /// </summary>
        /// <returns></returns>
        public async Task<List<QcGaugeDtoModel>> GetReturnQcGauge(long Id)
        {
            try
            {
                List<QcGaugeDtoModel> qcGaugeDtoModels = new();
                //所有的零件
                List<PartModel> partModels = new();
                partModels = await GetPart(Id);
                foreach (PartModel partModel in partModels)
                {
                    QcGaugeDtoModel qcGaugeDtoModel = new();
                    qcGaugeDtoModel.ProductId = partModel.ProductId;
                    //判断 该零件 是否已经录入
                    int Count = await _resourceNreIsSubmit.CountAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(partModel.ProductId) && p.EnumSole.Equals(NreIsSubmitDto.QRA2.ToString()));
                    qcGaugeDtoModel.IsSubmit = Count > 0;
                    qcGaugeDtoModel.QAQCDepartments = new();
                    List<QADepartmentQC> qADepartmentQCs = await _resourceQADepartmentQC.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(partModel.ProductId));
                    if (qADepartmentQCs is not null) qcGaugeDtoModel.QAQCDepartments = ObjectMapper.Map<List<QADepartmentQCModel>>(qADepartmentQCs);
                    qcGaugeDtoModels.Add(qcGaugeDtoModel);
                }
                return qcGaugeDtoModels;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// Nre 品保部=>项目制程QC量检具 录入过的值(单个零件)
        /// </summary>
        /// <returns></returns>
        public async Task<QcGaugeDtoModel> GetReturnQcGaugeSingle(long auditFlowId, long productId)
        {
            try
            {
                List<QcGaugeDtoModel> qcGaugeDtoModels = new();
                //所有的零件
                List<PartModel> partModels = new();
                partModels = await GetPart(auditFlowId);
                partModels = partModels.Where(p => p.ProductId.Equals(productId)).ToList();
                foreach (PartModel partModel in partModels)
                {
                    QcGaugeDtoModel qcGaugeDtoModel = new();

                    qcGaugeDtoModel.ProductId = partModel.ProductId;
                    //判断 该零件 是否已经录入
                    int Count = await _resourceNreIsSubmit.CountAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.ProductId.Equals(partModel.ProductId) && p.EnumSole.Equals(NreIsSubmitDto.QRA2.ToString()));
                    qcGaugeDtoModel.IsSubmit = Count > 0;
                    qcGaugeDtoModel.QAQCDepartments = new();
                    List<QADepartmentQC> qADepartmentQCs = await _resourceQADepartmentQC.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.ProductId.Equals(partModel.ProductId));
                    if (qADepartmentQCs is not null) qcGaugeDtoModel.QAQCDepartments = ObjectMapper.Map<List<QADepartmentQCModel>>(qADepartmentQCs);
                    qcGaugeDtoModels.Add(qcGaugeDtoModel);
                }
                return qcGaugeDtoModels.FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// Ner  营销部录入初始值
        /// </summary>
        /// <param name="Id">流程表Id</param>
        /// <returns></returns>
        public async Task<List<ReturnSalesDepartmentDto>> GetInitialSalesDepartment(long Id)    
        {        
             
            List<ReturnSalesDepartmentDto> initialSalesDepartmentDtos = new();
            ReturnSalesDepartmentDto initialSalesDepartmentDto = new();
            List<ModelCount> modelCounts = await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            //手板件费            
            List<HandPieceCost> handPieceCosts = await _resourceHandPieceCost.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            foreach (ModelCount item in modelCounts)
            {
                initialSalesDepartmentDto.PricingMoney += handPieceCosts.Where(p => p.ProductId.Equals(item.Id)).Sum(s => s.Cost)/* * item.SingleCarProductsQuantity*/;
            }
            initialSalesDepartmentDto.FormName = StaticName.SBJF;//这里就是写死的,因为手板费是一个表,我要从指定的表获取 费用的总和,这个不能配置             
            initialSalesDepartmentDtos.Add(new ReturnSalesDepartmentDto() { FormName = initialSalesDepartmentDto.FormName, PricingMoney = initialSalesDepartmentDto.PricingMoney });
            //模具费
            initialSalesDepartmentDto = new();
            List<MouldInventory> mouldInventories = await _resourceMouldInventory.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            foreach (ModelCount item in modelCounts)
            {
                initialSalesDepartmentDto.PricingMoney += mouldInventories.Where(p => p.ProductId.Equals(item.Id)).Sum(s => s.Cost)/* * item.SingleCarProductsQuantity*/;
            }
            initialSalesDepartmentDto.FormName = StaticName.MJF;
            initialSalesDepartmentDtos.Add(new ReturnSalesDepartmentDto() { FormName = initialSalesDepartmentDto.FormName, PricingMoney = initialSalesDepartmentDto.PricingMoney });
            //生产设备费  
            initialSalesDepartmentDto = new();
            initialSalesDepartmentDto.FormName = StaticName.SCSBF;
            List<EquipmentInfo> equipmentInfos = (from a in await _resourceWorkingHoursInfo.GetAllListAsync(p => p.AuditFlowId.Equals(Id))
                                                  join b in await _resourceEquipmentInfo.GetAllListAsync(p => p.Part.Equals(Part.Equipment)) on a.Id equals b.WorkHoursId
                                                  select new EquipmentInfo
                                                  {
                                                      Number = b.Number,
                                                      UnitPrice = b.UnitPrice,
                                                      Id = a.ProductId,
                                                  }).ToList();
            foreach (ModelCount item in modelCounts)
            {
                initialSalesDepartmentDto.PricingMoney += equipmentInfos.Where(p => p.Id.Equals(item.Id)).Sum(item => item.Number * item.UnitPrice)/* * item.SingleCarProductsQuantity*/;
            }
            initialSalesDepartmentDtos.Add(new ReturnSalesDepartmentDto() { FormName = initialSalesDepartmentDto.FormName, PricingMoney = initialSalesDepartmentDto.PricingMoney });
            //工装治具费
            initialSalesDepartmentDto = new();
            initialSalesDepartmentDto.FormName = StaticName.GZZJF;//工装+治具+测试线         
            //测试线+工装
            List<WorkingHoursInfo> workingHours = await _resourceWorkingHoursInfo.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            //治具
            List<EquipmentInfo> equipment = (from a in workingHours
                                             join b in await _resourceEquipmentInfo.GetAllListAsync(p => p.Part.Equals(Part.Fixture)) on a.Id equals b.WorkHoursId
                                             select new EquipmentInfo
                                             {
                                                 Number = b.Number,
                                                 UnitPrice = b.UnitPrice,
                                                 Id = a.ProductId,
                                             }).ToList();
            foreach (ModelCount item in modelCounts)
            {
                initialSalesDepartmentDto.PricingMoney += workingHours.Where(p => p.ProductId.Equals(item.Id)).Sum(s => s.TestNum * s.TestPrice + s.ToolingPrice * s.ToolingNum)/* * item.SingleCarProductsQuantity*/;
                initialSalesDepartmentDto.PricingMoney += equipment.Where(p => p.Id.Equals(item.Id)).Sum(s => s.Number * s.UnitPrice)/* * item.SingleCarProductsQuantity*/;
            }
            initialSalesDepartmentDtos.Add(new ReturnSalesDepartmentDto() { FormName = initialSalesDepartmentDto.FormName, PricingMoney = initialSalesDepartmentDto.PricingMoney });
            //检具费
            initialSalesDepartmentDto = new();
            List<QADepartmentQC> qADepartmentQCs = await _resourceQADepartmentQC.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            initialSalesDepartmentDto.FormName = StaticName.JJF;
            foreach (ModelCount item in modelCounts)
            {
                initialSalesDepartmentDto.PricingMoney += qADepartmentQCs.Where(p => p.ProductId.Equals(item.Id)).Sum(s => s.Count*s.UnitPrice)/* * item.SingleCarProductsQuantity*/;
            }
            initialSalesDepartmentDtos.Add(new ReturnSalesDepartmentDto() { FormName = initialSalesDepartmentDto.FormName, PricingMoney = initialSalesDepartmentDto.PricingMoney });
            //实验费
            initialSalesDepartmentDto = new();
            List<LaboratoryFee> laboratoryFees = await _resourceLaboratoryFee.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            List<QADepartmentTest> qADepartmentTests = await _resourceQADepartmentTest.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            initialSalesDepartmentDto.FormName = StaticName.SYF;
            foreach (ModelCount item in modelCounts)
            {
                initialSalesDepartmentDto.PricingMoney += laboratoryFees.Where(p => p.ProductId.Equals(item.Id)).Sum(s => s.AllCost)/* * item.SingleCarProductsQuantity*/;
                initialSalesDepartmentDto.PricingMoney += qADepartmentTests.Where(p => p.ProductId.Equals(item.Id)).Sum(s => s.AllCost)/* * item.SingleCarProductsQuantity*/;
            }
            initialSalesDepartmentDtos.Add(new ReturnSalesDepartmentDto() { FormName = initialSalesDepartmentDto.FormName, PricingMoney = initialSalesDepartmentDto.PricingMoney });
            //测试软件费  (硬件费用+追溯软件费用+开图软件费用)
            initialSalesDepartmentDto = new();
            List<WorkingHoursInfo> workingHoursInfos = await _resourceWorkingHoursInfo.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            initialSalesDepartmentDto.FormName = StaticName.CSRJF;
            foreach (ModelCount item in modelCounts)
            {
                initialSalesDepartmentDto.PricingMoney += workingHoursInfos.Where(p => p.ProductId.Equals(item.Id)).Sum(s => s.HardwareTotalPrice + s.TraceabilityDevelopmentFee + s.MappingDevelopmentFee)/* * item.SingleCarProductsQuantity*/;
            }
            initialSalesDepartmentDtos.Add(new ReturnSalesDepartmentDto() { FormName = initialSalesDepartmentDto.FormName, PricingMoney = initialSalesDepartmentDto.PricingMoney });
            //差旅费
            initialSalesDepartmentDto = new();
            List<TravelExpense> travelExpenses = await _resourceTravelExpense.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            initialSalesDepartmentDto.FormName = StaticName.CLF;
            foreach (ModelCount item in modelCounts)
            {
                initialSalesDepartmentDto.PricingMoney += travelExpenses.Where(p => p.ProductId.Equals(item.Id)).Sum(s => s.Cost)/* * item.SingleCarProductsQuantity*/;
            }
            initialSalesDepartmentDtos.Add(new ReturnSalesDepartmentDto() { FormName = initialSalesDepartmentDto.FormName, PricingMoney = initialSalesDepartmentDto.PricingMoney });
            //其他费用
            initialSalesDepartmentDto = new();
            List<RestsCost> restsCosts = await _resourceRestsCost.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            initialSalesDepartmentDto.FormName = StaticName.QTFY;
            foreach (ModelCount item in modelCounts)
            {
                initialSalesDepartmentDto.PricingMoney += restsCosts.Where(p => p.ProductId.Equals(item.Id)).Sum(s => s.Cost)/* * item.SingleCarProductsQuantity*/;
            }
            initialSalesDepartmentDtos.Add(new ReturnSalesDepartmentDto() { FormName = initialSalesDepartmentDto.FormName, PricingMoney = initialSalesDepartmentDto.PricingMoney });
            List<ReturnSalesDepartmentDto> returnSalesDepartmentDtos = await GetReturnInitialSalesDepartment(Id);
            if(returnSalesDepartmentDtos is not null)
            {
                foreach (var returnSales in returnSalesDepartmentDtos)
                {
                    foreach (ReturnSalesDepartmentDto nitialSales in initialSalesDepartmentDtos)
                    {
                        if(returnSales.FormName.Equals(nitialSales.FormName))
                        {
                            nitialSales.Id = returnSales.Id;
                            nitialSales.OfferCoefficient = returnSales.OfferCoefficient;
                            nitialSales.OfferMoney = returnSales.OfferMoney;
                            nitialSales.Remark = returnSales.Remark;
                        }
                    }
                }
            }
            return initialSalesDepartmentDtos;
        }
        /// <summary>
        /// Ner  营销部录入
        /// </summary>
        /// <param name="departmentDtos"></param>
        /// <returns></returns>
        public async Task PostSalesDepartment(List<InitialSalesDepartmentDto> departmentDtos)
        {
            try
            {
                List<InitialResourcesManagement> initialResourcesManagements = ObjectMapper.Map<List<InitialResourcesManagement>>(departmentDtos);
                foreach (var item in initialResourcesManagements)
                {
                    await _resourceInitialResourcesManagement.InsertOrUpdateAsync(item);
                }
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// Ner  营销部 录入过的值
        /// </summary>
        /// <param name="Id">流程表Id</param>
        /// <returns></returns>
        public async Task<List<ReturnSalesDepartmentDto>> GetReturnInitialSalesDepartment(long Id)
        {
            List<InitialResourcesManagement> initialResourcesManagements = await _resourceInitialResourcesManagement.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            List<ReturnSalesDepartmentDto> returnSalesDepartmentDtos = new List<ReturnSalesDepartmentDto>();
            returnSalesDepartmentDtos = ObjectMapper.Map<List<ReturnSalesDepartmentDto>>(initialResourcesManagements);
            return returnSalesDepartmentDtos;
        }
        /// <summary>
        ///整个NRE  退回重置状态
        /// </summary>
        /// <returns></returns>
        public async Task GetNreConfigurationState(long Id)
        {
            List<NreIsSubmit> nreAre1 = await _resourceNreIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(Id));
            foreach (NreIsSubmit item in nreAre1)
            {
                _resourceNreIsSubmit.HardDelete(item);
            }
        }
        /// <summary>
        /// 获取 第一个页面最初的年份
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        private async Task<int> GetYear(long processId)
        {
            List<ModelCountYear> modelCountYears = await _resourceModelCountYear.GetAllListAsync(p => p.AuditFlowId.Equals(processId));
            List<int> yearList = modelCountYears.Select(p => p.Year).Distinct().ToList();
            int year = yearList.Min();
            return year;
        }
        /// <summary>
        /// 将json 转成  List YearOrValueMode>
        /// </summary>
        internal static List<YearOrValueMode> JsonExchangeRateValue(string price)
        {
            return JsonConvert.DeserializeObject<List<YearOrValueMode>>(price);
        }
        /// <summary>
        ///获取 Nre 核价表
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public async Task<PricingFormDto> GetPricingForm(long Id, long ProductId)
        {
            try
            {
                PriceEvaluation priceEvaluation = await _resourcePriceEvaluation.FirstOrDefaultAsync(p => p.AuditFlowId == Id);
                List<ModelCount> modelCount = await _resourceModelCount.GetAllListAsync(p => p.AuditFlowId == Id);
                PricingFormDto pricingFormDto = new();
                if (priceEvaluation is not null)
                {
                    pricingFormDto.ProjectName = priceEvaluation.ProjectName;
                    pricingFormDto.ClientName = priceEvaluation.CustomerName;
                }
                pricingFormDto.RequiredCapacity = modelCount.Sum(p => p.ModelTotal).ToString();
                //手板件费用
                List<HandPieceCost> handPieceCosts = await _resourceHandPieceCost.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(ProductId));
                pricingFormDto.HandPieceCost = ObjectMapper.Map<List<HandPieceCostModel>>(handPieceCosts);
                //模具费用
                List<MouldInventory> mouldInventories = await _resourceMouldInventory.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(ProductId));
                pricingFormDto.MouldInventory = ObjectMapper.Map<List<MouldInventoryModel>>(mouldInventories);
                //工装费用 (工装费用+测试线费用)              
                List<ToolingCostModel> workingHoursInfosGZ = new();
                //工装费用=>工装费用
                List<WorkingHoursInfo> workingHours = await _resourceWorkingHoursInfo.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(ProductId));
                workingHoursInfosGZ = workingHours.Where(p => p.ToolingName is not null).GroupBy(m => new { m.ToolingName, m.ToolingPrice }).Select(a => new ToolingCostModel
                {
                    WorkName = a.Key.ToolingName,
                    UnitPriceOfTooling = a.Key.ToolingPrice,
                    ToolingCount = a.Sum(m => m.ToolingNum),
                    Cost = a.Key.ToolingPrice * a.Sum(m => m.ToolingNum),
                }).ToList();
                pricingFormDto.ToolingCost = workingHoursInfosGZ;
                //工装费用=>测试线费用               
                List<ToolingCostModel> workingHoursInfosCSX = workingHours.Where(p => p.TestName is not null).GroupBy(m => new { m.TestName, m.TestPrice }).Select(a => new ToolingCostModel
                {
                    WorkName = a.Key.TestName,
                    UnitPriceOfTooling = a.Key.TestPrice,
                    ToolingCount = a.Sum(m => m.TestNum),
                    Cost = a.Key.TestPrice * a.Sum(m => m.TestNum),
                }).ToList();
                pricingFormDto.ToolingCost.AddRange(workingHoursInfosCSX);
                //治具费用               
                List<EquipmentInfo> equipmentInfosZj = (from a in await _resourceWorkingHoursInfo.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(ProductId))
                                                        join b in await _resourceEquipmentInfo.GetAllListAsync(p => p.Part.Equals(Part.Fixture)) on a.Id equals b.WorkHoursId
                                                        select new EquipmentInfo
                                                        {
                                                            WorkHoursId = b.WorkHoursId,
                                                            Part = b.Part,
                                                            EquipmentName = b.EquipmentName,
                                                            Status = b.Status,
                                                            Number = b.Number,
                                                            UnitPrice = b.UnitPrice,
                                                        }).ToList();
                List<FixtureCostModel> productionEquipmentCostModelsZj = equipmentInfosZj.GroupBy(m => new { m.EquipmentName, m.UnitPrice }).Select(
                     a => new FixtureCostModel
                     {
                         ToolingName = a.Key.EquipmentName,
                         UnitPrice = a.Key.UnitPrice,
                         Number = a.Sum(c => c.Number),
                         Cost = a.Key.UnitPrice * a.Sum(c => c.Number),
                     }).ToList();
                pricingFormDto.FixtureCost = productionEquipmentCostModelsZj;
                //检具费用
                List<QADepartmentQC> qADepartmentQCs = await _resourceQADepartmentQC.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(ProductId));
                pricingFormDto.QAQCDepartments = ObjectMapper.Map<List<QADepartmentQCModel>>(qADepartmentQCs);
                //生产设备费用 
                List<EquipmentInfo> equipmentInfos = (from a in await _resourceWorkingHoursInfo.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(ProductId))
                                                      join b in await _resourceEquipmentInfo.GetAllListAsync(p => p.Part.Equals(Part.Equipment)) on a.Id equals b.WorkHoursId
                                                      select new EquipmentInfo
                                                      {
                                                          WorkHoursId = b.WorkHoursId,
                                                          Part = b.Part,
                                                          EquipmentName = b.EquipmentName,
                                                          Status = b.Status,
                                                          Number = b.Number,
                                                          UnitPrice = b.UnitPrice,
                                                      }).ToList();
                List<ProductionEquipmentCostModel> productionEquipmentCostModels = equipmentInfos.GroupBy(m => new { m.EquipmentName, m.UnitPrice }).Select(
                    a => new ProductionEquipmentCostModel
                    {
                        EquipmentName = a.Key.EquipmentName,
                        UnitPrice = a.Key.UnitPrice,
                        Number = a.Sum(c => c.Number),
                        Cost = a.Key.UnitPrice * a.Sum(c => c.Number),
                    }).ToList();
                pricingFormDto.ProductionEquipmentCost = productionEquipmentCostModels;
                //实验费用
                {
                    //-产品部-电子工程师录入的试验费用
                    List<LaboratoryFee> laboratoryFees = await _resourceLaboratoryFee.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(ProductId));
                    //-品保部录入的实验费用
                    List<QADepartmentTest> qADepartmentTests = await _resourceQADepartmentTest.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(ProductId));
                    pricingFormDto.LaboratoryFeeModels = ObjectMapper.Map<List<LaboratoryFeeModel>>(laboratoryFees);
                    pricingFormDto.LaboratoryFeeModels.AddRange(ObjectMapper.Map<List<LaboratoryFeeModel>>(qADepartmentTests));
                }
                //测试软件费用
                List<WorkingHoursInfo> workingHoursInfos = await _resourceWorkingHoursInfo.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(ProductId));
                //测试软件费用=>硬件费用
                List<SoftwareTestingCotsModel> softwareTestingCots = new List<SoftwareTestingCotsModel>() { { new SoftwareTestingCotsModel() { SoftwareProject = "硬件费用", Cost = workingHoursInfos.Sum(p => p.HardwareTotalPrice) } } };
                pricingFormDto.SoftwareTestingCost = softwareTestingCots;
                //测试软件费用=>追溯软件费用
                pricingFormDto.SoftwareTestingCost.Add(new SoftwareTestingCotsModel { SoftwareProject = "追溯软件费用", Cost = workingHoursInfos.Sum(p => p.TraceabilityDevelopmentFee) });
                //测试软件费用=>开图软件费用
                pricingFormDto.SoftwareTestingCost.Add(new SoftwareTestingCotsModel { SoftwareProject = "开图软件费用", Cost = workingHoursInfos.Sum(p => p.MappingDevelopmentFee) });
                //差旅费
                List<TravelExpenseModel> travelExpenses = _resourceTravelExpense.GetAll().Where(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(ProductId))
                    .Join(_financeDictionaryDetailRepository.GetAll(), t => t.ReasonsId, p => p.Id, (t, p) => new TravelExpenseModel
                    {
                        ReasonsId = t.ReasonsId,
                        ReasonsName = p.DisplayName,
                        PeopleCount = t.PeopleCount,
                        CostSky = t.CostSky,
                        SkyCount = t.SkyCount,
                        Cost = t.Cost,
                        Remark = t.Remark,
                    }).ToList();
                pricingFormDto.TravelExpense = travelExpenses;
                //其他费用
                List<RestsCost> rests = await _resourceRestsCost.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.ProductId.Equals(ProductId));
                pricingFormDto.RestsCost = ObjectMapper.Map<List<RestsCostModel>>(rests);
                //(不含税人民币) NRE 总费用
                pricingFormDto.RMBAllCost = pricingFormDto.HandPieceCost.Sum(p => p.Cost)//手板件总费用
                                         + pricingFormDto.MouldInventory.Sum(p => p.Cost)//模具清单总费用
                                         + pricingFormDto.ToolingCost.Sum(p => p.Cost)//工装费用总费用
                                         + pricingFormDto.FixtureCost.Sum(p => p.Cost)//治具费用总费用
                                         + pricingFormDto.QAQCDepartments.Sum(p => p.Cost)//检具费用总费用
                                         + pricingFormDto.ProductionEquipmentCost.Sum(p => p.Cost)//生产设备总费用
                                         + pricingFormDto.LaboratoryFeeModels.Sum(p => p.AllCost)//实验费用总费用
                                         + pricingFormDto.SoftwareTestingCost.Sum(p => p.Cost)//测试软件总费用
                                         + pricingFormDto.TravelExpense.Sum(p => p.Cost)//差旅费总费用
                                         + pricingFormDto.RestsCost.Sum(p => p.Cost);//其他费用总费用
                int year = await GetYear(Id);
                //获取汇率
                ExchangeRate exchangeRate = await _configExchangeRate.FirstOrDefaultAsync(p => p.ExchangeRateKind.Equals("USD"));
                List<YearOrValueMode> yearOrValueModes = JsonExchangeRateValue(exchangeRate.ExchangeRateValue);
                YearOrValueMode exchangeRateModel = new();
                if (yearOrValueModes.Count is not 0) exchangeRateModel = yearOrValueModes.FirstOrDefault(p => p.Year.Equals(year));
                //(不含税美金) NRE 总费用
                pricingFormDto.USDAllCost = 0.0M;
                if (exchangeRateModel is not null)
                {
                    pricingFormDto.USDAllCost = pricingFormDto.RMBAllCost / exchangeRateModel.Value;
                }
                else
                {
                    pricingFormDto.USDAllCost = pricingFormDto.RMBAllCost;
                }

                return pricingFormDto;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
    }
}
