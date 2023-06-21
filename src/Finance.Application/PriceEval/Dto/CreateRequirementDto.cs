using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 创建 要求 的 Dto
    /// </summary>
    public class CreateRequirementDto
    {
        /// <summary>
        /// 年降率（%）
        /// </summary>
        [Required]
        public virtual decimal AnnualDeclineRate { get; set; }


        /// <summary>
        /// 年度返利要求（金额）
        /// </summary>
        [Required]
        public virtual decimal AnnualRebateRequirements { get; set; }

        /// <summary>
        /// 一次性折让率（%）
        /// </summary>
        [Required]
        public virtual decimal OneTimeDiscountRate { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        [Required]
        [Range(FinanceConsts.MinYear, FinanceConsts.MaxYear)]
        public virtual int Year { get; set; }
    }
}
