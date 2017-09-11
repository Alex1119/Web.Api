using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Util;

namespace MQConsumer.Config
{
    public class MQRetryQueueManager
    {
        #region  singleton
        static readonly object _lock = new object();
        private static MQRetryQueueManager _CommonConfig = null;
        private static MQRetryQueueInfo _ConfigInfo = null;

        private MQRetryQueueManager() { }

        public static MQRetryQueueManager Instance()
        {
            if (_CommonConfig == null)
            {
                lock (_lock)
                {
                    if (_CommonConfig == null)
                    {
                        _CommonConfig = new MQRetryQueueManager();
                    }
                }
            }
            return _CommonConfig;
        }
        #endregion

        /// <summary>
        /// 配置
        /// </summary>
        string ConfigPath
        {
            get
            {
                var path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config", "MQRetryQueue.xml");

                return path;
            }
        }

        public List<MQRetryQueueItem> ConfigInfo
        {
            get
            {
                if (_ConfigInfo == null)
                {
                    _ConfigInfo = XmlHelper.XmlDeserializeFromFile<MQRetryQueueInfo>(this.ConfigPath, ASCIIEncoding.UTF8);
                }
                return _ConfigInfo != null ?_ConfigInfo.Items :new List<MQRetryQueueItem>();
            }
        }

        #region 配置相关方法        


        private int parseInt(string value)
        {
            int result = 0;
            var c = int.TryParse(value, out result);
            return result;
        }
        private bool parseBool(string value)
        {
            bool result = false;
            var c = bool.TryParse(value, out result);
            return result;
        }
        private double parseDouble(string value)
        {
            double result = 0;
            var c = double.TryParse(value, out result);
            return result;
        }

        #endregion
    }
}