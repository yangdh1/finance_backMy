using Finance.Debugging;

namespace Finance
{
    public class FinanceConsts
    {
        public const string LocalizationSourceName = "Finance";

        public const string ConnectionStringName = "Default";

        public const int Mail_Password_Date = 80;     //距离上一次修改密码多少天进行提醒        

        public const int SmtpPort_Sunny = 25;          //SMTP服务器端口
        public const string SmtpServer_Sunny = "mail.sunnyoptical.com"; //SMTP服务器
        public const string MailFrom_Sunny = "zlcwhjbjxt@sunnyoptical.com"; //登陆用户名，邮箱
        public const string MailUserPassword_Sunny = "mb395$BQ";//登录密码

        public const string MailForMaintainer = "wslinzhang@sunnyoptical.com";

        public const string AliServer_In_IP = "172.26.144.105"; //阿里云内网IP地址
        public const string AliServer_Out_IP = "139.196.216.165"; //阿里云外网IP地址
        
        public const int SmtpPort_Tencent = 587;    //SMTP服务器端口
        public const string SmtpServer_Tencent = "smtp.qq.com"; //SMTP服务器
        public const string MailFrom_Tencent = "274439023@qq.com"; //登陆用户名，邮箱
        public const string MailUserPassword_Tencent = "ynsmqsxldqdmcaid";//授权码

        public const bool MultiTenancyEnabled = false;
        public const int DefaultPageSize = 20;
        public const int MaxPageSize = 1000;

        public const int MinYear = 1000;
        public const int MaxYear = 3000;
        public const string Sorting = "Id Desc";

        public const string SystemLogFileType = ".csv";

        public const string ElectronicName = "电子料";
        public const string StructuralName = "结构料";
        public const string GlueMaterialName = "胶水等辅材";
        public const string SMTOutSourceName = "SMT外协";
        public const string PackingMaterialName = "包材";

        public const string EccnCode_Eccn = "ECCN";
        public const string EccnCode_Ear99 = "EAR99";
        public const string EccnCode_Uninvolved = "不涉及";
        public const string EccnCode_Pending = "待定";

        /// <summary>
        /// 部门PathId的生成规则
        /// </summary>
        public const string DepartmentPathIdRegular = "{0}.{1}";

        /// <summary>
        /// 部门PathName的生成规则
        /// </summary>
        public const string DepartmentPathNameRegular = $"{{0}}{DepartmentNameSeparator}{{1}}";


        /// <summary>
        /// 部门名称的正则表达式，用来验证是否包含/
        /// </summary>
        public const string DepartmentNameRegular = $"^((?!{DepartmentNameSeparator}).)*$";

        /// <summary>
        /// 部门Name的分隔符
        /// </summary>
        public const string DepartmentNameSeparator = "/";

        /// <summary>
        /// 字典名称的正则表达式，用来验证是否包含-
        /// </summary>
        public const string FinanceDictionaryNameRegular = $"^((?!{FinanceDictionaryNameSeparator}).)*$";

        /// <summary>
        /// 字典名的分隔符
        /// </summary>
        public const string FinanceDictionaryNameSeparator = "-";

        /// <summary>
        /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public static readonly string DefaultPassPhrase =
            DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "f58393d400854e16be45a08409982c15";


        #region 核价需求录入
        /// <summary>
        /// 客户性质（字典表的Name）
        /// </summary>
        public const string CustomerNature = "CustomerNature";

        /// <summary>
        /// 客户国家（字典表的Name）
        /// </summary>
        public const string Country = "Country";

        /// <summary>
        /// 客户国家之伊朗（字典明细表的Name）
        /// </summary>
        public const string Country_Iran = "Country_Iran";

        /// <summary>
        /// 客户国家之朝鲜（字典明细表的Name）
        /// </summary>
        public const string Country_NorthKorea = "Country_NorthKorea";

        /// <summary>
        /// 客户国家之叙利亚（字典明细表的Name）
        /// </summary>
        public const string Country_Syria = "Country_Syria";

        /// <summary>
        /// 客户国家之古巴（字典明细表的Name）
        /// </summary>
        public const string Country_Cuba = "Country_Cuba";

        /// <summary>
        /// 客户国家之其他国家（字典明细表的Name）
        /// </summary>
        public const string Country_Other = "Country_Other";

        /// <summary>
        /// 车厂（字典明细表的Name）
        /// </summary>
        public const string CustomerNature_CarFactory = "CustomerNature_CarFactory";

        /// <summary>
        /// Tier1（字典明细表的Name）
        /// </summary>
        public const string CustomerNature_Tier1 = "CustomerNature_Tier1";

        /// <summary>
        /// 方案商（字典明细表的Name）
        /// </summary>
        public const string CustomerNature_SolutionProvider = "CustomerNature_SolutionProvider";

        /// <summary>
        /// 平台（字典明细表的Name）
        /// </summary>
        public const string CustomerNature_Platform = "CustomerNature_Platform";

        /// <summary>
        /// 算法（字典明细表的Name）
        /// </summary>
        public const string CustomerNature_Algorithm = "CustomerNature_Algorithm";

        /// <summary>
        /// 代理（字典明细表的Name）
        /// </summary>
        public const string CustomerNature_Agent = "CustomerNature_Agent";
        

        /// <summary>
        /// 终端性质（字典表的Name）
        /// </summary>
        public const string TerminalNature = "TerminalNature";

        /// <summary>
        /// 终端性质之车厂（字典明细表的Name）
        /// </summary>
        public const string TerminalNature_CarFactory = "TerminalNature_CarFactory";

        /// <summary>
        /// 终端性质之Tier1（字典明细表的Name）
        /// </summary>
        public const string TerminalNature_Tier1 = "TerminalNature_Tier1";

        /// <summary>
        /// 终端性质之Other（字典明细表的Name）
        /// </summary>
        public const string TerminalNature_Other = "TerminalNature_Other";

        /// <summary>
        /// 报价形式（字典表的Name）
        /// </summary>
        public const string QuotationType = "QuotationType";

        /// <summary>
        /// 报价形式之量产品报价（字典明细表的Name）
        /// </summary>
        public const string QuotationType_ProductForMassProduction = "QuotationType_ProductForMassProduction";

        /// <summary>
        /// 报价形式之样品报价（字典明细表的Name）
        /// </summary>
        public const string QuotationType_Sample = "QuotationType_Sample";

        /// <summary>
        /// 报价形式之定制化样品报价（字典明细表的Name）
        /// </summary>
        public const string QuotationType_CustomizationSample = "QuotationType_CustomizationSample";

        /// <summary>
        /// 样品报价类型（字典表的Name）
        /// </summary>
        public const string SampleQuotationType = "SampleQuotationType";

        /// <summary>
        /// 样品报价类型之现有样品（字典明细表的Name）
        /// </summary>
        public const string SampleQuotationType_Existing = "SampleQuotationType_Existing";

        /// <summary>
        /// 样品报价类型之定制化样品（字典明细表的Name）
        /// </summary>
        public const string SampleQuotationType_Customization = "SampleQuotationType_Customization";

        /// <summary>
        /// 产品（字典表的Name）
        /// </summary>
        public const string Product = "Product";

        /// <summary>
        /// 产品小类（字典表的Name）
        /// </summary>
        public const string ProductType = "ProductType";

        /// <summary>
        /// 产品小类之外摄显像（字典明细表的Name）
        /// </summary>
        public const string ProductType_ExternalImaging = "ProductType_ExternalImaging";

        /// <summary>
        /// 产品小类之环境感知（字典明细表的Name）
        /// </summary>
        public const string ProductType_EnvironmentalPerception = "ProductType_EnvironmentalPerception";

        /// <summary>
        /// 产品小类之舱内监测（字典明细表的Name）
        /// </summary>
        public const string ProductType_CabinMonitoring = "ProductType_CabinMonitoring";

        /// <summary>
        /// 模具费分摊（字典表的Name）
        /// </summary>
        public const string AllocationOfMouldCost = "AllocationOfMouldCost";

        /// <summary>
        /// 治具费分摊（字典表的Name）
        /// </summary>
        public const string AllocationOfFixtureCost = "AllocationOfFixtureCost";

        /// <summary>
        /// 设备费分摊（字典表的Name）
        /// </summary>
        public const string AllocationOfEquipmentCost = "AllocationOfEquipmentCost";

        /// <summary>
        /// 信赖性费用分摊（字典表的Name）
        /// </summary>
        public const string ReliabilityCost = "ReliabilityCost";


        /// <summary>
        /// 开发费分摊（字典表的Name）
        /// </summary>
        public const string DevelopmentCost = "DevelopmentCost";

        /// <summary>
        /// 落地工厂（字典表的Name）
        /// </summary>
        public const string LandingFactory = "LandingFactory";


        /// <summary>
        /// 落地工厂之舜宇智领（字典明细表的Name）
        /// </summary>
        public const string LandingFactory_SunnySmartLead = "LandingFactory_SunnySmartLead";

        #region 类型选择
        /// <summary>
        /// 类型选择（字典表的Name）
        /// </summary>
        public const string TypeSelect = "TypeSelect";

        /// <summary>
        /// 类型选择之我司推荐（字典明细表的Name）
        /// </summary>
        public const string TypeSelect_Recommend = "TypeSelect_Recommend";

        /// <summary>
        /// 类型选择之客户指定（字典明细表的Name）
        /// </summary>
        public const string TypeSelect_Appoint = "TypeSelect_Appoint";

        /// <summary>
        /// 类型选择之客户供应（字典明细表的Name）
        /// </summary>
        public const string TypeSelect_Supply = "TypeSelect_Supply";
        #endregion

        /// <summary>
        /// 销售类型（字典表的Name）
        /// </summary>
        public const string SalesType = "SalesType";

        /// <summary>
        /// 销售类型之内销（字典明细表的Name）
        /// </summary>
        public const string SalesType_ForTheDomesticMarket = "SalesType_ForTheDomesticMarket";

        /// <summary>
        /// 销售类型之外销（字典明细表的Name）
        /// </summary>
        public const string SalesType_ForExport = "SalesType_ForExport";

        /// <summary>
        /// 报价币种（字典表的Name）
        /// </summary>
        public const string Currency = "Currency";

        /// <summary>
        /// 运输方式（字典表的Name）
        /// </summary>
        public const string ShippingType = "ShippingType";

        /// <summary>
        /// 运输方式之陆运（字典明细表的Name）
        /// </summary>
        public const string ShippingType_LandTransportation = "ShippingType_LandTransportation";

        /// <summary>
        /// 运输方式之海运（字典明细表的Name）
        /// </summary>
        public const string ShippingType_OceanShipping = "ShippingType_OceanShipping";

        /// <summary>
        /// 运输方式之空运（字典明细表的Name）
        /// </summary>
        public const string ShippingType_AirTransport = "ShippingType_AirTransport";

        /// <summary>
        /// 运输方式之手提取货（字典明细表的Name）
        /// </summary>
        public const string ShippingType_HandPick = "ShippingType_HandPick";


        /// <summary>
        /// 包装方式（字典表的Name）
        /// </summary>
        public const string PackagingType = "PackagingType";

        /// <summary>
        /// 包装方式之周转箱（字典明细表的Name）
        /// </summary>
        public const string PackagingType_TurnoverBox = "PackagingType_TurnoverBox";

        /// <summary>
        /// 包装方式之一次性纸箱（字典明细表的Name）
        /// </summary>
        public const string PackagingType_DisposableCarton = "PackagingType_DisposableCarton";

        /// <summary>
        /// 包装方式之客户无特殊要求（字典明细表的Name）
        /// </summary>
        public const string PackagingType_NoSpecialRequirements = "PackagingType_NoSpecialRequirements";

        #endregion
        //Nre 核价 项目管理部  下的 差旅费 的事由下拉
        #region
        /// <summary>
        /// NRE字典表  事由
        /// </summary>
        public const string NreReasons = "NreReasons";
        /// <summary>
        /// 字典明细表事由->车厂跟线
        /// </summary>
        public const string NreReasons_DepotWithLine = "NreReaso1ns_DepotWithLine";
        /// <summary>
        /// 字典明细表事由->项目交流
        /// </summary>
        public const string NreReasons_ProjectCommunication = "NreReaso1ns_ProjectCommunication";
        /// <summary>
        /// 字典明细表事由->客户端交流
        /// </summary>
        public const string NreReasons_ClientCommunication = "NreReasons_ClientCommunication";
        /// <summary>
        /// 字典明细表事由->其他
        /// </summary>
        public const string NreReasons_Else = "NreReaso1ns_Else";

        /// <summary>
        /// 贸易方式
        /// </summary>
        public const string TradeMethod = "TradeMethod";

        /// <summary>
        /// 贸易方式-EXW
        /// </summary>
        public const string TradeMethodEXW = "TradeMethodEXW";

        /// <summary>
        /// 贸易方式-FCA
        /// </summary>
        public const string TradeMethodFCA = "TradeMethodFCA";

        /// <summary>
        /// 贸易方式-FAS
        /// </summary>
        public const string TradeMethodFAS = "TradeMethodFAS";

        /// <summary>
        /// 贸易方式-FOB
        /// </summary>
        public const string TradeMethodFOB = "TradeMethodFOB";

        /// <summary>
        /// 贸易方式-CFR
        /// </summary>
        public const string TradeMethodCFR = "TradeMethodCFR";

        /// <summary>
        /// 贸易方式-CIF
        /// </summary>
        public const string TradeMethodCIF = "TradeMethodCIF";

        /// <summary>
        /// 贸易方式-CPT
        /// </summary>
        public const string TradeMethodCPT = "TradeMethodCPT";

        /// <summary>
        /// 贸易方式-CIP
        /// </summary>
        public const string TradeMethodCIP = "TradeMethodCIP";

        /// <summary>
        /// 贸易方式-DAF
        /// </summary>
        public const string TradeMethodDAF = "TradeMethodDAF";

        /// <summary>
        /// 贸易方式-DES
        /// </summary>
        public const string TradeMethodDES = "TradeMethodDES";

        /// <summary>
        /// 贸易方式-DEQ
        /// </summary>
        public const string TradeMethodDEQ = "TradeMethodDEQ";

        /// <summary>
        /// 贸易方式-DDU
        /// </summary>
        public const string TradeMethodDDU = "TradeMethodDDU";

        /// <summary>
        /// 贸易方式-DDP
        /// </summary>
        public const string TradeMethodDDP = "TradeMethodDDP";


        #endregion
    }
}
