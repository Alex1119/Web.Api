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
using IRepository.EF;
using IRepository.Redis;
using IRepository.MongoDB;
using IService;
using Entity.EF;
using ViewModel.User;
using ViewModel;

namespace WEB.API.Controllers
{
    public class UserController : BaseController
    {

        #region
        public IUserService _IUserService { get; set; }
        #endregion

        [HttpGet]
        public ResponseData<int> CountsFromSQLServer()
        {
            var counts = _IUserService.CountsFromSQLServer();
            return Response<int>(counts);
        }

        [HttpPost]
        public ResponseData<int> CountsFromMongo()
        {
            var counts = _IUserService.CountsFromMongo();
            return Response<int>(counts);
        }

        [HttpPost]
        public ResponseData<int> CountsFromRedis()
        {
            var counts = _IUserService.CountsFromRedis();
            return Response<int>(counts);
        }

        [HttpPost]
        public ResponseData<DTO_Output_Register> Register(DTO_Input_Register viewModel) {
            var entity = _IUserService.Register(viewModel.UserName, viewModel.Gender, viewModel.Age);
            return Response<DTO_Output_Register>(new DTO_Output_Register().ConvertFrom(entity));
        }

    }
}
