using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.BaseLibrary
{
    [Table("ProcessHoursEnteritem")]
    public class Processhoursenteritem : FullAuditedEntity<long>
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        [Column("id")]
        public override long Id { get; set; }

        /// <summary>
        /// 标准人工工时
        /// </summary>
        [Column("labor_hour")]
        public decimal? LaborHour { get; set; }

        /// <summary>
        /// 标准机器工时
        /// </summary>
        [Column("machine_hour")]
        public decimal? MachineHour { get; set; }

        /// <summary>
        /// 人员数量
        /// </summary>
        [Column("number_personnel")]
        public decimal? NumberPersonnel { get; set; }

        /// <summary>
        /// 工时工序id
        /// </summary>
        [Column("process_hours_enter_id")]
        public decimal? ProcessHoursenterId { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        [Column("year")]
        public int? Year { get; set; }

    }
}
