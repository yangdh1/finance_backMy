﻿using Abp.Domain.Entities;
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
    /// 模组数量
    /// </summary>
    [Table("Pe_ModelCount")]
    public class ModelCount : FullAuditedEntity<long>
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
        /// 序号（正序排序，从1开始）
        /// </summary>
        [Required]
        public virtual long Order { get; set; }

        /// <summary>
        /// 客户零件号
        /// </summary>
        public virtual string PartNumber { get; set; }


        /// <summary>
        /// 产品（产品名称从这里取）（字典明细表主键）
        /// </summary>
        [Required]
        public virtual string Product { get; set; }

        /// <summary>
        /// 产品小类（字典明细表主键）
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
        /// 投入量
        /// </summary>
        public virtual int? InputCount { get; set; }

        /// <summary>
        /// 年份（生成核价表的年份）
        /// </summary>
        public virtual int? Year { get; set; }

        /// <summary>
        /// 核价表Json
        /// </summary>
        [Column(TypeName = "CLOB")]
        public virtual string TableJson { get; set; }

        /// <summary>
        /// 核价表Json（全生命周期）
        /// </summary>
        [Column(TypeName = "CLOB")]
        public virtual string TableAllJson { get; set; }
    }


    /// <summary>
    /// 模组数量年份
    /// </summary>
    [Table("Pe_ModelCountYear")]
    public class ModelCountYear : Entity<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 主表 模组数量（ModelCount） 的Id
        /// </summary>
        [Required]
        public virtual long ModelCountId { get; set; }

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

        /// <summary>
        /// 核价表Json
        /// </summary>
        [Column(TypeName = "CLOB")]
        public virtual string TableJson { get; set; }

    }
}
