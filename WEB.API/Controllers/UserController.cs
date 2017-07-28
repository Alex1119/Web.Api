using Repository.EF;
using Repository.MongoDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Entity;
using Repository.Redis;
using IService;
using Entity.EF;
using ViewModel.User;
using ViewModel;
using WEB.API.Filter;
using IRepository.MongoDB;

namespace WEB.API.Controllers
{
    [TokenFilter, BaseExceptionFilter]
    public class UserController : BaseController
    {

        #region
        public IUserService _IUserService { get; set; }
        #endregion

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseData<DTO_Output_Register> Register(DTO_Input_Register viewModel) {
            var entity = _IUserService.Register(viewModel.UserName, viewModel.Gender, viewModel.Age);
            return Response<DTO_Output_Register>(new DTO_Output_Register().ConvertFrom(entity));
        }

    }
}
