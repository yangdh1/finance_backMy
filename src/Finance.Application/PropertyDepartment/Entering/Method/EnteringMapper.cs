

using AutoMapper;
using Finance.Entering;
using Finance.Entering.Model;
using Finance.ProductDevelopment;
using Finance.PropertyDepartment.Entering.Dto;
using Finance.PropertyDepartment.Entering.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Finance.PropertyDepartment.Entering.Method
{
    /// <summary>
    /// 
    /// </summary>
    public class EnteringMapper
    {
        /// <summary>
        /// 对象映射
        /// </summary>
        /// <param name="configuration"></param>
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<ElectronicBomInfo, ElectronicDto>()
                 .ForMember(u => u.ElectronicId, options => options.MapFrom(input => input.Id));
            configuration.CreateMap<ElectronicDto, EnteringElectronic>()
                 .ForMember(u => u.MaterialsUseCount, p => p.MapFrom(input => ListToJson(input.MaterialsUseCount)))
                 .ForMember(u => u.SystemiginalCurrency, p => p.MapFrom(input => ListToJson(input.SystemiginalCurrency)))
                 .ForMember(u => u.InTheRate, p => p.MapFrom(input => ListToJson(input.InTheRate)))
                 .ForMember(u => u.IginalCurrency, p => p.MapFrom(input => ListToJson(input.IginalCurrency)))
                 .ForMember(u => u.StandardMoney, p => p.MapFrom(input => ListToJson(input.StandardMoney)));
            configuration.CreateMap<StructuralMaterialModel, StructureElectronic>()
                 .ForMember(u => u.Sop, p => p.MapFrom(input => ListToJson(input.Sop)))
                 .ForMember(u => u.IginalCurrency, p => p.MapFrom(input => ListToJson(input.IginalCurrency)))
                 .ForMember(u => u.StandardMoney, p => p.MapFrom(input => ListToJson(input.StandardMoney)))
                 .ForMember(u => u.MaterialsUseCount, p => p.MapFrom(input => ListToJson(input.MaterialsUseCount)))
                 .ForMember(u => u.InTheRate, p => p.MapFrom(input => ListToJson(input.InTheRate)))
                 .ForMember(u => u.SystemiginalCurrency, p => p.MapFrom(input => ListToJson(input.SystemiginalCurrency)));
            configuration.CreateMap<StructureBomInfo, ConstructionModel>()
                 .ForMember(u => u.StructureId, p => p.MapFrom(input => input.Id));

            //从数据库实体类 映射到交互类
            configuration.CreateMap<EnteringElectronic, ElectronicDto>()
                 .ForMember(u => u.MaterialsUseCount, p => p.MapFrom(input => JsonToList(input.MaterialsUseCount)))
                 .ForMember(u => u.SystemiginalCurrency, p => p.MapFrom(input => JsonToList(input.SystemiginalCurrency)))
                 .ForMember(u => u.InTheRate, p => p.MapFrom(input => JsonToList(input.InTheRate)))
                 .ForMember(u => u.IginalCurrency, p => p.MapFrom(input => JsonToList(input.IginalCurrency)))
                 .ForMember(u => u.StandardMoney, p => p.MapFrom(input => JsonToList(input.StandardMoney)));


            configuration.CreateMap<ElectronicBomInfoBak, ElectronicBomInfo>();
        }
        /// <summary>
        /// 将json 转成   List YearOrValueMode>
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public static List<YearOrValueMode> JsonToList(string price)
        {
            return JsonConvert.DeserializeObject<List<YearOrValueMode>>(price);
        }
        /// <summary>
        /// 将list 序列化成json
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public static string ListToJson(List<YearOrValueMode> price)
        {
            return JsonConvert.SerializeObject(price);
        }
    }
}
