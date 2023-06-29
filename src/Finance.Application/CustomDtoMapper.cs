﻿using Abp.Extensions;
using AutoMapper;
using Finance.Audit;
using Finance.Audit.Dto;
using Finance.Authorization.Roles;
using Finance.Authorization.Users;
using Finance.BaseLibrary;
using Finance.EngineeringDepartment;
using Finance.EngineeringDepartment.Dto;
using Finance.Ext;
using Finance.FinanceDepartment.Dto;
using Finance.FinanceParameter;
using Finance.Hr;
using Finance.Hr.Dto;
using Finance.Infrastructure;
using Finance.Infrastructure.Dto;
using Finance.NrePricing.Dto;
using Finance.NrePricing.Model;
using Finance.PriceEval;
using Finance.PriceEval.Dto;
using Finance.PriceEval.Dto.AllManufacturingCost;
using Finance.PriceEval.Dto.DataTableVersion;
using Finance.PriceEval.Dto.Timelinesss;
using Finance.ProductDevelopment;
using Finance.ProductDevelopment.Dto;
using Finance.ProductionControl;
using Finance.ProjectManagement;
using Finance.ProjectManagement.Dto;
using Finance.Roles.Dto;
using Finance.TradeCompliance;
using Finance.Users.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance
{
    internal class CustomDtoMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            //ExcelPricingFormDto
            configuration.CreateMap<PricingFormDto, ExcelPricingFormDto>();
            configuration.CreateMap<HandPieceCostModel, ExcelHandPieceCostModel>();
            configuration.CreateMap<MouldInventoryModel, ExcelMouldInventoryModel>();
            configuration.CreateMap<ToolingCostModel, ExcelToolingCostModel>();
            configuration.CreateMap<FixtureCostModel, ExcelFixtureCostModel>();
            configuration.CreateMap<QADepartmentQCModel, ExcelQADepartmentQCModel>();
            configuration.CreateMap<ProductionEquipmentCostModel, ExcelProductionEquipmentCostModel>();
            configuration.CreateMap<LaboratoryFeeModel, ExcelLaboratoryFeeModel>();
            configuration.CreateMap<SoftwareTestingCotsModel, ExcelSoftwareTestingCotsModel>();
            configuration.CreateMap<TravelExpenseModel, ExcelTravelExpenseModel>();
            configuration.CreateMap<RestsCostModel, ExcelRestsCostModel>();

            configuration.CreateMap<PriceEvaluationTableDto, ExcelPriceEvaluationTableDto>();


            configuration.CreateMap<AllManufacturingCostInput, AllManufacturingCost>();
            configuration.CreateMap<AllManufacturingCost, AllManufacturingCostInput>();

            configuration.CreateMap<OtherManufacturingCostInput, AllManufacturingCost>();
            configuration.CreateMap<AllManufacturingCost, OtherManufacturingCostInput>();


            //User
            configuration.CreateMap<ExcelImportUserDto, CreateUserDto>()
                .ForMember(p => p.RoleNames, p => p.MapFrom(o => o.RoleNames.SplitPro(',')))
                .ForMember(p => p.Number, p => p.MapFrom(o => o.Number.To<long>()));

            configuration.CreateMap<ExcelImportUserDto, UserDto>()
                .ForMember(p => p.RoleNames, p => p.MapFrom(o => o.RoleNames.SplitPro(',')))
                .ForMember(p => p.Number, p => p.MapFrom(o => o.Number.To<long>()));

            configuration.CreateMap<User, UserDto>();

            //AuditFlow
            configuration.CreateMap<AuditFlowDto, AuditFlow>();
            configuration.CreateMap<AuditFlowDetailDto, AuditFlowDetail>();
            configuration.CreateMap<AuditFlowDetailDto, AuditFlowRight>();
            configuration.CreateMap<FlowProcess, FlowProcessDto>();
            configuration.CreateMap<FlowJumpInfo, FlowJumpInfoDto>();
            configuration.CreateMap<FlowClearInfo, FlowClearInfoDto>();

            //NoticeEmail
            configuration.CreateMap<NoticeEmailInfo, EmailDto > ();

            //FinanceDictionary
            configuration.CreateMap<AddFinanceDictionaryInput, FinanceDictionary>();
            configuration.CreateMap<EditFinanceDictionaryInput, FinanceDictionary>();
            configuration.CreateMap<FinanceDictionary, FinanceDictionaryListDto>();

            configuration.CreateMap<FinanceDictionary, FinanceDictionaryAndDetailListDto>();



            //FinanceDictionaryDetail
            configuration.CreateMap<AddFinanceDictionaryDetailInput, FinanceDictionaryDetail>();
            configuration.CreateMap<EditFinanceDictionaryDetailInput, FinanceDictionaryDetail>();


            configuration.CreateMap<FinanceDictionaryDetail, FinanceDictionaryDetailListDto>();

            configuration.CreateMap<AddDepartment, Department>();
            configuration.CreateMap<Department, DepartmentListDto>();
            configuration.CreateMap<Department, DepartmentListExcelDto>();

            //Timeliness
            configuration.CreateMap<Timeliness, TimelinessDto>().ForMember(p => p.Data, p => p.MapFrom(o => JsonConvert.DeserializeObject<List<NameValue>>(o.Data)));
            configuration.CreateMap<SetTimelinessDto, Timeliness>().ForMember(p => p.Data, p => p.MapFrom(o => JsonConvert.SerializeObject(o.Data)));


            //PriceEvaluation
            configuration.CreateMap<PriceEvaluationStartInput, PriceEvaluation>().ForMember(p => p.SorFile, p => p.MapFrom(o => JsonConvert.SerializeObject(o.SorFile)));
            configuration.CreateMap<PriceEvaluation, PriceEvaluationStartInput>().ForMember(p => p.SorFile, p => p.MapFrom(o => JsonConvert.DeserializeObject<List<long>>(o.SorFile)));
            configuration.CreateMap<PriceEvaluation, PriceEvaluationStartInputResult>().ForMember(p => p.SorFile, p => p.MapFrom(o => JsonConvert.DeserializeObject<List<long>>(o.SorFile)));

            configuration.CreateMap<CreatePcsDto, Pcs>();
            configuration.CreateMap<CreatePcsYearDto, PcsYear>();

            configuration.CreateMap<Pcs, CreatePcsDto>();
            configuration.CreateMap<PcsYear, CreatePcsYearDto>();

            configuration.CreateMap<Pcs, PcsListDto>();
            configuration.CreateMap<PcsYear, PcsYearListDto>();

            configuration.CreateMap<CreateModelCountDto, ModelCount>();
            configuration.CreateMap<CreateModelCountYearDto, ModelCountYear>();
            configuration.CreateMap<CreateRequirementDto, Requirement>();
            configuration.CreateMap<CreateProductInformationDto, ProductInformation>();
            configuration.CreateMap<CreateColumnFormatProductInformationDto, ProductInformation>();


            configuration.CreateMap<ModelCount, CreateModelCountDto>();
            configuration.CreateMap<ModelCountYear, CreateModelCountYearDto>();
            configuration.CreateMap<Requirement, CreateRequirementDto>();
            configuration.CreateMap<ProductInformation, CreateProductInformationDto>();
            configuration.CreateMap<ProductInformation, CreateColumnFormatProductInformationDto>();

            configuration.CreateMap<ModelCount, ModelCountListDto>();
            configuration.CreateMap<ModelCountYear, ModelCountYearListDto>();


            configuration.CreateMap<ProductionControlInfo, ProductionControlInfoListDto>();


            configuration.CreateMap<Material, CostDetailVarianceMaterialInfo>();
            configuration.CreateMap<ManufacturingCost, CostDetailVarianceManufacturingCostInfo>();


            //ProductDevelopmentInput
            configuration.CreateMap<StructureBomDto, StructureBomInfo>();
            configuration.CreateMap<StructureBomDto, StructureBomInfoBak>();
            configuration.CreateMap<StructureBomInfoBak, StructureBomInfo>();
            configuration.CreateMap<ElectronicBomDto, ElectronicBomInfo>();
            configuration.CreateMap<ElectronicBomDto, ElectronicBomInfoBak>();
            configuration.CreateMap<ElectronicBomInfoBak, ElectronicBomInfo>();
            configuration.CreateMap<ProductDevelopmentInputDto, ProductDevelopmentInput>().ConvertUsing<ProductDevelopmentInputDtoConverter>();

            configuration.CreateMap<LossRateDto, LossRateInfo>();
            configuration.CreateMap<LossRateInfo, LossRateDto>();
            configuration.CreateMap<LossRateYearDto, LossRateYearInfo>();
            configuration.CreateMap<LossRateYearInfo, LossRateYearDto>();

            configuration.CreateMap<RateEntryDto, RateEntryInfo>();

            configuration.CreateMap<QualityRatioEntryInfo, QualityCostDto>();
            configuration.CreateMap<QualityCostDto, QualityRatioEntryInfo>();
            configuration.CreateMap<QualityRatioYearInfo, QualityCostYearDto>();
            configuration.CreateMap<QualityCostYearDto, QualityRatioYearInfo>();

            configuration.CreateMap<UserInputDto, UserInputInfo>();
            configuration.CreateMap<UserInputInfo, UserInputDto>();

            configuration.CreateMap<Material, ProductMaterialInfo>()
                .ForMember(p => p.MaterialCode, p => p.MapFrom(o => o.Sap))
                .ForMember(p => p.MaterialName, p => p.MapFrom(o => o.MaterialName))
                .ForMember(p => p.Count, p => p.MapFrom(o => o.AssemblyCount))
                .ForMember(p => p.UnitPrice, p => p.MapFrom(o => o.MaterialPriceCyn))
                .ForMember(p => p.Amount, p => p.MapFrom(o => o.TotalMoneyCyn));

            configuration.CreateMap<ProcessMaintenanceDto, BaseProcessMaintenance>();
            configuration.CreateMap<BaseProcessMaintenance, ProcessMaintenanceDto>();

            configuration.CreateMap<ProcesshoursenteritemDto, Processhoursenteritem>();
            configuration.CreateMap<Processhoursenteritem, ProcesshoursenteritemDto>();

            configuration.CreateMap<ProcesshoursenterDto, Processhoursenter>();
            configuration.CreateMap<Processhoursenter, ProcesshoursenterDto>();
        }
    }
}
