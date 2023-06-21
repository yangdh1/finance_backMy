using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.UnitPriceLibrary.Dto
{
    /// <summary>
    /// 基础单价表 交互类
    /// </summary>
    public class UInitPriceFormDto : FullAuditedEntity<long>
    {
        /// <summary>
        /// 单据号
        /// </summary>
        
        public virtual string DorderNumber { get; set; }
        /// <summary>
        /// 凭据号
        /// </summary>
        public virtual string VoucherNumber { get; set; }
        /// <summary>
        /// 单据状态
        /// </summary>
        public virtual string BillsState { get; set; }
        /// <summary>
        /// 采购分类
        /// </summary>
        public virtual string PurchaseClassify { get; set; }
        /// <summary>
        /// 供应商优先级
        /// </summary>     
        public virtual string Priority { get; set; }
        /// <summary>
        /// 供应商编号
        /// </summary>
        public virtual string SupplierCode { get; set; }
        /// <summary>
        /// 供应商名称
        /// </summary>
        public virtual string SupplierName { get; set; }
        /// <summary>
        /// 物料编号
        /// </summary>
        public virtual string StockNumber { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public virtual string StockName { get; set; }
        /// <summary>
        /// 工厂
        /// </summary>
        public virtual string Plant { get; set; }
        /// <summary>
        /// 工厂图号
        /// </summary>
        public virtual string PlantNumber { get; set; }
        /// <summary>
        /// 税码
        /// </summary>
        public virtual string TaxCode { get; set; }
        /// <summary>
        /// 货币代码
        /// </summary>
        public virtual string Currencies { get; set; }
        /// <summary>
        /// 原净价
        /// </summary>
        public virtual string RowNetPrice { get; set; }
        /// <summary>
        /// 总净价(不含税)
        /// </summary>
        public virtual string SumNetPrice { get; set; }
        /// <summary>
        /// 返利类型
        /// </summary>
        public virtual string RebatesType { get; set; }
        /// <summary>
        /// 返利金额
        /// </summary>
        public virtual string RebateAmount { get; set; }
        /// <summary>
        /// 净价
        /// </summary>
        
        public virtual decimal NetPrice { get; set; }
        /// <summary>
        /// 税率
        /// </summary>
        public virtual string TaxRate { get; set; }
        /// <summary>
        /// 含税价
        /// </summary>
        public virtual string PriceIncludingTax { get; set; }
        /// <summary>
        /// 价格单位
        /// </summary>
        public virtual string PriceUnit { get; set; }
        /// <summary>
        /// 内部核算价
        /// </summary>
        public virtual string InternalValuation { get; set; }
        /// <summary>
        /// 供应商报价
        /// </summary>   
        public virtual string VendorOffer { get; set; }
        /// <summary>
        /// 有效开始日期
        /// </summary>
        public virtual DateTime EffectiveDate { get; set; }
        /// <summary>
        /// 有效结束日期
        /// </summary>
        public virtual DateTime EffectiveEndDate { get; set; }
        /// <summary>
        /// 价格变动类型
        /// </summary>
        public virtual string TypePriceChange { get; set; }
        /// <summary>
        /// 是否客户指定
        /// </summary>
        public virtual string IfClientSpecify { get; set; }
        /// <summary>
        /// 库存单位
        /// </summary>
        public virtual string SKU { get; set; }
        /// <summary>
        /// 采购单位
        /// </summary>
        public virtual string PUOM { get; set; }
        /// <summary>
        /// 货期
        /// </summary>
        public virtual string Delivery { get; set; }
        /// <summary>
        /// 允许超收
        /// </summary>
        public virtual string AllowExcess { get; set; }
        /// <summary>
        /// 更改原因说明
        /// </summary>
        public virtual string ReasonForChange { get; set; }
        /// <summary>
        /// 采购组织
        /// </summary>
        public virtual string PurchaseOrg { get; set; }
        /// <summary>
        /// 采购组
        /// </summary>
        public virtual string ProcurementSection { get; set; }
        /// <summary>
        /// 报价单类型
        /// </summary>
        public virtual string TypeQuotation { get; set; }
        /// <summary>
        /// 变动类型
        /// </summary>
        public virtual string ChangeType { get; set; }
        /// <summary>
        /// /所属部门
        /// </summary>
        public virtual string TheirDepartment { get; set; }
        /// <summary>
        /// 登记人
        /// </summary>
        public virtual string Registrant { get; set; }
        /// <summary>
        /// 登记日期
        /// </summary>
        public virtual DateTime RecordDate { get; set; }
        /// <summary>
        /// 冻结状态
        /// </summary>
        public virtual bool FrozenState { get; set; }
        /// <summary>
        /// 下限
        /// </summary>
        public virtual decimal Floor { get; set; }
        /// <summary>
        /// 上限
        /// </summary>
        public virtual decimal Upper { get; set; }
        /// <summary>
        /// 总净价（不含税）
        /// </summary>
        public virtual string NetAmount { get; set; }
        /// <summary>
        /// 最新变动日期
        /// </summary>
        public virtual DateTime DateLastChange { get; set; }
        /// <summary>
        /// ECCN码
        /// </summary>
        public virtual string ECCNCode { get; set; }
        /// <summary>
        /// 冻结状态显示
        /// </summary>     
        public virtual string DisplayFrozenState { get; set; }
    }
}
