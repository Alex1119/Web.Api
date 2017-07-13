using Repository.Redis;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Redis
{
    public class DoRedisHash : DoRedisBase
    {
        #region 添加
        /// <summary>
        /// 向hashid集合中添加key/value
        /// </summary>       
        public bool SetEntryInHash(string hashid, string key, string value)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                return Core.SetEntryInHash(hashid, key, value);
            }
        }
        /// <summary>
        /// 如果hashid集合中存在key/value则不添加返回false，如果不存在在添加key/value,返回true
        /// </summary>
        public bool SetEntryInHashIfNotExists(string hashid, string key, string value)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                return Core.SetEntryInHashIfNotExists(hashid, key, value);
            }
        }
        /// <summary>
        /// 存储对象T t到hash集合中
        /// </summary>
        public void StoreAsHash<T>(T t)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                Core.StoreAsHash<T>(t);
            }
        }
        #endregion

        #region 获取
        /// <summary>
        /// 获取对象T中ID为id的数据。
        /// </summary>
        public T GetFromHash<T>(object id)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                return Core.GetFromHash<T>(id);
            }
        }
        /// <summary>
        /// 获取所有hashid数据集的key/value数据集合
        /// </summary>
        public Dictionary<string, string> GetAllEntriesFromHash(string hashid)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                return Core.GetAllEntriesFromHash(hashid);
            }
        }
        /// <summary>
        /// 获取hashid数据集中的数据总数
        /// </summary>
        public long GetHashCount(string hashid)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                return Core.GetHashCount(hashid);
            }
        }
        /// <summary>
        /// 获取hashid数据集中所有key的集合
        /// </summary>
        public List<string> GetHashKeys(string hashid)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                return Core.GetHashKeys(hashid);
            }
        }
        /// <summary>
        /// 获取hashid数据集中的所有value集合
        /// </summary>
        public List<string> GetHashValues(string hashid)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                return Core.GetHashValues(hashid);
            }
        }
        /// <summary>
        /// 获取hashid数据集中，key的value数据
        /// </summary>
        public string GetValueFromHash(string hashid, string key)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                return Core.GetValueFromHash(hashid, key);
            }
        }
        /// <summary>
        /// 获取hashid数据集中，多个keys的value集合
        /// </summary>
        public List<string> GetValuesFromHash(string hashid, string[] keys)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                return Core.GetValuesFromHash(hashid, keys);
            }
        }
        #endregion

        #region 删除
        #endregion

        /// <summary>
        /// 删除hashid数据集中的key数据
        /// </summary>
        public bool RemoveEntryFromHash(string hashid, string key)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                return Core.RemoveEntryFromHash(hashid, key);
            }
        }
        #region 其它
        /// <summary>
        /// 判断hashid数据集中是否存在key的数据
        /// </summary>
        public bool HashContainsEntry(string hashid, string key)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                return Core.HashContainsEntry(hashid, key);
            }
        }
        /// <summary>
        /// 给hashid数据集key的value加countby，返回相加后的数据
        /// </summary>
        public double IncrementValueInHash(string hashid, string key, int countBy)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                return Core.IncrementValueInHash(hashid, key, countBy);
            }
        }
        #endregion
    }
}
