using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TempModbusProject.Service.IService;

namespace TempModbusProject.Configure.FilterConfigure
{
    public class AllExceptionFilter : IAsyncExceptionFilter
    {
        private readonly IHostEnvironment env;
        private readonly IConsoleChangeColors _consoleChangeColors;

        public AllExceptionFilter(IHostEnvironment env,IConsoleChangeColors consoleChangeColors) //全局异常过滤器
        {
            this.env = env;
            _consoleChangeColors = consoleChangeColors;
        }
        public Task OnExceptionAsync(ExceptionContext context)
        {
            Exception exception =context.Exception;
            String message=exception.Message;
            if (env.IsDevelopment())
            {
                message = exception.Message;
            }
            else
            {
                message = "服务器发生错误，请稍后再试！";
            }
            _consoleChangeColors.ConsoleChangeRed(exception.ToString());
            ObjectResult result = new ObjectResult(new { code = 500, message = message });//设置响应报文
            result.StatusCode = 500; //这里设置响应状态码500
            context.Result = result; //设置响应结果
            context.ExceptionHandled = true; //设置异常已处理
            return Task.CompletedTask;


        }
    }
}
