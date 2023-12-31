﻿using Abp.Domain.Uow;
using Finance.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.EntityFrameworkCore.Seed.Host
{
    public class DefaultFinanceDictionaryCreator
    {
        private readonly FinanceDbContext _context;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public DefaultFinanceDictionaryCreator(FinanceDbContext context, IUnitOfWorkManager unitOfWorkManager)
        {
            _context = context;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public void Create()
        {
            CreateEditions();
        }
        private void CreateEditions()
        {
            if (_unitOfWorkManager is not null)
            {
                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete);
            }

            //仅把ChartPointCountName提取成配置，其他的字符串仅在程序中使用一次，不提取
            var financeDictionaryList = new List<FinanceDictionary>
            {
                new FinanceDictionary { Id=FinanceConsts.CustomerNature, DisplayName="客户性质",  },
                new FinanceDictionary { Id=FinanceConsts.Country, DisplayName="出口国家",  },

                new FinanceDictionary { Id=FinanceConsts.TerminalNature,DisplayName="终端性质",},
                new FinanceDictionary { Id=FinanceConsts.QuotationType,DisplayName="报价形式",},
                new FinanceDictionary { Id=FinanceConsts.SampleQuotationType,DisplayName="样品报价类型",},
                new FinanceDictionary { Id=FinanceConsts.Product,DisplayName="产品",},
                new FinanceDictionary { Id=FinanceConsts.ProductType,DisplayName="产品小类",},
                new FinanceDictionary { Id=FinanceConsts.AllocationOfMouldCost,DisplayName="模具费分摊",},
                new FinanceDictionary { Id=FinanceConsts.AllocationOfFixtureCost,DisplayName="治具费分摊",},
                new FinanceDictionary { Id=FinanceConsts.AllocationOfEquipmentCost,DisplayName="设备费分摊",},
                new FinanceDictionary { Id=FinanceConsts.ReliabilityCost,DisplayName="信赖性费用分摊",},
                new FinanceDictionary { Id=FinanceConsts.DevelopmentCost,DisplayName="开发费分摊",},
                new FinanceDictionary { Id=FinanceConsts.LandingFactory,DisplayName="落地工厂",},
                new FinanceDictionary { Id=FinanceConsts.TypeSelect,DisplayName="类型选择",},
                new FinanceDictionary { Id=FinanceConsts.SalesType,DisplayName="销售类型",},
                new FinanceDictionary { Id=FinanceConsts.Currency,DisplayName="报价币种",},
                new FinanceDictionary { Id=FinanceConsts.ShippingType,DisplayName="运输方式",},
                new FinanceDictionary { Id=FinanceConsts.PackagingType,DisplayName="包装方式",},
                 new FinanceDictionary { Id=FinanceConsts.NreReasons,DisplayName="NRE事由",},
                 new FinanceDictionary { Id=FinanceConsts.TradeMethod,DisplayName="贸易方式",},


            };

            var noDb = financeDictionaryList.Where(p => !_context.FinanceDictionary.Contains(p));
            if (noDb.Any())
            {
                _context.FinanceDictionary.AddRange(noDb);
                _context.SaveChanges();
            }
            var isDeleted = _context.FinanceDictionary.Where(p => financeDictionaryList.Contains(p) && p.IsDeleted).ToList();
            if (isDeleted.Any())
            {
                isDeleted.ForEach(p => p.IsDeleted = false);
                _context.FinanceDictionary.UpdateRange(isDeleted);
                _context.SaveChanges();
            }

            //////////////////

            //仅把ChartPointCountName提取成配置，其他的字符串仅在程序中使用一次，不提取
            var financeDictionaryDetailList = new List<FinanceDictionaryDetail>
            {
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TypeSelect, Id=FinanceConsts.TypeSelect_Recommend,DisplayName="我司推荐",  },
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TypeSelect, Id=FinanceConsts.TypeSelect_Appoint,DisplayName="客户指定",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TypeSelect, Id=FinanceConsts.TypeSelect_Supply,DisplayName="客户供应",},

                //客户性质
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.CustomerNature, Id=FinanceConsts.CustomerNature_CarFactory,DisplayName="车厂",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.CustomerNature, Id=FinanceConsts.CustomerNature_Tier1,DisplayName="Tier1",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.CustomerNature, Id=FinanceConsts.CustomerNature_SolutionProvider,DisplayName="方案商",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.CustomerNature, Id=FinanceConsts.CustomerNature_Platform,DisplayName="平台",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.CustomerNature, Id=FinanceConsts.CustomerNature_Algorithm,DisplayName="算法",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.CustomerNature, Id=FinanceConsts.CustomerNature_Agent,DisplayName="代理",},

                //终端性质
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TerminalNature, Id=FinanceConsts.TerminalNature_CarFactory,DisplayName="车厂",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TerminalNature, Id=FinanceConsts.TerminalNature_Tier1,DisplayName="Tier1",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TerminalNature, Id=FinanceConsts.TerminalNature_Other,DisplayName="其他",},

                //报价形式
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.QuotationType, Id=FinanceConsts.QuotationType_ProductForMassProduction ,DisplayName="量产品报价",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.QuotationType, Id=FinanceConsts.QuotationType_Sample,DisplayName="现有样品报价",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.QuotationType, Id=FinanceConsts.QuotationType_CustomizationSample,DisplayName="定制化样品报价",},

                 //样品报价类型
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.SampleQuotationType, Id=FinanceConsts.SampleQuotationType_Existing ,DisplayName="现有样品",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.SampleQuotationType, Id=FinanceConsts.SampleQuotationType_Customization,DisplayName="定制化样品",},

                //落地工厂
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.LandingFactory, Id=FinanceConsts.LandingFactory_SunnySmartLead,DisplayName="舜宇智领",},

                //销售类型
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.SalesType, Id=FinanceConsts.SalesType_ForTheDomesticMarket, DisplayName="内销",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.SalesType, Id=FinanceConsts.SalesType_ForExport, DisplayName="外销",},

                //出口国家
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.Country, Id=FinanceConsts.Country_Iran, DisplayName="伊朗",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.Country, Id=FinanceConsts.Country_NorthKorea, DisplayName="朝鲜",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.Country, Id=FinanceConsts.Country_Syria, DisplayName="叙利亚",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.Country, Id=FinanceConsts.Country_Cuba, DisplayName="古巴",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.Country, Id=FinanceConsts.Country_Other, DisplayName="其他国家",},

                //运输方式
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.ShippingType, Id=FinanceConsts.ShippingType_LandTransportation, DisplayName="陆运",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.ShippingType, Id=FinanceConsts.ShippingType_OceanShipping, DisplayName="海运",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.ShippingType, Id=FinanceConsts.ShippingType_AirTransport, DisplayName="空运",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.ShippingType, Id=FinanceConsts.ShippingType_HandPick, DisplayName="手提取货",},


                //包装方式 
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.PackagingType, Id=FinanceConsts.PackagingType_TurnoverBox, DisplayName="周转箱",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.PackagingType, Id=FinanceConsts.PackagingType_DisposableCarton, DisplayName="一次性纸箱",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.PackagingType, Id=FinanceConsts.PackagingType_NoSpecialRequirements, DisplayName="客户无特殊要求",},

                //产品小类
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.ProductType, Id=FinanceConsts.ProductType_ExternalImaging, DisplayName="外摄显像",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.ProductType, Id=FinanceConsts.ProductType_EnvironmentalPerception, DisplayName="环境感知",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.ProductType, Id=FinanceConsts.ProductType_CabinMonitoring, DisplayName="舱内监测",},

                //NRE事由
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.NreReasons, Id=FinanceConsts.NreReasons_DepotWithLine, DisplayName="车厂跟线",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.NreReasons, Id=FinanceConsts.NreReasons_ProjectCommunication, DisplayName="项目交流",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.NreReasons, Id=FinanceConsts.NreReasons_ClientCommunication, DisplayName="客户端交流",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.NreReasons, Id=FinanceConsts.NreReasons_Else, DisplayName="其他",},

                //贸易方式
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TradeMethod, Id=FinanceConsts.TradeMethodEXW, DisplayName="EXW",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TradeMethod, Id=FinanceConsts.TradeMethodFCA, DisplayName="FCA",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TradeMethod, Id=FinanceConsts.TradeMethodFAS, DisplayName="FAS",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TradeMethod, Id=FinanceConsts.TradeMethodFOB, DisplayName="FOB",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TradeMethod, Id=FinanceConsts.TradeMethodCFR, DisplayName="CFR",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TradeMethod, Id=FinanceConsts.TradeMethodCIF, DisplayName="CIF",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TradeMethod, Id=FinanceConsts.TradeMethodCPT, DisplayName="CPT",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TradeMethod, Id=FinanceConsts.TradeMethodCIP, DisplayName="CIP",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TradeMethod, Id=FinanceConsts.TradeMethodDAF, DisplayName="DAF",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TradeMethod, Id=FinanceConsts.TradeMethodDES, DisplayName="DES",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TradeMethod, Id=FinanceConsts.TradeMethodDEQ, DisplayName="DEQ",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TradeMethod, Id=FinanceConsts.TradeMethodDDU, DisplayName="DDU",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TradeMethod, Id=FinanceConsts.TradeMethodDDP, DisplayName="DDP",},
            };

            var noDbDetail = financeDictionaryDetailList.Where(p => !_context.FinanceDictionaryDetail.Contains(p));
            if (noDbDetail.Any())
            {
                _context.FinanceDictionaryDetail.AddRange(noDbDetail);
                _context.SaveChanges();
            }
            var isDeletedDetail = _context.FinanceDictionaryDetail.Where(p => financeDictionaryDetailList.Contains(p) && p.IsDeleted).ToList();
            if (isDeletedDetail.Any())
            {
                isDeletedDetail.ForEach(p => p.IsDeleted = false);
                _context.FinanceDictionaryDetail.UpdateRange(isDeletedDetail);
                _context.SaveChanges();
            }
        }
    }
}
