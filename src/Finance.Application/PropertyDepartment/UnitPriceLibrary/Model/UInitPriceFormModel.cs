using Abp.Domain.Entities.Auditing;
using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.UnitPriceLibrary.Model
{
    /// <summary>
    /// 基础单价表 模型
    /// </summary>
    public class UInitPriceFormModel  
    {
        /// <summary>
        /// 单据号
        /// </summary>
        [ExcelColumnName("单据号")]
        public virtual string DorderNumber { get; set; }
        /// <summary>
        /// 凭据号
        /// </summary>
        [ExcelColumnName("凭证号")]
        public virtual string VoucherNumber { get; set; }
        /// <summary>
        /// 单据状态
        /// </summary>
        [ExcelColumnName("单据状态")]
        public virtual string BillsState { get; set; }
        /// <summary>
        /// 采购分类
        /// </summary>
        [ExcelColumnName("采购分类")]
        public virtual string PurchaseClassify { get; set; }
        /// <summary>
        /// 供应商优先级
        /// </summary>
        [ExcelColumnName("供应商优先级")]
        public virtual string Priority { get; set; } 
        /// <summary>
        /// 供应商编号
        /// </summary>
        [ExcelColumnName("供应商编号")]
        public virtual string SupplierCode { get; set; }
        /// <summary>
        /// 供应商名称
        /// </summary>
        [ExcelColumnName("供应商名称")]
        public virtual string SupplierName { get; set; }
        /// <summary>
        /// 物料编号
        /// </summary>
        [ExcelColumnName("物料编号")]
        public virtual string StockNumber { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        [ExcelColumnName("物料名称")]
        public virtual string StockName { get; set; }
        /// <summary>
        /// 工厂
        /// </summary>
        [ExcelColumnName("工厂")]
        public virtual string Plant { get; set; }
        /// <summary>
        /// 工厂图号
        /// </summary>
        [ExcelColumnName("工厂图号")]
        public virtual string PlantNumber { get; set; }
        /// <summary>
        /// 税码
        /// </summary>
        [ExcelColumnName("税码")]
        public virtual string TaxCode { get; set; }
        /// <summary>
        /// 货币代码
        /// </summary>
        [ExcelColumnName("货币代码")]
        public virtual string Currencies { get; set; }
        /// <summary>
        /// 原净价
        /// </summary>
        [ExcelColumnName("原净价")]
        public virtual string RowNetPrice { get; set; }
        /// <summary>
        /// 总净价(不含税)
        /// </summary>
        [ExcelColumnName("总净价(不含税)")]
        public virtual string SumNetPrice { get; set; }
        /// <summary>
        /// 返利类型
        /// </summary>
        [ExcelColumnName("返利类型")]
        public virtual string RebatesType { get; set; }
        /// <summary>
        /// 返利金额
        /// </summary>
        [ExcelColumnIndex("Q")]
        public virtual string RebateAmount { get; set; }
        /// <summary>
        /// 净价
        /// </summary>
        [ExcelColumnIndex("R")]
        public virtual decimal NetPrice { get; set; }
        /// <summary>
        /// 税率
        /// </summary>
        [ExcelColumnName("税率")]
        public virtual string TaxRate { get; set; }
        /// <summary>
        /// 含税价
        /// </summary>
        [ExcelColumnName("含税价")]
        public virtual string PriceIncludingTax { get; set; }
        /// <summary>
        /// 价格单位
        /// </summary>
        [ExcelColumnName("价格单位")]
        public virtual string PriceUnit { get; set; }
        /// <summary>
        /// 内部核算价
        /// </summary>
        [ExcelColumnName("内部核算价")]
        public virtual string InternalValuation { get; set; }
        /// <summary>
        /// 供应商报价
        /// </summary>      
        [ExcelColumnName("供应商报价")]
        public virtual string VendorOffer { get; set; }
        /// <summary>
        /// 有效开始日期
        /// </summary>
        [ExcelFormat("yyyy-MM-dd")]
        [ExcelColumnName("有效开始日期")]
        public virtual DateTime EffectiveDate { get; set; }
        /// <summary>
        /// 有效结束日期
        /// </summary>
        [ExcelFormat("yyyy-MM-dd")]
        [ExcelColumnName("有效结束日期")]
        public virtual DateTime EffectiveEndDate { get; set; }
        /// <summary>
        /// 价格变动类型
        /// </summary>
        [ExcelColumnName("价格变动类型")]
        public virtual string TypePriceChange { get; set; }
        /// <summary>
        /// 是否客户指定
        /// </summary>
        [ExcelColumnName("是否客户指定")]
        public virtual string IfClientSpecify { get; set; }
        /// <summary>
        /// 库存单位
        /// </summary>
        [ExcelColumnName("库存单位")]
        public virtual string SKU { get; set; }
        /// <summary>
        /// 采购单位
        /// </summary>
        [ExcelColumnName("采购单位")]
        public virtual string PUOM { get; set; }
        /// <summary>
        /// 货期
        /// </summary>
        [ExcelColumnName("货期")]
        public virtual string Delivery { get; set; }
        /// <summary>
        /// 允许超收
        /// </summary>
        [ExcelColumnName("允许超收")]
        public virtual string AllowExcess { get; set; }
        /// <summary>
        /// 更改原因说明
        /// </summary>
        [ExcelColumnName("更改原因说明")]
        public virtual string ReasonForChange { get; set; }
        /// <summary>
        /// 采购组织
        /// </summary>
        [ExcelColumnName("采购组织")]
        public virtual string PurchaseOrg { get; set; }
        /// <summary>
        /// 采购组
        /// </summary>
        [ExcelColumnName("采购组")]
        public virtual string ProcurementSection { get; set; }
        /// <summary>
        /// 报价单类型
        /// </summary>
        [ExcelColumnName("报价单类型")]
        public virtual string TypeQuotation { get; set; }
        /// <summary>
        /// 变动类型
        /// </summary>
        [ExcelColumnName("变动类型")]
        public virtual string ChangeType { get; set; }
        /// <summary>
        /// /所属部门
        /// </summary>
        [ExcelColumnName("所属部门")]
        public virtual string TheirDepartment { get; set; }
        /// <summary>
        /// 登记人
        /// </summary>
        [ExcelColumnName("登记人")]
        public virtual string Registrant { get; set; }
        /// <summary>
        /// 登记日期
        /// </summary>
        [ExcelFormat("yyyy-MM-dd")]
        [ExcelColumnName("登记日期")]
        public virtual DateTime RecordDate { get; set; }
        /// <summary>
        /// 冻结状态
        /// </summary>
        [ExcelColumnName("冻结状态")]
        public virtual string FrozenState { get; set; }
        /// <summary>
        /// 下限
        /// </summary>
        [ExcelColumnName("下限")]

        public virtual string Floor { get; set; }
        /// <summary>
        /// 上限
        /// </summary>
        [ExcelColumnName("上限")]
        public virtual string Upper { get; set; }
        /// <summary>
        /// 总净价（不含税）
        /// </summary>
        [ExcelColumnName("总净价（不含税）")]
        public virtual string NetAmount { get; set; }
        /// <summary>
        /// 最新变动日期
        /// </summary>
        [ExcelFormat("yyyy-MM-dd")]
        [ExcelColumnName("最新变动日期")]
        public virtual DateTime DateLastChange { get; set; }
        /// <summary>
        /// ECNN码
        /// </summary>
        [ExcelColumnName("ECCN码")]
        public virtual string ECCNCode { get; set; }

    }

}
