using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics.Contracts;

namespace TempModbusProject.Configure.FilterConfigure
{
    public class currentLimit : IAsyncActionFilter
    {
        private readonly IMemoryCache _memoryCache;
        public currentLimit(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)//全局限流过滤器
        {
            Console.WriteLine("过滤器设置");
           String removeIp = context.HttpContext.Connection.RemoteIpAddress?.ToString(); //获取请求的IP地址
            String cacheKey = $"LastTime{removeIp}";
           long lastTikc=  _memoryCache.Get<long>(cacheKey);
            if (lastTikc == null || Environment.TickCount64 - lastTikc > 2000)
            {
                _memoryCache.Set<long>(cacheKey, Environment.TickCount64, TimeSpan.FromSeconds(10));
                await next(); //放行
                return;
            }
            else {
                context.Result = new ContentResult { StatusCode = 429 };
                return; 
            }
        }
    }
}
