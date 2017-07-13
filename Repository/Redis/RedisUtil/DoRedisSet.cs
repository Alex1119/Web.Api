using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Redis
{
    public class DoRedisSet : DoRedisBase
    {
        #region 添加
        /// <summary>
        /// key集合中添加value值
        /// </summary>
        public void Add(string key, string value)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                Core.AddItemToSet(key, value);
            }
        }
        /// <summary>
        /// key集合中添加list集合
        /// </summary>
        public void Add(string key, List<string> list)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                Core.AddRangeToSet(key, list);
            }
        }
        #endregion

        #region 获取
        /// <summary>
        /// 随机获取key集合中的一个值
        /// </summary>
        public string GetRandomItemFromSet(string key)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                return Core.GetRandomItemFromSet(key);
            }
        }
        /// <summary>
        /// 获取key集合值的数量
        /// </summary>
        public long GetCount(string key)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                return Core.GetSetCount(key);
            }
        }
        /// <summary>
        /// 获取所有key集合的值
        /// </summary>
        public HashSet<string> GetAllItemsFromSet(string key)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                return Core.GetAllItemsFromSet(key);
            }
        }
        #endregion

        #region 删除
        /// <summary>
        /// 随机删除key集合中的一个值
        /// </summary>
        public string PopItemFromSet(string key)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                return Core.PopItemFromSet(key);
            }
        }
        /// <summary>
        /// 删除key集合中的value
        /// </summary>
        public void RemoveItemFromSet(string key, string value)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                Core.RemoveItemFromSet(key, value);
            }
        }
        #endregion

        #region 其它
        /// <summary>
        /// 从fromkey集合中移除值为value的值，并把value添加到tokey集合中
        /// </summary>
        public void MoveBetweenSets(string fromkey, string tokey, string value)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                Core.MoveBetweenSets(fromkey, tokey, value);
            }
        }
        /// <summary>
        /// 返回keys多个集合中的并集，返还hashset
        /// </summary>
        public HashSet<string> GetUnionFromSets(string[] keys)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                return Core.GetUnionFromSets(keys);
            }
        }
        /// <summary>
        /// keys多个集合中的并集，放入newkey集合中
        /// </summary>
        public void StoreUnionFromSets(string newkey, string[] keys)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                Core.StoreUnionFromSets(newkey, keys);
            }
        }
        /// <summary>
        /// 把fromkey集合中的数据与keys集合中的数据对比，fromkey集合中不存在keys集合中，则把这些不存在的数据放入newkey集合中
        /// </summary>
        public void StoreDifferencesFromSet(string newkey, string fromkey, string[] keys)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                Core.StoreDifferencesFromSet(newkey, fromkey, keys);
            }
        }
        #endregion
    }
}
