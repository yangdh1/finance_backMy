using Abp;
using Abp.Dependency;
using Abp.Domain.Repositories;
using AutoMapper;
using Finance.Audit;
using Finance.Audit.Dto;
using Finance.Authorization.Users;
using Finance.EntityFrameworkCore;
using Finance.Job;
using Finance.MakeOffers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sundial;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Finance.Job
{
    public class MailJob : IJob
    {
        private IHttpClientFactory _httpClient;
    
        //在访问控制器时，进行IHttpClientFactory的初始化
        public MailJob(IHttpClientFactory _httpClient)
        {
            this._httpClient = _httpClient;      
        }
 
        public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
        {
            SendEmail sendEmail = new();
            string loginIp = sendEmail.GetLoginAddr();
            string Mail_Http = $"http://{loginIp}:44311/api/services/app/Mail/GetIsMain";
            var client = _httpClient.CreateClient();
            var response = await client.GetAsync(Mail_Http);
            await Task.CompletedTask;

        }       
    }
}
