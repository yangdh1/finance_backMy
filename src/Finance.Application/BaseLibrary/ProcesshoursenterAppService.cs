using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 管理
    /// </summary>
    public class ProcesshoursenterAppService : ApplicationService
    {
        private readonly IRepository<Processhoursenter, long> _processhoursenter;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="processhoursenter"></param>
        public ProcesshoursenterAppService(
            IRepository<Processhoursenter, long> processhoursenter)
        {
            _processhoursenter = processhoursenter;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<ProcesshoursenterDto> GetAsync(long id)
        {
            try
            {
                Processhoursenter entity = await _processhoursenter.GetAsync(id);
                return ObjectMapper.Map<ProcesshoursenterDto>(entity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<ProcesshoursenterDto>> GetListAsync(GetProcesshoursentersInput input)
        {
            // 设置查询条件
            var query = this._processhoursenter.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var tasksCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            var resultList = ObjectMapper.Map<List<ProcesshoursenterDto>>(list);
            return new PagedResultDto<ProcesshoursenterDto>(tasksCount, resultList);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<ProcesshoursenterDto> GetEditorAsync(long id)
        {
            Processhoursenter entity = await this._processhoursenter.GetAsync(id);

            return ObjectMapper.Map<ProcesshoursenterDto>(entity);
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<ProcesshoursenterDto> CreateAsync(ProcesshoursenterDto input)
        {
            var entity = ObjectMapper.Map<Processhoursenter>(input);
            entity = await this._processhoursenter.InsertAsync(entity);
            return ObjectMapper.Map<ProcesshoursenterDto>(entity);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<ProcesshoursenterDto> UpdateAsync(long id, ProcesshoursenterDto input)
        {
            Processhoursenter entity = await this._processhoursenter.GetAsync(id);
            entity = ObjectMapper.Map(input, entity);
            entity = await this._processhoursenter.UpdateAsync(entity);
            return ObjectMapper.Map<ProcesshoursenterDto>(entity);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(decimal id)
        {
            await this._processhoursenter.DeleteAsync(s => s.Id == id);
        }
    }
}
