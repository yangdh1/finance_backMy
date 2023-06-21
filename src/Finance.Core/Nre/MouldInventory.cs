using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Nre
{
    /// <summary>
    /// Nre 资源部 模具清单实体类
    /// </summary>
    public class MouldInventory : FullAuditedEntity<long>
    {
        /// <summary>
        /// 流程号Id
        /// </summary> 
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 零件的id
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 结构BOM Id
        /// </summary>
        public long StructuralId { get; set; }
        /// <summary>
        /// 模具名称
        /// </summary>
        public string ModelName { get; set; }
        /// <summary>
        /// 模穴数
        /// </summary>
        public int MoldCavityCount { get; set; }
        /// <summary>
        /// 模次数
        /// </summary>
        public int ModelNumber { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// 费用
        /// </summary>
        public decimal Cost { get; set; }
    }
}
