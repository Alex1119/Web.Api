using Entity.EF;
using IRepository.Redis;
using Repository.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Redis
{
    public sealed class UserInfoRepository : UserRepository, IUserInfoRepository
    {
        #region 
        private const string PRE_SINGLE_KEY = "SingleKey";
        private const string PRE_USERINFO_KEY = "UserInfoKey";
        private TimeSpan COMMON_EXPIRE_TIMESPAN = new TimeSpan(24, 0, 0);

        private string USER_INFO_KEY(string UserId) { return string.Format("{0}:{1}", PRE_USERINFO_KEY, UserId); }
        #endregion


        public override UserInfo Register(string UserName, bool Gender, int Age){
            var userInfo = base.Register(UserName, Gender, Age);
            if (userInfo != null) {
                try
                {
                    var repository = new DoRedisString();
                    var key = USER_INFO_KEY(userInfo.UserID);
                    repository.Set(key, userInfo, COMMON_EXPIRE_TIMESPAN);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            return userInfo;
        }

    }
}
