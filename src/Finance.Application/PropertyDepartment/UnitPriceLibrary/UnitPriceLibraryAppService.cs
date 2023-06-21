using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using Finance.Dto;
using Finance.EntityFrameworkCore;
using Finance.FinanceMaintain;
using Finance.PropertyDepartment.UnitPriceLibrary.Dto;
using Finance.PropertyDepartment.UnitPriceLibrary.Model;
using Finance.Users.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using Newtonsoft.Json;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.UnitPriceLibrary
{
    /// <summary>
    /// 
    /// </summary>
    public class UnitPriceLibraryAppService : ApplicationService, IUnitPriceLibraryAppService
    {

        /// <summary>
        /// 基础单价库实体类
        /// </summary>
        private readonly IRepository<UInitPriceForm, long> _configUInitPriceForm;
        /// <summary>
        ///  财务维护 毛利率方案
        /// </summary>
        private readonly IRepository<GrossMarginForm, long> _configGrossMarginForm;
        /// <summary>
        ///  财务维护 汇率表
        /// </summary>
        private readonly IRepository<ExchangeRate, long> _configExchangeRate;
        /// <summary>
        /// 数据上下文提供者
        /// </summary>
        private readonly IDbContextProvider<FinanceDbContext> _provider;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configUInitPriceForm"></param>
        /// <param name="configGrossMarginForm"></param>
        /// <param name="configExchangeRate"></param>
        /// <param name="provider"></param>
        public UnitPriceLibraryAppService(IRepository<UInitPriceForm, long> configUInitPriceForm, IRepository<GrossMarginForm, long> configGrossMarginForm, IRepository<ExchangeRate, long> configExchangeRate, IDbContextProvider<FinanceDbContext> provider)
        {
            _configUInitPriceForm = configUInitPriceForm;
            _configGrossMarginForm = configGrossMarginForm;
            _configExchangeRate = configExchangeRate;
            _provider = provider;
        }



        /// <summary>
        /// 查询单价库信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<UInitPriceFormDto>> GetGainUInitPriceForm(GainUInitPriceInputDto input)
        {
            try
            {             
                //定义列表查询
                var filter = _configUInitPriceForm.GetAll().WhereIf(!input.Filter.IsNullOrEmpty(), p => p.DorderNumber.Contains(input.Filter)||p.StockNumber.Contains(input.Filter));
                var pagedSorted = filter.OrderByDescending(p => p.Id).PageBy(input);
                //获取总数
                var count = await filter.CountAsync();
                //获取查询结果
                List<UInitPriceForm> result = await pagedSorted.ToListAsync();
                return new PagedResultDto<UInitPriceFormDto>(count, ObjectMapper.Map<List<UInitPriceFormDto>>(result));
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 添加 (刷新整个基础单价库)
        /// </summary>
        /// <param Name="stationBind"></param>
        /// <returns></returns>
        public async Task PostUInitPriceForm(IFormFile file)
        {

            long size = file.Length;
            using (var memoryStream = new MemoryStream())
            {
                string[] suffix = file.FileName.Split(".");
                if (suffix.Length < 1)
                {
                    throw new FriendlyException("文件名错误");
                }
                await file.CopyToAsync(memoryStream);
                //载入xls文档
                Workbook workbook = new Workbook();
                if (suffix[suffix.Length - 1].Equals("xls"))
                {
                    string templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\wwwroot\Excel\基础单价库.xlsx";
                    workbook.LoadFromStream(memoryStream);
                    //保存为xlsx格式
                    workbook.SaveToFile(templatePath, ExcelVersion.Version2016);
                    using (FileStream fs = File.OpenRead(templatePath))
                    {
                        memoryStream.SetLength(0);
                        await fs.CopyToAsync(memoryStream);
                    }
                }
                if (memoryStream.Length < 512 * 1048576)
                {
                    List<UInitPriceFormModel> rows = memoryStream.Query<UInitPriceFormModel>(startCell: "A2").ToList();
                    #region excel校验
                    //校验供应商优先级
                    var priority = rows.Where(p => p.Priority.IsNullOrWhiteSpace()).Select(p => $"文件第{rows.IndexOf(p) + 2}行, 供应商优先级为空。");
                    //校验物料编号
                    var stockNumbe = rows.Where(p => p.StockNumber.IsNullOrWhiteSpace()).Select(p => $"文件第{rows.IndexOf(p) + 2}行, 物料编号为空。");
                    //校验货币代码
                    var currencies = rows.Where(p => p.Currencies.IsNullOrWhiteSpace()).Select(p => $"文件第{rows.IndexOf(p) + 2}行,货币代码为空。");
                    //校验冻结状态
                    var frozenState = rows.Where(p => p.FrozenState.IsNullOrWhiteSpace()).Select(p => $"文件第{rows.IndexOf(p) + 2}行,冻结状态为空。");
                    //校验ECNN码
                    var eCCNCode = rows.Where(p => p.ECCNCode.IsNullOrWhiteSpace()).Select(p => $"文件第{rows.IndexOf(p) + 2}行,ECCN码为空。");
                    var failMessage = priority.Union(stockNumbe).Union(currencies).Union(frozenState).Union(eCCNCode);
                    if (failMessage.Any())
                    {
                        throw new FriendlyException($"{string.Join("\r\n", failMessage)}");
                    }
                    #endregion
                    try
                    {
                        int AllCount = await _configUInitPriceForm.CountAsync();
                        if (AllCount is not 0)
                        {
                            await _configUInitPriceForm.GetDbContext().Database.ExecuteSqlRawAsync("DELETE FROM \"UInitPriceForm\"");
                        }
                        List<UInitPriceForm> price = ObjectMapper.Map<List<UInitPriceForm>>(rows);
                        //await _configUInitPriceForm.GetDbContext().BulkInsertAsync(price);                        
                        _configUInitPriceForm.GetDbContext().Set<UInitPriceForm>().AddRange(price);
                        _configUInitPriceForm.GetDbContext().SaveChanges();
                    }
                    catch (Exception e)
                    {
                        throw new UserFriendlyException(e.Message);
                    }
                }
                else
                {
                    throw new FriendlyException("文件过大");
                }
            }
        }

        /// <summary>
        /// 查询毛利率方案(查询依据 GrossMarginName)
        /// </summary>
        /// <returns></returns>
        public async Task<PagedResultDto<GrossMarginDto>> GetGrossMargin(GrossMarginInputDto input)
        {
            try
            {
                //定义列表查询
                var filter = _configGrossMarginForm.GetAll().WhereIf(!input.GrossMarginName.IsNullOrEmpty(), p => p.GrossMarginName.Contains(input.GrossMarginName));
                var pagedSorted = filter.OrderByDescending(p => p.Id).PageBy(input);
                //获取总数
                var count = await filter.CountAsync();
                //获取查询结果
                var result = await pagedSorted.ToListAsync();
                return new PagedResultDto<GrossMarginDto>(count, ObjectMapper.Map<List<GrossMarginDto>>(result));
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }

        }
        /// <summary>
        /// 添加/修改毛利率方案 有则修改无则添加
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public async Task PostGrossMargin(BacktrackGrossMarginDto price)
        {
            try
            {
                //判断添加或者修改的是否是默认的方案
                if (price.IsDefaultn)
                {
                    List<GrossMarginForm> entityDefaultn = await _configGrossMarginForm.GetAllListAsync(p => p.IsDefaultn);
                    foreach (var item in entityDefaultn)
                    {
                        item.IsDefaultn = false;
                        await _configGrossMarginForm.UpdateAsync(item);
                    }
                }
                var prop = ObjectMapper.Map<GrossMarginForm>(price);
                GrossMarginForm entity = await _configGrossMarginForm.FirstOrDefaultAsync(p => p.Id.Equals(price.Id));
                if (entity == null)
                {
                    await _configGrossMarginForm.InsertAsync(prop);
                }
                else
                {
                    entity.GrossMarginPrice = prop.GrossMarginPrice;
                    entity.GrossMarginName = prop.GrossMarginName;
                    entity.IsDefaultn = price.IsDefaultn;
                    await _configGrossMarginForm.UpdateAsync(entity);
                }
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }

        }
        /// <summary>
        /// 删除毛利率方案
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task DeleteGrossMargin(long Id)
        {
            GrossMarginForm entity = await _configGrossMarginForm.FirstOrDefaultAsync(p=>p.Id.Equals(Id));
            if (entity != null)
            {
                if (entity.IsDefaultn) throw new FriendlyException("不能删除默认的毛利率");               
                await _configGrossMarginForm.DeleteAsync(entity);
            }
        }
        /// <summary>
        /// 添加/修改汇率
        /// </summary>
        /// <param name="exchangeRate"></param>
        /// <returns></returns>
        public async Task PostExchangeRate(ExchangeRateDto exchangeRate)
        {
            try
            {
                ExchangeRate entity = await _configExchangeRate.FirstOrDefaultAsync(p => p.Id.Equals(exchangeRate.Id));
                if (entity == null)
                {
                    var prop = ObjectMapper.Map<ExchangeRate>(exchangeRate);
                    await _configExchangeRate.InsertAsync(prop);
                }
                else
                {
                    entity.ExchangeRateKind = exchangeRate.ExchangeRateKind;
                    entity.ExchangeRateValue = JsonConvert.SerializeObject(exchangeRate.ExchangeRateValue);
                    await _configExchangeRate.UpdateAsync(entity);
                }
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 查询汇率
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<PagedResultDto<ExchangeRateDto>> GetExchangeRate(ExchangeRateInputDto input)
        {
            try
            {
                //定义列表查询
                var filter = _configExchangeRate.GetAll().WhereIf(!input.ExchangeRateKind.IsNullOrEmpty(), p => p.ExchangeRateKind.Contains(input.ExchangeRateKind));
                var pagedSorted = filter.OrderByDescending(p => p.Id).PageBy(input);
                //获取总数
                var count = await filter.CountAsync();
                //获取查询结果
                var result = await pagedSorted.ToListAsync();
                return new PagedResultDto<ExchangeRateDto>(count, ObjectMapper.Map<List<ExchangeRateDto>>(result));
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 删除汇率
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task DeleteExchangeRate(long Id)
        {
            ExchangeRate entity = await _configExchangeRate.FirstOrDefaultAsync(Id);
            if (entity != null)
            {
                await _configExchangeRate.DeleteAsync(entity);
            }
        }
    }

}
