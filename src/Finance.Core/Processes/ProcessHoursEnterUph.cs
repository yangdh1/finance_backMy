using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.Processes
{
    [Table("ProcessHoursEnterUph")]
	public class ProcessHoursEnterUph : FullAuditedEntity<long>
    {

		/// <summary>
		/// 
		/// </summary>
		[Column("audit_flow_id")]
		[StringLength(255, ErrorMessage = "AuditFlowId长度不能超出255字符")]
		public string AuditFlowId { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[Column("product_id")]
		[StringLength(255, ErrorMessage = "ProductId长度不能超出255字符")]
		public string ProductId { get; set; }

		/// <summary>
		/// UPH数据
		/// </summary>
		[Column("UPH")]
		[StringLength(255, ErrorMessage = "UPH数据长度不能超出255字符")]
		public string Uph { get; set; }

		/// <summary>
		/// 值
		/// </summary>
		[Column("value")]
		public decimal? Value { get; set; }

		/// <summary>
		/// 年份
		/// </summary>
		[Column("year")]
		[StringLength(255, ErrorMessage = "年份长度不能超出255字符")]
		public string Year { get; set; }


	}
}
