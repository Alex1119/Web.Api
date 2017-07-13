using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Redis
{
    public static class DoRedisExtensions
    {

        public static List<T> Get<T>(this DoRedisList input, string key) where T : class, new()
        {
            var data = input.Get(key);
            var result = new List<T>();
            data.ForEach(item => {
                result.Add(StringToObject<T>(item));

            });
            return result;
        }

        public static void LPush<T>(this DoRedisList input, string key, T entity) where T : class, new()
        {
            var item = ConvertJson<T>(entity);
            input.LPush(key, item);
        }

        public static void RPush<T>(this DoRedisList input, string key, T entity) where T : class, new()
        {
            var item = ConvertJson<T>(entity);
            input.RPush(key, item);
        }

        public static void AddList<T>(this DoRedisList input, string key, List<T> entitys) where T : class, new()
        {
            List<string> list = new List<string>();
            foreach (T entity in entitys)
            {
                string item = ConvertJson<T>(entity);
                list.Add(item);
            }
            input.Add(key, list);
        }

        public static bool SetEntryInHash<T>(this DoRedisHash input, string hashid, string key, T entity) where T : class, new()
        {
            string item = ConvertJson<T>(entity);
            bool result = input.SetEntryInHash(hashid, key, item);
            return result;
        }

        public static T GetValueFromHash<T>(this DoRedisHash input, string hashid, string key) where T : class, new()
        {
            string item = input.GetValueFromHash(hashid, key);
            if (string.IsNullOrEmpty(item))
            {
                return null;
            }
            T obj = StringToObject<T>(item);
            return obj;
        }

        public static List<T> GetHashValues<T>(this DoRedisHash input, string hashid) where T : class, new()
        {
            List<T> list = new List<T>();
            List<string> values = input.GetHashValues(hashid);
            foreach (string value in values)
            {
                T obj = StringToObject<T>(value);
                list.Add(obj);
            }
            return list;
        }

        #region 辅助方法

        public static string ConvertJson<T>(T value)
        {
            string result = value is string ? value.ToString() : JsonConvert.SerializeObject(value);
            return result;
        }

        public static T StringToObject<T>(string value)
        {
            var result = string.IsNullOrEmpty(value) ? default(T) : JsonConvert.DeserializeObject<T>(value);
            return result;
        }


        #endregion 辅助方法
    }
}
