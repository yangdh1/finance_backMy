using Finance.NrePricing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing.Dto
{
    /// <summary>
    /// 产品部-电子工程师 录入 交互类
    /// </summary>
    public class ProductDepartmentDto
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        ///  实验费 实体类
        /// </summary>
        public List<ProductDepartmentModel> ProductDepartmentModels { get; set; }
    }
    /// <summary>
    /// 产品部-电子工程师 录入 交互类(单个零件)
    /// </summary>
    public class ProductDepartmentSingleDto
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        ///  实验费 实体类
        /// </summary>
        public ProductDepartmentModel ProductDepartmentModels { get; set; }
    }

}
