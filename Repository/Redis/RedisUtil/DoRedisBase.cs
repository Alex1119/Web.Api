using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Redis
{
    public abstract class DoRedisBase : IDisposable
    {
        //protected IRedisClient Core { get; private set; }
        private bool _disposed = false;

        protected DoRedisBase()
        {
            //Core = RedisManager.GetClient();
        }

        protected IRedisClient CreateRedisClient()
        {
            return RedisManager.GetClient();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    //Core.Dispose();
                    //Core = null;
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// 保存数据DB文件到硬盘
        /// </summary>
        /// 

        public void Save()
        {
            //Core.Save();
        }
        /// <summary>
        /// 异步保存数据DB文件到硬盘
        /// </summary>
        /// 

        public void SaveAsync()
        {
            //Core.SaveAsync();
        }

        #region 辅助方法

        public string ConvertJson<T>(T value)
        {
            string result = value is string ? value.ToString() : JsonConvert.SerializeObject(value);
            return result;
        }

        public T StringToObject<T>(string value)
        {
            var result = string.IsNullOrEmpty(value) ? default(T) : JsonConvert.DeserializeObject<T>(value);
            return result;
        }


        #endregion 辅助方法
    }
}
