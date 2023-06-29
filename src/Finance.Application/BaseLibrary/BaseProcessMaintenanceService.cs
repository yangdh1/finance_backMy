using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Finance.Audit;
using Finance.BaseLibrary;
using Finance.Dto;
using Finance.Hr;
using Finance.Roles.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Finance.BaseLibrary
{
    public class BaseProcessMaintenanceService : ApplicationService
    {
        private readonly IRepository<BaseProcessMaintenance, long> _baseProcessMaintenance;
        public BaseProcessMaintenanceService(IRepository<BaseProcessMaintenance, long> baseProcessMaintenance)
        {
            this._baseProcessMaintenance = baseProcessMaintenance;
        }

        /// <summary>
        /// 根据
        /// </summary>
        /// <param name="paraDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PagedResultDto<ProcessMaintenanceDto>> PostGetProcessMaintenanceList(GetProcessMaintenanceDto input)
        {
            // 设置查询条件
            var query = this._baseProcessMaintenance.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var tasksCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();

            var resultList = ObjectMapper.Map<List<ProcessMaintenanceDto>>(list);

            return new PagedResultDto<ProcessMaintenanceDto>(tasksCount, resultList);
        }


        /// <summary>
        /// 获取工序列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<ProcessMaintenanceDto>> GetProcessMaintenanceAll()
        {
            var list = await this._baseProcessMaintenance.GetAllListAsync(p => p.IsDeleted == false);
            List<ProcessMaintenanceDto> processMaintenanceDtos =  ObjectMapper.Map<List<ProcessMaintenanceDto>>(list);
            return processMaintenanceDtos;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="processMaintenanceDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResultDto> UpdateProcessMaintenance(ProcessMaintenanceDto input)
        {
            var processMaintenance = ObjectMapper.Map<BaseProcessMaintenance>(input);
            processMaintenance.LastModificationTime = DateTime.Now;
            if (AbpSession.UserId != null)
            {
                processMaintenance.LastModifierUserId = AbpSession.UserId.Value;
            }
            await this._baseProcessMaintenance.UpdateAsync(processMaintenance);
            return new ResultDto { IsSuccess = true };
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="processMaintenanceDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResultDto> AddProcessMaintenance(ProcessMaintenanceDto input)
        {
            var processMaintenance = ObjectMapper.Map<BaseProcessMaintenance>(input);
            var maxId = this._baseProcessMaintenance.GetAll().Max(t => t.Id);
            processMaintenance.CreationTime = DateTime.Now;
            processMaintenance.Id = maxId + 1;
            if (AbpSession.UserId != null)
            {
                processMaintenance.CreatorUserId = AbpSession.UserId.Value;
            }
            await this._baseProcessMaintenance.InsertAsync(processMaintenance);
            return new ResultDto { IsSuccess = true };
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="processMaintenanceDto"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResultDto> DeleteProcessMaintenance(int id)
        {
            var p = await this._baseProcessMaintenance.GetAsync(id);
            p.DeletionTime = DateTime.Now;
            p.IsDeleted = true;
            if (AbpSession.UserId != null)
            {
                p.DeleterUserId = AbpSession.UserId.Value;
            }
            await this._baseProcessMaintenance.UpdateAsync(p);
            return new ResultDto { IsSuccess = true };
        }


        /// <summary>
        /// 根据id查询
        /// </summary>
        /// <param name="processMaintenanceDto"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResultDto> GetProcessMaintenanceById(int id)
        {
            var p = await this._baseProcessMaintenance.GetAsync(id);
            return new ResultDto { IsSuccess = true, ResultData = ObjectMapper.Map<ProcessMaintenanceDto>(p)};
        }
    }
}
