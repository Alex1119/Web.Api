using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WEB.API.Filter
{
    public class TokenFilterAttribute : ActionFilterAttribute
    {
        //1.OnActionExecuting
        //    在Action方法调用前使用，使用场景：如何验证登录等。
        //2.OnActionExecuted
        //    在Action方法调用后，result方法调用前执行，使用场景：异常处理。
        //3.OnResultExecuting
        //    在result执行前发生(在view 呈现前)，使用场景：设置客户端缓存，服务器端压缩。
        //4.OnResultExecuted
        //    在result执行后发生，使用场景：异常处理，页面尾部输出调试信息。

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            string token = null;
            if (GetToken(actionContext, out token)) {

            }

            base.OnActionExecuting(actionContext);
        }

        private bool GetToken(HttpActionContext actionContext, out string token) {
            token = null;
            var viewModel = actionContext.ActionArguments["viewModel"];
            Type t = viewModel.GetType();
            foreach (var propertyInfo in t.GetProperties()) {
                var value = propertyInfo.GetValue(viewModel, null);
                var propertyName = propertyInfo.Name.ToLower();
                if (propertyName == "token") {
                    token = value.ToString();
                    break;
                }
            }
            return string.IsNullOrEmpty(token);
        }

    }
}