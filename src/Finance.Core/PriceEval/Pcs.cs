using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval
{
    /// <summary>
    /// 终端走量（PCS）
    /// </summary>
    [Table("Pe_Pcs")]
    public class Pcs : FullAuditedEntity<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 核价表主键
        /// </summary>
        [Required]
        public virtual long PriceEvaluationId { get; set; }

        /// <summary>
        /// 车厂
        /// </summary>
        [Required]
        public virtual string CarFactory { get; set; }

        /// <summary>
        /// 车型
        /// </summary>
        [Required]
        public virtual string CarModel { get; set; }
    }

    /// <summary>
    /// 终端走量年份
    /// </summary>
    [Table("Pe_PcsYear")]
    public class PcsYear : Entity<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 主表 终端走量（PCS） 的Id
        /// </summary>
        [Required]
        public virtual long PcsId { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        [Required]
        public virtual int Year { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [Required]
        public virtual int Quantity { get; set; }

    }
}
