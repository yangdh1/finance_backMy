using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 创建 模组数量 的参数
    /// </summary>
    public class CreateModelCountDto
    {
        /// <summary>
        /// 序号（正序排序，从1开始）
        /// </summary>
        [Required]
        public virtual long Order { get; set; }

        /// <summary>
        /// 客户零件号
        /// </summary>
        public virtual string PartNumber { get; set; }


        /// <summary>
        /// 产品（字典明细表主键，根据字典名，调用【FinanceDictionary/GetFinanceDictionaryAndDetailByName】取字典，字典名Name是【Product】）
        /// </summary>
        [Required]
        public virtual string Product { get; set; }

        /// <summary>
        /// 产品小类（字典明细表主键，根据字典名，调用【FinanceDictionary/GetFinanceDictionaryAndDetailByName】取字典，字典名Name是【ProductType】）
        /// </summary>
        [Required]
        public virtual string ProductType { get; set; }

        /// <summary>
        /// 市场份额（%）
        /// </summary>
        [Required]
        public virtual decimal MarketShare { get; set; }

        /// <summary>
        /// 模组搭载率
        /// </summary>
        [Required]
        public virtual decimal ModuleCarryingRate { get; set; }

        /// <summary>
        /// 单车产品数量
        /// </summary>
        [Required]
        public virtual int SingleCarProductsQuantity { get; set; }

        /// <summary>
        /// 模组总量
        /// </summary>
        [Required]
        public virtual int ModelTotal { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        [Required]
        public virtual List<CreateModelCountYearDto> ModelCountYearList { get; set; }
    }

    /// <summary>
    /// 创建 终端走量年份 的参数
    /// </summary>
    public class CreateModelCountYearDto
    {
        /// <summary>
        /// 年份
        /// </summary>
        [Required]
        [Range(FinanceConsts.MinYear, FinanceConsts.MaxYear)]
        public virtual int Year { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [Required]
        [Range(1, long.MaxValue)]
        public virtual int Quantity { get; set; }

    }
}
