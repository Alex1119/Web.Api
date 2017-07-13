using Entity.EF;
using IRepository.cs.EF;
using IRepository.cs.MongoDB;
using IRepository.cs.Redis;
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


        public int CountsFromSQLServer()
        {
            return _IUserRepository.Counts();
        }

        public int CountsFromMongo()
        {
            return _IUserDetailRepository.Counts();
        }
       
        public int CountsFromRedis()
        {
            return _IUserInfoRepository.Counts();
        }

        public UserInfo Register(string UserName, bool? Gender, int? Age) {
            var entity = _IUserRepository.Register(UserName, Gender, Age);
            if (entity != null) {
                new DLXProducter().Pub<UserInfo>(entity);
            };
            return entity;
        }

    }
}
