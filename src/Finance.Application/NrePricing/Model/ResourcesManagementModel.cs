using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing.Model
{
    /// <summary>
    /// Nre核价  资源部录入 模型
    /// </summary>
    public class ResourcesManagementModel
    {
        /// <summary>
        /// 零件的id
        /// </summary>
        public long ProductId { get; set; } 
        /// <summary>
        /// 模具清单
        /// </summary>
        public List<MouldInventoryModel> MouldInventory { get; set; }
    }
}
