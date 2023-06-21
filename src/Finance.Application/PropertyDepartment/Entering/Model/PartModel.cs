using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.Entering.Model
{
    /// <summary>
    /// 零件模组 模型
    /// </summary>
    public class PartModel
    {
        /// <summary>
        /// 零件的id
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 零件的名称
        /// </summary>
        public string ParName { get; set; }
    }
}
