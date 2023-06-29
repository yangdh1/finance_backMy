using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 工序维护
    /// </summary>
    [Table("Base_Process_Maintenance")]
    public class BaseProcessMaintenance : FullAuditedEntity<long>
    {
        /// <summary>
        /// id
        /// </summary>
        [Key]
        [Column("id")]
        public override long Id { get; set; }

        /// <summary>
        /// 是否删除 0--正常 1--删除
        /// </summary>
        [Column("is_deleted")]
        public override bool IsDeleted { get; set; }

        /// <summary>
        /// 删除人
        /// </summary>
        [Column("deleter_user_id")]
        public override long? DeleterUserId { get; set; }

        /// <summary>
        /// 删除人
        /// </summary>
        [Column("delete_time")]
        public override DateTime? DeletionTime { get; set; }

        /// <summary>
        /// 工序编号
        /// </summary>
        [Column("process_number")]
        public string ProcessNumber{ get; set; }

        /// <summary>
        /// 工序名称
        /// </summary>
        [Column("process_name")]
        public string ProcessName { get; set; }
    }
}
