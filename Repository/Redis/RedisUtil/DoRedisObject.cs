using Repository.Redis;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Redis
{
    public class DoRedisObject : DoRedisBase
    {
        #region 添加
        /// <summary>
        /// key集合中添加value值
        /// </summary>
        public void Add<T>(string key, T value)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                Core.Add<T>(key, value);
            }
        }

        #endregion

        #region 获取
        /// <summary>
        /// 获取key中的一个值
        /// </summary>
        public T Get<T>(string key)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                return Core.Get<T>(key);
            }
        }

        /// <summary>
        /// 获取keys集合中的一个值
        /// </summary>
        public IDictionary<string, T> Get<T>(List<string> keys)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                return Core.GetAll<T>(keys);
            }
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除key中的一个值
        /// </summary>
        public void Remove(string key)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                Core.Remove(key);
            }
        }
        #endregion

    }
}
