using Autofac;
using Entity.MongoDB;
using IRepository.MongoDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using ViewModel;

namespace WEB.API.Filter
{
    public class BaseExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private ILogRepository _ILogRepository { get { return WebApiApplication.Container.Resolve<ILogRepository>(); } }

        //重写基类的异常处理方法
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            //1.异常日志记录（正式项目里面一般是用log4net记录异常日志）
            //var message = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "——" +
            //    actionExecutedContext.Exception.GetType().ToString() + "：" + actionExecutedContext.Exception.Message + "——堆栈信息：" +
            //    actionExecutedContext.Exception.StackTrace;

            // 加入异常日志
            _ILogRepository.AddExpection(new ExceptionLog(actionExecutedContext.Exception));

            //2.返回调用方具体的异常信息
            if (actionExecutedContext.Exception is NotImplementedException)
            {
                actionExecutedContext.Response = new BaseRequestResult(HttpStatusCode.NotImplemented);
            }
            else if (actionExecutedContext.Exception is TimeoutException)
            {
                actionExecutedContext.Response = new BaseRequestResult(HttpStatusCode.RequestTimeout);
            }
            else
            {
                actionExecutedContext.Response = new BaseRequestResult(HttpStatusCode.InternalServerError);
            }
            base.OnException(actionExecutedContext);
        }
    }
}