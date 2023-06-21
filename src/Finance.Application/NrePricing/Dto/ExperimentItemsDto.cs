using Finance.NrePricing.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing.Dto
{
    /// <summary>
    /// Nre 品保部=>试验项目 录入 模型
    /// </summary>
    public class ExperimentItemsDto
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 带零件id 的 品保录入模型
        /// </summary>
        public List<ExperimentItemsModel> ExperimentItems { get; set; }
    }
    /// <summary>
    /// Nre 品保部=>试验项目 录入 模型(单个零件)
    /// </summary>
    public class ExperimentItemsSingleDto
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 带零件id 的 品保录入模型
        /// </summary>
        public ExperimentItemsModel ExperimentItems { get; set; }
    }
    /// <summary>
    /// 带零件id 的 品保部=>试验项目 录入 模型
    /// </summary>
    public class ExperimentItemsModel
    {
        /// <summary>
        /// 零件的id
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        ///品保录入  试验项目表 模型
        /// </summary>
        public List<QADepartmentTestModel> QATestDepartments { get; set; }
        /// <summary>
        /// 是否已经提交过 true/提交  false/未提交
        /// </summary>
        public bool IsSubmit { get; set; }
    }
  
}
