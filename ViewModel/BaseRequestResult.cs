using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class BaseRequestResult : HttpResponseMessage
    {
        public BaseRequestResult(Object data = null, string message = null, HttpStatusCode state = HttpStatusCode.OK)
        {
            var result = new ResponseData<Object>()
            {
                Data = data,
                Message = message,
                Code = (int)state
            };
            //Content = new ObjectContent<HttpStatusCode<Object>>(result, new JsonMediaTypeFormatter());
            StatusCode = HttpStatusCode.OK;
        }

    }
}
