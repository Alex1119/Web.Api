using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Entity;
using ViewModel;

namespace WEB.API.Controllers
{
    public class BaseController : ApiController
    {

        public static ResponseData<T> Response<T>(int Code, string Message, T Data)
        {
            return new ResponseData<T>
            {
                Code = Code,
                Message = Message,
                Data = Data
            };
        }
        public static ResponseData<T> Response<T>(int Code, string Message)
        {
            return Response<T>(Code, Message);
        }
        public static ResponseData<T> Response<T>(T Data)
        {
            return Response<T>(1, "操作成功！", Data);
        }

    }
}
