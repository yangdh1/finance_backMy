using Finance.PropertyDepartment.Entering.Dto;
using Finance.PropertyDepartment.Entering.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.UnitPriceLibrary.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scoure"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        internal static List<decimal> SplitGrossMargin(this string scoure, Func<string, List<decimal>> func)
        {
             return func(scoure);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scoure"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        internal static List<YearOrValueMode> JsonExchangeRateValue(this string scoure, Func<string, List<YearOrValueMode>> func)
        {
            return func(scoure);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scoure"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        internal static string StringGrossMargin(this List<decimal> scoure, Func<List<decimal>,string> func)
        {
            return func(scoure);
        }
    }
}
