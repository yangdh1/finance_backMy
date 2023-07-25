using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.Processes
{
    [Table("BomEnterTotal")]
	public class BomEnterTotal : FullAuditedEntity<long>
    {

		/// <summary>
		/// 
		/// </summary>
		[Column("audit_flow_id")]
		public decimal? AuditFlowId { get; set; }

		/// <summary>
		/// 分类
		/// </summary>
		[Column("classification")]
		[StringLength(255, ErrorMessage = "分类长度不能超出255字符")]
		public string Classification { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[Column("product_id")]
		public decimal? ProductId { get; set; }

		/// <summary>
		/// 备注
		/// </summary>
		[Column("remark")]
		[StringLength(255, ErrorMessage = "备注长度不能超出255字符")]
		public string Remark { get; set; }

		/// <summary>
		/// 总价
		/// </summary>
		[Column("total_cost")]
		public decimal? TotalCost { get; set; }

		/// <summary>
		/// 年
		/// </summary>
		[Column("year")]
		[StringLength(255, ErrorMessage = "年长度不能超出255字符")]
		public string Year { get; set; }

	}
}
