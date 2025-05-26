using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using System.Transactions;
using TempModbusProject.Configure.AttributeConfigure;

namespace TempModbusProject.Configure.FilterConfigure
{
    public class TransactionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) //全局事务过滤器
        {
            bool hasNotTransactional = false;
            if(context.ActionDescriptor is ControllerActionDescriptor) //判断是不是控制器方法
            {
                var actionDesc=(ControllerActionDescriptor)context.ActionDescriptor;
                hasNotTransactional=actionDesc.MethodInfo.IsDefined(typeof(NotTransactionalAttribute));
            }
            if (hasNotTransactional)
            { 
               await  next();
                return;

            }
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);//开启事务
            var result = await next();
            if (result.Exception==null)
            {
                transaction.Complete();
            }
        }
    }
}
