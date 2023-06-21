using AutoMapper;
using Finance.Nre;
using Finance.NrePricing.Dto;
using Finance.NrePricing.Model;
using Finance.ProductDevelopment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing.Method
{
    /// <summary>
    /// 
    /// </summary>
    public class NrePricingMapper
    {
        /// <summary>
        /// 对象映射
        /// </summary>
        /// <param name="configuration"></param>
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {

            configuration.CreateMap<HandPieceCostModel, HandPieceCost>();

            configuration.CreateMap<HandPieceCost, HandPieceCostModel>();


            configuration.CreateMap<RestsCostModel, RestsCost>()
                  .ForMember(u => u.ConstName, p => p.MapFrom(o => o.Rroject)); ;
            configuration.CreateMap<MouldInventoryModel, MouldInventory>();
            configuration.CreateMap<MouldInventory, MouldInventoryModel>();
            configuration.CreateMap<TravelExpenseModel, TravelExpense>();

            configuration.CreateMap<StructureBomInfo, MouldInventoryModel>()
                .ForMember(u => u.ModelName, p => p.MapFrom(o => o.TypeName))
                .ForMember(u => u.Id, options => options.Ignore())
                .ForMember(u => u.StructuralId, p => p.MapFrom(o => o.Id));
            configuration.CreateMap<LaboratoryFeeModel, LaboratoryFee>();
            configuration.CreateMap<QADepartmentTestModel, QADepartmentTest>();
            configuration.CreateMap<QADepartmentQCModel, QADepartmentTest>();

            configuration.CreateMap<QADepartmentQCModel, QADepartmentQC>()
                 .ForMember(u => u.Cost, p => p.MapFrom(o => o.Count*o.UnitPrice));

            configuration.CreateMap<QADepartmentTest, QADepartmentTestModel>();

            configuration.CreateMap<QADepartmentQC, QADepartmentQCModel>()
                 .ForMember(u => u.Cost, p => p.MapFrom(o => o.Count * o.UnitPrice));
            ;
            configuration.CreateMap<InitialSalesDepartmentDto, InitialResourcesManagement>();

            configuration.CreateMap<LaboratoryFee, LaboratoryFeeModel>()
            .ForMember(u => u.IsThirdPartyName, p => p.MapFrom(o => o.IsThirdParty ? "是" : "否"));
            configuration.CreateMap<QADepartmentTest, LaboratoryFeeModel>()
               .ForMember(u => u.IsThirdPartyName, p => p.MapFrom(o => o.IsThirdParty ? "是" : "否"))
               .ForMember(u => u.TestItem, p => p.MapFrom(o => o.ProjectName));

            configuration.CreateMap<TravelExpense, TravelExpenseModel>();
            configuration.CreateMap<RestsCost, RestsCostModel>()
                 .ForMember(u => u.Rroject, p => p.MapFrom(o => o.ConstName));

            configuration.CreateMap<InitialResourcesManagement, ReturnSalesDepartmentDto>();

            configuration.CreateMap<QADepartmentTestExcelModel, QADepartmentTestModel>()
                 .ForMember(u => u.IsThirdParty, p => p.MapFrom(o => o.IsThirdParty=="是"));

            configuration.CreateMap<LaboratoryFeeExcelModel, LaboratoryFeeModel>()
                .ForMember(u => u.IsThirdParty, p => p.MapFrom(o => o.IsThirdParty=="是"))
                .ForMember(u => u.IsThirdPartyName, p => p.MapFrom(o => o.IsThirdParty));
        }
    }
}
