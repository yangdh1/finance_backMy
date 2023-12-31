﻿using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Finance.Audit;
using Finance.Audit.Dto;
using Finance.EngineeringDepartment;
using Finance.Entering;
using Finance.Ext;
using Finance.FinanceMaintain;
using Finance.FinanceParameter;
using Finance.Hr;
using Finance.Infrastructure;
using Finance.NerPricing;
using Finance.Nre;
using Finance.PriceEval.Dto;
using Finance.PriceEval.Dto.AllManufacturingCost;
using Finance.ProductDevelopment;
using Finance.ProductionControl;
using Finance.ProjectManagement;
using Finance.ProjectManagement.Dto;
using Finance.PropertyDepartment.Entering.Method;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using Rougamo;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.PriceEval
{
    /// <summary>
    /// 核价服务
    /// </summary>
    [ParameterValidator]
    public class PriceEvaluationAppService : PriceEvaluationGetAppService
    {
        #region 类初始化



        private readonly IRepository<ProductInformation, long> _productInformationRepository;
        private readonly IRepository<Department, long> _departmentRepository;
        private readonly NrePricingAppService _nrePricingAppService;
        private readonly IRepository<AuditFlow, long> _auditFlowRepository;
        private readonly IRepository<FileManagement, long> _fileManagementRepository;
        /// <summary>
        ///  零件是否全部录入 依据实体类
        /// </summary>
        private readonly IRepository<NreIsSubmit, long> _productIsSubmit;

        /// <summary>
        /// 流程流转服务
        /// </summary>
        private readonly AuditFlowAppService _flowAppService;

        public PriceEvaluationAppService(IRepository<FinanceDictionaryDetail, string> financeDictionaryDetailRepository, IRepository<PriceEvaluation, long> priceEvaluationRepository, IRepository<Pcs, long> pcsRepository, IRepository<PcsYear, long> pcsYearRepository, IRepository<ModelCount, long> modelCountRepository, IRepository<ModelCountYear, long> modelCountYearRepository, IRepository<Requirement, long> requirementRepository, IRepository<ElectronicBomInfo, long> electronicBomInfoRepository, IRepository<StructureBomInfo, long> structureBomInfoRepository, IRepository<EnteringElectronic, long> enteringElectronicRepository, IRepository<StructureElectronic, long> structureElectronicRepository, IRepository<LossRateInfo, long> lossRateInfoRepository, IRepository<LossRateYearInfo, long> lossRateYearInfoRepository, IRepository<ExchangeRate, long> exchangeRateRepository, IRepository<ManufacturingCostInfo, long> manufacturingCostInfoRepository, IRepository<YearInfo, long> yearInfoRepository, IRepository<WorkingHoursInfo, long> workingHoursInfoRepository, IRepository<RateEntryInfo, long> rateEntryInfoRepository, IRepository<ProductionControlInfo, long> productionControlInfoRepository, IRepository<QualityRatioEntryInfo, long> qualityCostProportionEntryInfoRepository, IRepository<UserInputInfo, long> userInputInfoRepository, IRepository<QualityRatioYearInfo, long> qualityCostProportionYearInfoRepository, IRepository<UPHInfo, long> uphInfoRepository, IRepository<AllManufacturingCost, long> allManufacturingCostRepository,
            IRepository<ProductInformation, long> productInformationRepository, IRepository<Department, long> departmentRepository, NrePricingAppService nrePricingAppService, IRepository<AuditFlow, long> auditFlowRepository, IRepository<FileManagement, long> fileManagementRepository, AuditFlowAppService flowAppService, IRepository<NreIsSubmit, long> productIsSubmit)
            : base(financeDictionaryDetailRepository, priceEvaluationRepository, pcsRepository, pcsYearRepository, modelCountRepository, modelCountYearRepository, requirementRepository, electronicBomInfoRepository, structureBomInfoRepository, enteringElectronicRepository, structureElectronicRepository, lossRateInfoRepository, lossRateYearInfoRepository, exchangeRateRepository, manufacturingCostInfoRepository, yearInfoRepository, workingHoursInfoRepository, rateEntryInfoRepository, productionControlInfoRepository, qualityCostProportionEntryInfoRepository, userInputInfoRepository, qualityCostProportionYearInfoRepository, uphInfoRepository, allManufacturingCostRepository)
        {
            _productInformationRepository = productInformationRepository;
            _departmentRepository = departmentRepository;
            _nrePricingAppService = nrePricingAppService;
            _auditFlowRepository = auditFlowRepository;
            _fileManagementRepository = fileManagementRepository;
            _flowAppService = flowAppService;
            _productIsSubmit = productIsSubmit;
        }



        #endregion

        #region 核价开始
        /// <summary>
        /// 开始核价：报价核价需求录入界面（第一步）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize]
        public async virtual Task<PriceEvaluationStartResult> PriceEvaluationStart(PriceEvaluationStartInput input)
        {
            long auditFlowId;
            var check = from m in input.ModelCount
                        join p in input.ProductInformation on m.Product equals p.Product
                        select m;
            if (check.Count() != input.ModelCount.Count)
            {
                throw new FriendlyException($"产品信息和模组数量没有正确对应！");
            }
            if (!input.SorFile.Any())
            {
                throw new FriendlyException($"SOR文件没有上传！");
            }

            //验证模组信息是否存在重复数据
            var isModelCount = input.ModelCount.GroupBy(p => p.Product).Any(p => p.Count() > 1);
            if (isModelCount)
            {
                throw new FriendlyException($"模组数量有重复的零件名称！");
            }
            var isProductInformation = input.ProductInformation.GroupBy(p => p.Product).Any(p => p.Count() > 1);
            if (isProductInformation)
            {
                throw new FriendlyException($"产品信息有重复的零件名称！");
            }

            var modelMinYeay = input.ModelCount.SelectMany(p => p.ModelCountYearList).Min(p => p.Year);
            var pcsMinYeay = input.Pcs.SelectMany(p => p.PcsYearList).Min(p => p.Year);
            var requirementMinYeay = input.Requirement.Min(p => p.Year);



            if (modelMinYeay < input.SopTime || pcsMinYeay < input.SopTime || requirementMinYeay < input.SopTime)
            {
                throw new FriendlyException($"SOP年份和实际录入的模组数量、产品信息、PCS不吻合！");
            }



            //PriceEvaluation
            var priceEvaluation = ObjectMapper.Map<PriceEvaluation>(input);

            var user = await UserManager.GetUserByIdAsync(AbpSession.UserId.Value);

            var department = await _departmentRepository.FirstOrDefaultAsync(user.DepartmentId);

            if (department is not null)
            {
                priceEvaluation.DraftingCompanyId = department.CompanyId;
            }
            priceEvaluation.DraftingDepartmentId = user.DepartmentId;

            long flowId = await _flowAppService.SavaNewAuditFlowInfo(new Audit.Dto.AuditFlowDto()
            {
                QuoteProjectName = input.ProjectName,
                QuoteProjectNumber = input.ProjectCode
            });
            auditFlowId = flowId;
            priceEvaluation.AuditFlowId = auditFlowId;
            var priceEvaluationId = await _priceEvaluationRepository.InsertAndGetIdAsync(priceEvaluation);

            //Pcs
            foreach (var createPcsDto in input.Pcs)
            {
                var pcs = ObjectMapper.Map<Pcs>(createPcsDto);
                pcs.PriceEvaluationId = priceEvaluationId;
                pcs.AuditFlowId = auditFlowId;
                var pcsId = await _pcsRepository.InsertAndGetIdAsync(pcs);
                //Pcs 年份
                foreach (var createPcsYearDto in createPcsDto.PcsYearList)
                {
                    var pcsYear = ObjectMapper.Map<PcsYear>(createPcsYearDto);
                    pcsYear.AuditFlowId = auditFlowId;
                    pcsYear.PcsId = pcsId;
                    await _pcsYearRepository.InsertAsync(pcsYear);
                }
            }

            //模组数量
            foreach (var createModelCountDto in input.ModelCount)
            {
                var modelCount = ObjectMapper.Map<ModelCount>(createModelCountDto);
                modelCount.PriceEvaluationId = priceEvaluationId;
                modelCount.AuditFlowId = auditFlowId;
                var modelCountId = await _modelCountRepository.InsertAndGetIdAsync(modelCount);
                foreach (var createModelCountYearDto in createModelCountDto.ModelCountYearList)
                {
                    var modelCountYear = ObjectMapper.Map<ModelCountYear>(createModelCountYearDto);
                    modelCountYear.ModelCountId = modelCountId;
                    modelCountYear.AuditFlowId = auditFlowId;
                    await _modelCountYearRepository.InsertAsync(modelCountYear);
                }
            }

            //要求
            var requirements = ObjectMapper.Map<List<Requirement>>(input.Requirement);
            foreach (var requirement in requirements)
            {
                requirement.PriceEvaluationId = priceEvaluationId;
                requirement.AuditFlowId = auditFlowId;
                await _requirementRepository.InsertAsync(requirement);
            }

            //产品信息
            var productInformations = ObjectMapper.Map<List<ProductInformation>>(input.ProductInformation);
            foreach (var productInformation in productInformations)
            {
                productInformation.PriceEvaluationId = priceEvaluationId;
                productInformation.AuditFlowId = auditFlowId;
                await _productInformationRepository.InsertAsync(productInformation);
            }
            _flowAppService.SavaProjectManagerInfo(input.ProjectManager);
            await _flowAppService.UpdateAuditFlowInfo(new AuditFlowDetailDto()
            {
                AuditFlowId = auditFlowId,
                ProcessIdentifier = AuditFlowConsts.AF_RequirementInput,
                UserId = user.Id,
                Opinion = OPINIONTYPE.Submit_Agreee
            });
            return new PriceEvaluationStartResult { IsSuccess = true, Message = "添加成功！" };
        }

        /// <summary>
        /// 获取【PriceEvaluationStart】（开始核价：报价核价需求录入界面（第一步））接口输入的数据
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        public async virtual Task<PriceEvaluationStartInputResult> GetPriceEvaluationStartData(long auditFlowId)
        {
            var priceEvaluation = await _priceEvaluationRepository.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId);
            var priceEvaluationDto = ObjectMapper.Map<PriceEvaluationStartInputResult>(priceEvaluation);

            var quoteVersion = await _auditFlowRepository.GetAll().Where(p => p.Id == auditFlowId).Select(p => p.QuoteVersion).FirstOrDefaultAsync();

            var fileNames = await _fileManagementRepository.GetAll().Where(p => priceEvaluationDto.SorFile.Contains(p.Id))
                .Select(p => new FileUploadOutputDto { FileId = p.Id, FileName = p.Name, })
                .ToListAsync();

            var pcsDto = (await _pcsRepository.GetAll().Where(p => p.AuditFlowId == auditFlowId)
                   .Join(_pcsYearRepository.GetAll(), p => p.Id, p => p.PcsId, (pcs, pcstYear) => new { pcs, pcstYear }).ToListAsync()).GroupBy(p => p.pcs).Select(p =>
                   {
                       var dto = ObjectMapper.Map<CreatePcsDto>(p.Key);
                       dto.PcsYearList = ObjectMapper.Map<List<CreatePcsYearDto>>(p.Select(o => o.pcstYear));
                       return dto;
                   }).ToList();

            var modelCountDto = (await _modelCountRepository.GetAll().Where(p => p.AuditFlowId == auditFlowId)
                   .Join(_modelCountYearRepository.GetAll(), p => p.Id, p => p.ModelCountId, (modelCount, modelCountYear) => new { modelCount, modelCountYear }).ToListAsync()).GroupBy(p => p.modelCount).Select(p =>
                   {
                       var dto = ObjectMapper.Map<CreateModelCountDto>(p.Key);
                       dto.ModelCountYearList = ObjectMapper.Map<List<CreateModelCountYearDto>>(p.Select(o => o.modelCountYear));
                       return dto;
                   }).ToList();

            var requirementDto = ObjectMapper.Map<List<CreateRequirementDto>>(await _requirementRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId));


            var productInformation = await _productInformationRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
            var productInformationDto = ObjectMapper.Map<List<CreateColumnFormatProductInformationDto>>(productInformation);

            priceEvaluationDto.Pcs = pcsDto;
            priceEvaluationDto.ModelCount = modelCountDto;
            priceEvaluationDto.Requirement = requirementDto;
            priceEvaluationDto.ProductInformation = productInformationDto;
            priceEvaluationDto.QuoteVersion = quoteVersion;
            priceEvaluationDto.Files = fileNames;
            return priceEvaluationDto;
        }

        #endregion

        #region 核价看板
        /// <summary>
        /// 核价看板-利润分布图
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<ListResultDto<ProportionOfProductCostListDto>> GetPricingPanelProfit(GetPricingPanelProfitInput input)
        {
            var data = await this.GetPriceEvaluationTable(new GetPriceEvaluationTableInput { AuditFlowId = input.AuditFlowId, InputCount = 0, ModelCountId = input.ModelCountId, Year = input.Year });

            //bom成本
            var bomCost = data.Material.Sum(p => p.TotalMoneyCyn);

            //损耗成本
            var costItemAll = data.Material.Sum(p => p.Loss);

            //制造成本
            var manufacturingCost = data.ManufacturingCost.FirstOrDefault(p => p.CostType == CostType.Total).Subtotal;

            //物流成本
            var logisticsFee = data.OtherCostItem.LogisticsFee;

            //质量成本
            var qualityCost = data.OtherCostItem.QualityCost;

            var sum = bomCost + costItemAll + manufacturingCost + logisticsFee + qualityCost;

            var list = new List<ProportionOfProductCostListDto>
            {
                new ProportionOfProductCostListDto{ Name="bom成本", Proportion= bomCost},
                new ProportionOfProductCostListDto{ Name="损耗成本", Proportion= costItemAll},
                new ProportionOfProductCostListDto{ Name="制造成本", Proportion= manufacturingCost},
                new ProportionOfProductCostListDto{ Name="物流成本", Proportion= logisticsFee},
                new ProportionOfProductCostListDto{ Name="质量成本", Proportion= qualityCost},
            };

            var customerTargetPrice = await _productInformationRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId);
            list.Add(new ProportionOfProductCostListDto { Name = "利润", Proportion = customerTargetPrice.CustomerTargetPrice - sum });
            return new ListResultDto<ProportionOfProductCostListDto>(list);
        }


        /// <summary>
        /// 初版NRE核价表下载-流
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public async virtual Task<MemoryStream> NreTableDownloadStream(NreTableDownloadInput input)
        {
            var data = await _nrePricingAppService.GetPricingForm(input.AuditFlowId, input.ModelCountId);

            var dto = ObjectMapper.Map<ExcelPricingFormDto>(data);

            //模组名
            var modelCountName = await _modelCountRepository.GetAll().Where(p => p.Id == input.ModelCountId).Select(p => p.Product).FirstOrDefaultAsync();
            dto.ProjectName = $"{modelCountName}——{dto.ProjectName}";


            dto.HandPieceCost = dto.HandPieceCost.Select((p, i) => { p.Index = i + 1; return p; }).ToList();
            dto.MouldInventory = dto.MouldInventory.Select((p, i) => { p.Index = i + 1; return p; }).ToList();
            dto.ToolingCost = dto.ToolingCost.Select((p, i) => { p.Index = i + 1; return p; }).ToList();
            dto.FixtureCost = dto.FixtureCost.Select((p, i) => { p.Index = i + 1; return p; }).ToList();
            dto.QAQCDepartments = dto.QAQCDepartments.Select((p, i) => { p.Index = i + 1; return p; }).ToList();
            dto.ProductionEquipmentCost = dto.ProductionEquipmentCost.Select((p, i) => { p.Index = i + 1; return p; }).ToList();
            dto.LaboratoryFeeModels = dto.LaboratoryFeeModels.Select((p, i) => { p.Index = i + 1; p.Count = (p.DataThoroughly + p.DataDV + p.DataPV) + p.Unit; return p; }).ToList();
            dto.SoftwareTestingCost = dto.SoftwareTestingCost.Select((p, i) => { p.Index = i + 1; return p; }).ToList();
            dto.TravelExpense = dto.TravelExpense.Select((p, i) => { p.Index = i + 1; return p; }).ToList();
            dto.RestsCost = dto.RestsCost.Select((p, i) => { p.Index = i + 1; return p; }).ToList();

            dto.HandPieceCostSum = dto.HandPieceCost.Sum(p => p.Cost);
            dto.MouldInventorySum = dto.MouldInventory.Sum(p => p.Cost);
            dto.ToolingCostSum = dto.ToolingCost.Sum(p => p.Cost);
            dto.FixtureCostSum = dto.FixtureCost.Sum(p => p.Cost);
            dto.QAQCDepartmentsSum = dto.QAQCDepartments.Sum(p => p.Cost);
            dto.ProductionEquipmentCostSum = dto.ProductionEquipmentCost.Sum(p => p.Cost);
            dto.LaboratoryFeeModelsSum = dto.LaboratoryFeeModels.Sum(p => p.AllCost);
            dto.SoftwareTestingCostSum = dto.SoftwareTestingCost.Sum(p => p.Cost);
            dto.TravelExpenseSum = dto.TravelExpense.Sum(p => p.Cost);
            dto.RestsCostSum = dto.RestsCost.Sum(p => p.Cost);


            var memoryStream = new MemoryStream();

            await MiniExcel.SaveAsByTemplateAsync(memoryStream, "wwwroot/Excel/NRE.xlsx", dto);
            return memoryStream;
        }

        /// <summary>
        /// 初版NRE核价表下载
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public async virtual Task<FileResult> NreTableDownload(NreTableDownloadInput input)
        {
            var data = await _nrePricingAppService.GetPricingForm(input.AuditFlowId, input.ModelCountId);


            var dto = ObjectMapper.Map<ExcelPricingFormDto>(data);

            //模组名
            var modelCountName = await _modelCountRepository.GetAll().Where(p => p.Id == input.ModelCountId).Select(p => p.Product).FirstOrDefaultAsync();
            dto.ProjectName = $"{modelCountName}——{dto.ProjectName}";

            dto.HandPieceCost = dto.HandPieceCost.Select((p, i) => { p.Index = i + 1; return p; }).ToList();
            dto.MouldInventory = dto.MouldInventory.Select((p, i) => { p.Index = i + 1; return p; }).ToList();
            dto.ToolingCost = dto.ToolingCost.Select((p, i) => { p.Index = i + 1; return p; }).ToList();
            dto.FixtureCost = dto.FixtureCost.Select((p, i) => { p.Index = i + 1; return p; }).ToList();
            dto.QAQCDepartments = dto.QAQCDepartments.Select((p, i) => { p.Index = i + 1; return p; }).ToList();
            dto.ProductionEquipmentCost = dto.ProductionEquipmentCost.Select((p, i) => { p.Index = i + 1; return p; }).ToList();
            dto.LaboratoryFeeModels = dto.LaboratoryFeeModels.Select((p, i) => { p.Index = i + 1; p.Count = (p.DataThoroughly + p.DataDV + p.DataPV) + p.Unit; return p; }).ToList();
            dto.SoftwareTestingCost = dto.SoftwareTestingCost.Select((p, i) => { p.Index = i + 1; return p; }).ToList();
            dto.TravelExpense = dto.TravelExpense.Select((p, i) => { p.Index = i + 1; return p; }).ToList();
            dto.RestsCost = dto.RestsCost.Select((p, i) => { p.Index = i + 1; return p; }).ToList();

            dto.HandPieceCostSum = dto.HandPieceCost.Sum(p => p.Cost);
            dto.MouldInventorySum = dto.MouldInventory.Sum(p => p.Cost);
            dto.ToolingCostSum = dto.ToolingCost.Sum(p => p.Cost);
            dto.FixtureCostSum = dto.FixtureCost.Sum(p => p.Cost);
            dto.QAQCDepartmentsSum = dto.QAQCDepartments.Sum(p => p.Cost);
            dto.ProductionEquipmentCostSum = dto.ProductionEquipmentCost.Sum(p => p.Cost);
            dto.LaboratoryFeeModelsSum = dto.LaboratoryFeeModels.Sum(p => p.AllCost);
            dto.SoftwareTestingCostSum = dto.SoftwareTestingCost.Sum(p => p.Cost);
            dto.TravelExpenseSum = dto.TravelExpense.Sum(p => p.Cost);
            dto.RestsCostSum = dto.RestsCost.Sum(p => p.Cost);


            var memoryStream = new MemoryStream();

            await MiniExcel.SaveAsByTemplateAsync(memoryStream, "wwwroot/Excel/NRE.xlsx", dto);

            return new FileContentResult(memoryStream.ToArray(), "application/octet-stream") { FileDownloadName = "NRE核价表.xlsx" };
        }

        #endregion

        #region 制造成本

        /// <summary>
        /// 保存 制造成本（添加和修改）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task ManufacturingCostInput(ManufacturingCostInput input)
        {
            List<NreIsSubmit> productIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(input.AuditFlowId) && p.ProductId.Equals(input.ModelCountId) && p.EnumSole.Equals(AuditFlowConsts.AF_ProductionCostInput));
            if (productIsSubmits.Count is not 0)
            {
                throw new FriendlyException(input.ModelCountId + ":该零件id已经提交过了");
            }
            else
            {
                //查询核价需求导入时的零件信息
                var productIds = await _modelCountRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId);

                #region Input data check

                if (input.Smt.IsNullOrEmpty() || input.Cob.IsNullOrEmpty() || input.Other.IsNullOrEmpty())
                {
                    throw new FriendlyException($"输入的SMT、COB或其他制造成本为空！");
                }

                //获取总年数
                var yearCount = await _modelCountYearRepository.GetAll()
                    .Where(p => p.AuditFlowId == input.AuditFlowId && p.ModelCountId == input.ModelCountId)
                    .Select(p => p.Year).OrderBy(p => p).ToListAsync();
                yearCount.Add(PriceEvalConsts.AllYear);
                yearCount = yearCount.OrderBy(p => p).ToList();
                var smtYear = input.Smt.Select(p => p.Year).OrderBy(p => p).ToList();
                var cobYear = input.Cob.Select(p => p.Year).OrderBy(p => p).ToList();
                var otherYear = input.Other.Select(p => p.Year).OrderBy(p => p).ToList();

                if (!(yearCount.Count == smtYear.Count && yearCount.Count == cobYear.Count)
                    || !yearCount.Zip(smtYear, (a, b) => a == b).All(p => p)
                    || !yearCount.Zip(cobYear, (a, b) => a == b).All(p => p)
                    || !yearCount.Zip(otherYear, (a, b) => a == b).All(p => p))
                {
                    throw new FriendlyException($"输入的SMT、COB或其他制造成本的年份和要求的录入的年份{string.Join("、", yearCount.Select(p => p == 0 ? "全生命周期" : p.ToString()))}不一致");
                }
                #endregion


                await Save(input.Smt, CostType.SMT);
                await Save(input.Cob, CostType.COB);
                await Save(input.Other, CostType.Other);

                #region 保存数据
                async Task Save<TManufacturingCostInput>(List<TManufacturingCostInput> a, CostType b) where TManufacturingCostInput : IManufacturingCostInput
                {
                    foreach (var cost in a)
                    {
                        var dbEntity = await _allManufacturingCostRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId
                                                && p.Year == cost.Year && p.ModelCountId == input.ModelCountId && p.CostType == b);
                        if (dbEntity is null)
                        {
                            var entity = ObjectMapper.Map<AllManufacturingCost>(cost);
                            entity.AuditFlowId = input.AuditFlowId;
                            entity.ModelCountId = input.ModelCountId;
                            entity.CostType = b;
                            await _allManufacturingCostRepository.InsertAsync(entity);
                        }
                        else
                        {
                            ObjectMapper.Map(cost, dbEntity);
                            await _allManufacturingCostRepository.UpdateAsync(dbEntity);
                        }
                    }
                }

                #endregion


                #region 录入完成之后
                await _productIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = input.AuditFlowId, ProductId = input.ModelCountId, EnumSole = AuditFlowConsts.AF_ProductionCostInput });
                #endregion

                List<NreIsSubmit> allProductIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(input.AuditFlowId) && p.EnumSole.Equals(AuditFlowConsts.AF_ProductionCostInput));
                //当前已保存的确认表中零件数目等于 核价需求导入时的零件数目
                if (productIds.Count == allProductIsSubmits.Count + 1)
                {
                    //执行跳转
                    if (AbpSession.UserId is null)
                    {
                        throw new FriendlyException("请先登录");
                    }

                    await _flowAppService.UpdateAuditFlowInfo(new Audit.Dto.AuditFlowDetailDto()
                    {
                        AuditFlowId = input.AuditFlowId,
                        ProcessIdentifier = AuditFlowConsts.AF_ProductionCostInput,
                        UserId = AbpSession.UserId.Value,
                        Opinion = OPINIONTYPE.Submit_Agreee,
                    });
                }
            }
        }

        /// <summary>
        /// 制造成本录入 退回重置状态
        /// </summary>
        /// <returns></returns>
        public async Task ClearProductionCostInputState(long Id)
        {
            List<NreIsSubmit> productIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.EnumSole.Equals(AuditFlowConsts.AF_ProductionCostInput));
            foreach (NreIsSubmit item in productIsSubmits)
            {
                await _productIsSubmit.HardDeleteAsync(item);
            }
        }

        /// <summary>
        /// 获取录入的制造成本
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<ManufacturingCostInput> GetInputManufacturingCost(GetManufacturingCostInputDto input)
        {
            var data = await _allManufacturingCostRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId
            && p.ModelCountId == input.ModelCountId).ToListAsync();
            var cob = ObjectMapper.Map<List<AllManufacturingCostInput>>(data.Where(p => p.CostType == CostType.COB));
            var smt = ObjectMapper.Map<List<AllManufacturingCostInput>>(data.Where(p => p.CostType == CostType.SMT));
            var other = ObjectMapper.Map<List<OtherManufacturingCostInput>>(data.Where(p => p.CostType == CostType.Other));

            if (cob.Count == 0 && smt.Count == 0)
            {
                return null;
            }
            return new ManufacturingCostInput { AuditFlowId = input.AuditFlowId, ModelCountId = input.ModelCountId, Cob = cob, Smt = smt, Other = other };
        }

        #endregion
    }
}
