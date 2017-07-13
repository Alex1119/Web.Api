using Repository.Redis;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Redis
{
    public class DoRedisSubscription : DoRedisBase
    {
        private IRedisSubscription Subscription
        {
            get
            {
                using (IRedisClient Core = CreateRedisClient())
                {
                    return Core.CreateSubscription();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            Subscription.Dispose();

            base.Dispose(disposing);
        }

        #region 发布订阅

        /// <summary>
        /// Redis发布订阅  订阅
        /// </summary>
        /// <param name="subChannel"></param>
        /// <param name="handler"></param>
        public void Subscribe(string subChannel, Action<string, string> handler = null)
        {
            Subscription.OnSubscribe = channel =>
            {
                Console.WriteLine(subChannel + " 发布订阅收到消息");
            };
            Subscription.OnMessage = (channel, msg) =>
            {
                if (handler == null)
                {
                    Console.WriteLine(subChannel + " 订阅收到消息:" + msg);
                }
                else
                {
                    handler(channel, msg);
                }

            };

            Subscription.SubscribeToChannels(subChannel); //blocking
        }

        /// <summary>
        /// Redis发布订阅  发布
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public long Publish<T>(string channel, T msg)
        {
            using (IRedisClient Core = CreateRedisClient())
            {
                return Core.PublishMessage(channel, ConvertJson(msg));
            }
        }

        /// <summary>
        /// Redis发布订阅  取消订阅
        /// </summary>
        /// <param name="channel"></param>
        public void Unsubscribe(string channel)
        {
            Subscription.UnSubscribeFromChannels(channel);
        }

        /// <summary>
        /// Redis发布订阅  取消全部订阅
        /// </summary>
        public void UnsubscribeAll()
        {
            Subscription.UnSubscribeFromAllChannels();
        }

        #endregion 发布订阅

    }
}
