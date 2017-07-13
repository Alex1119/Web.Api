using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.MongoDB
{
    public sealed class MongoConfig : ConfigurationSection
    {
        public static MongoConfig GetConfig(string sectionName)
        {
            MongoConfig section = (MongoConfig)ConfigurationManager.GetSection(sectionName);
            if (section == null)
                throw new ConfigurationErrorsException("Section " + sectionName + " is not found.");
            return section;
        }

        /// <summary>
        /// 可写的Mongo链接地址
        /// </summary>
        [ConfigurationProperty("ServerConStr", IsRequired = false)]
        public string ServerConStr
        {
            get
            {
                return (string)base["ServerConStr"];
            }
            set
            {
                base["ServerConStr"] = value;
            }
        }

        [ConfigurationProperty("ServerPort", IsRequired = false)]
        public int ServerPort
        {
            get
            {
                return (int)base["ServerPort"];
            }
            set
            {
                base["ServerPort"] = value;
            }
        }

        /// <summary>
        /// 默认的连接数据库
        /// </summary>
        [ConfigurationProperty("DefaultDb", IsRequired = false)]
        public string DefaultDb
        {
            get
            {
                return (string)base["DefaultDb"];
            }
            set
            {
                base["DefaultDb"] = value;
            }
        }

        [ConfigurationProperty("UserName", IsRequired = false)]
        public string UserName
        {
            get
            {
                return (string)base["UserName"];
            }
            set
            {
                base["UserName"] = value;
            }
        }

        [ConfigurationProperty("PassWord", IsRequired = false)]
        public string PassWord
        {
            get
            {
                return (string)base["PassWord"];
            }
            set
            {
                base["PassWord"] = value;
            }
        }

        /// <summary>
        /// 最大连接池
        /// </summary>
        [ConfigurationProperty("MaxConnectionPoolSize", IsRequired = false, DefaultValue = 500)]
        public int MaxConnectionPoolSize
        {
            get
            {
                return (int)base["MaxConnectionPoolSize"];
            }
            set
            {
                base["MaxConnectionPoolSize"] = value;
            }
        }

        /// <summary>
        /// 最大闲置时间
        /// </summary>
        [ConfigurationProperty("MaxConnectionIdleTime", IsRequired = false, DefaultValue = 30)]
        public int MaxConnectionIdleTime
        {
            get
            {
                return (int)base["MaxConnectionIdleTime"];
            }
            set
            {
                base["MaxConnectionIdleTime"] = value;
            }
        }

        /// <summary>
        /// 最大存活时间
        /// </summary>
        [ConfigurationProperty("MaxConnectionLifeTime", IsRequired = false, DefaultValue = 60)]
        public int MaxConnectionLifeTime
        {
            get
            {
                return (int)base["MaxConnectionLifeTime"];
            }
            set
            {
                base["MaxConnectionLifeTime"] = value;
            }
        }

        /// <summary>
        /// 链接时间
        /// </summary>
        [ConfigurationProperty("ConnectTimeout", IsRequired = false, DefaultValue = 10)]
        public int ConnectTimeout
        {
            get
            {
                return (int)base["ConnectTimeout"];
            }
            set
            {
                base["ConnectTimeout"] = value;
            }
        }

        /// <summary>
        /// 等待队列大小
        /// </summary>
        [ConfigurationProperty("WaitQueueSize", IsRequired = false, DefaultValue = 50)]
        public int WaitQueueSize
        {
            get
            {
                return (int)base["WaitQueueSize"];
            }
            set
            {
                base["WaitQueueSize"] = value;
            }
        }

        /// <summary>
        /// socket超时时间
        /// </summary>
        [ConfigurationProperty("SocketTimeout", IsRequired = false, DefaultValue = 10)]
        public int SocketTimeout
        {
            get
            {
                return (int)base["SocketTimeout"];
            }
            set
            {
                base["SocketTimeout"] = value;
            }
        }

        /// <summary>
        /// 队列等待时间
        /// </summary>
        [ConfigurationProperty("WaitQueueTimeout", IsRequired = false, DefaultValue = 60)]
        public int WaitQueueTimeout
        {
            get
            {
                return (int)base["WaitQueueTimeout"];
            }
            set
            {
                base["WaitQueueTimeout"] = value;
            }
        }

        ///// <summary>
        ///// 操作时间
        ///// </summary>
        //[ConfigurationProperty("OperationTimeout", IsRequired = false, DefaultValue = 60)]
        //public int OperationTimeout
        //{
        //    get
        //    {
        //        return (int)base["OperationTimeout"];
        //    }
        //    set
        //    {
        //        base["OperationTimeout"] = value;
        //    }
        //}

    }
}
