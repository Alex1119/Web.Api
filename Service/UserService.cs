using Entity.EF;
using IRepository.EF;
using IRepository.MongoDB;
using IRepository.Redis;
using IService;
using Service.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class UserService : IUserService
    {

        #region
        public IUserRepository _IUserRepository { get; private set; }
        public IUserInfoRepository _IUserInfoRepository { get; private set; }
        public IUserDetailRepository _IUserDetailRepository { get; private set; }

        public UserService(IUserRepository _IUserRepository, IUserInfoRepository _IUserInfoRepository, IUserDetailRepository _IUserDetailRepository)
        {
            this._IUserRepository = _IUserRepository;
            this._IUserInfoRepository = _IUserInfoRepository;
            this._IUserDetailRepository = _IUserDetailRepository;
        }

        #endregion

        public UserInfo Register(string UserName, bool Gender, int Age) {
            var entity = _IUserInfoRepository.Register(UserName, Gender, Age);
            if (entity != null) {
                new DLXProducter().Pub<UserInfo>(entity);
            };
            return entity;
        }

    }
}
