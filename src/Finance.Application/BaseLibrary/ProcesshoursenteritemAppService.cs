using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Finance.BaseLibrary;
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
    public class ProcesshoursenteritemAppService : ApplicationService
    {
        private readonly IRepository<Processhoursenteritem, long> _processhoursenteritem;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="processhoursenteritemRepository"></param>
        public ProcesshoursenteritemAppService(
            IRepository<Processhoursenteritem, long> processhoursenteritem)
        {
            _processhoursenteritem = processhoursenteritem;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<ProcesshoursenteritemDto> GetAsync(long id)
        {
            try
            {
                Processhoursenteritem entity = await _processhoursenteritem.GetAsync(id);

                return ObjectMapper.Map<ProcesshoursenteritemDto>(entity);
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
        public virtual async Task<PagedResultDto<ProcesshoursenteritemDto>> GetListAsync(GetProcesshoursenteritemsInput input)
        {
            // 设置查询条件
            var query = this._processhoursenteritem.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var tasksCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            var resultList = ObjectMapper.Map<List<ProcesshoursenteritemDto>>(list);
            return new PagedResultDto<ProcesshoursenteritemDto>(tasksCount, resultList);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<ProcesshoursenteritemDto> GetEditorAsync(long id)
        {
            Processhoursenteritem entity = await _processhoursenteritem.GetAsync(id);

            return ObjectMapper.Map<ProcesshoursenteritemDto>(entity);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<ProcesshoursenteritemDto> CreateAsync(ProcesshoursenteritemDto input)
        {
            var entity = ObjectMapper.Map<Processhoursenteritem>(input);
            entity = await _processhoursenteritem.InsertAsync(entity);
            return ObjectMapper.Map<ProcesshoursenteritemDto>(entity);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<ProcesshoursenteritemDto> UpdateAsync(long id, ProcesshoursenteritemDto input)
        {
            Processhoursenteritem entity = await _processhoursenteritem.GetAsync(id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _processhoursenteritem.UpdateAsync(entity);
            return ObjectMapper.Map<ProcesshoursenteritemDto>(entity);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _processhoursenteritem.DeleteAsync(s => s.Id == id);
        }
    }
}
