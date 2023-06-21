using Abp.AutoMapper;
using Abp.Modules;
using AutoMapper;
using Finance.FinanceMaintain;
using Finance.PropertyDepartment.Entering.Dto;
using Finance.PropertyDepartment.Entering.Model;
using Finance.PropertyDepartment.UnitPriceLibrary.Dto;
using Finance.PropertyDepartment.UnitPriceLibrary.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.UnitPriceLibrary
{

    internal class UnitPriceLibraryMapper
    {
        /// <summary>
        /// 对象映射
        /// </summary>
        /// <param name="configuration"></param>
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {

            configuration.CreateMap<UInitPriceFormModel, UInitPriceForm>()
                    .ForMember(u => u.FrozenState, p => p.MapFrom(input => input.FrozenState.Contains("已冻结")))//暂时写死
                    .ForMember(u => u.Priority, p => p.MapFrom(input => input.Priority.Contains("核心"))) //暂时写死
                    .ForMember(u => u.Floor, p => p.MapFrom(input => !string.IsNullOrWhiteSpace(input.Floor) ? decimal.Parse(input.Floor) : 0M))
                    .ForMember(u => u.Upper, p => p.MapFrom(input => !string.IsNullOrWhiteSpace(input.Upper) ? decimal.Parse(input.Upper) : 0M))
                    .ForMember(u => u.ECCNCode, p => p.MapFrom(input => string.IsNullOrWhiteSpace(input.ECCNCode) ? "待定" : input.ECCNCode));
            configuration.CreateMap<UInitPriceForm, UInitPriceFormDto>()
                      .ForMember(u => u.DisplayFrozenState, p => p.MapFrom(input => input.FrozenState ? "已冻结" : "未冻结"))//暂时写死
                      .ForMember(u => u.Priority, p => p.MapFrom(input => input.Priority ? "核心" : "现货/临时")); //暂时写死
            configuration.CreateMap<BacktrackGrossMarginDto, GrossMarginForm>()
                      .ForMember(u => u.GrossMarginPrice, p => p.MapFrom(o => o.GrossMarginPrice.StringGrossMargin(StringGrossMargin)));
            configuration.CreateMap<GrossMarginForm, GrossMarginDto>()
                     .ForMember(u => u.GrossMarginPrice, p => p.MapFrom(o => o.GrossMarginPrice.SplitGrossMargin(SplitGrossMargin)));
            configuration.CreateMap<ExchangeRateDto, ExchangeRate>()
                 .ForMember(u => u.ExchangeRateValue, p => p.MapFrom(o => JsonConvert.SerializeObject(o.ExchangeRateValue)));
            configuration.CreateMap<ExchangeRate, ExchangeRateDto>()
                .ForMember(u => u.ExchangeRateValue, p => p.MapFrom(o => o.ExchangeRateValue.JsonExchangeRateValue(JsonExchangeRateValue)));
        }
        /// <summary>
        /// 将list 拆分 string
        /// </summary>
        public static Func<List<decimal>, string> StringGrossMargin = p =>
        {

            if (p.Count>0)
            {
                return string.Join("|", p.ToArray());
            }
            else
            {
                return "";
            }
        };
        /// <summary>
        /// 将字符串拆分成一个一个毛利率  string  to 将list
        /// </summary>
        public static Func<string, List<decimal>> SplitGrossMargin = p =>
        {
            if (!string.IsNullOrEmpty(p))
            {
                string[] price = p.Split('|');
                List<decimal> result = new();
                foreach (var item in price)
                {
                    if (!string.IsNullOrEmpty(item)) result.Add(decimal.Parse(item));
                }
                return result;
            }
            else
            {
                return null;
            }
        };
        /// <summary>
        ///  将json 转成   List YearOrValueMode>
        /// </summary>
        public static Func<string, List<YearOrValueMode>> JsonExchangeRateValue = p =>
        {
            return JsonConvert.DeserializeObject<List<YearOrValueMode>>(p);
        };
    }

}
