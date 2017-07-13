using IRepository.cs.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Redis
{
    public class UserInfoRepository : IUserInfoRepository
    {
        #region 
        private const string PRE_SINGLE_KEY = "SingleKey";
        private TimeSpan COMMON_EXPIRE_TIMESPAN = new TimeSpan(24, 0, 0);

        private string USER_INFO_KEY { get { return string.Format("{0}:UserInfoKey", PRE_SINGLE_KEY); } }
        #endregion

        public int Counts() {
            using (var client = RedisManager.GetClient()) {
                return (int)client.GetSetCount(USER_INFO_KEY);
            }
        }

    }
}
