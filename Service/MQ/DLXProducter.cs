using Entity.EF;
using IRepository.MongoDB;
using RabbitMQ.Client;
using Service.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Util;

namespace Service.MQ
{
    public class DLXProducter : MQBase
    {

        public IConnection connection;
        public IModel channel;

        //public void Pub<T>(T Data){
        //    var userInfo = Data as UserInfo;
        //    if (userInfo != null) {
        //        Pub<UserInfo>(new MQMessage<UserInfo>(5000, MQ_USER_EXCHANGE, MQ_DLX_QUEUE, MQ_USER_ROUTEKEY, userInfo));
        //    }
        //}

        //public void RePub<T>(MQMessage<T> Message)
        //{
        //    if (Message.MessageTTL > 10000) {
        //        return;
        //    }
        //    var userMessage = Message as MQMessage<UserInfo>;
        //    if (userMessage != null)
        //    {
        //        userMessage.MessageTTL += 5000;
        //        Pub<UserInfo>(userMessage);
        //    }
        //}

        public void Declare()
        {
            try
            {
                //var queueArg = new Dictionary<string, object>();
                //queueArg.Add("x-message-ttl", 10000);//队列上消息过期时间，应小于队列过期时间
                //queueArg.Add("x-dead-letter-exchange", MQ_USER_EXCHANGE);//过期消息转向路由  
                //queueArg.Add("x-dead-letter-routing-key", MQ_USER_ROUTEKEY);//过期消息转向路由相匹配routingkey, 如果不指定沿用死信队列的routingkey
                ////Subscribe(ref connection, ref channel, MQ_DLX_EXCHANGE, MQ_DLX_QUEUE, MQ_DLX_ROUTEKEY, null, queueArg);

                connection = CreateConnectFactory().CreateConnection();
                channel = connection.CreateModel();
                channel.BasicQos(0, 1, false);
                channel.ExchangeDeclare(MQ_DLX_EXCHANGE, ExchangeType.Headers, true, false, null);

                MQ_RETRY_QUEUE_LIST.ForEach((item) =>
                {
                    channel.QueueDeclare(item.QueueName, true, false, false, new Dictionary<string, object>() {
                        { "x-message-ttl", item.TTL},
                        { "x-dead-letter-exchange", MQ_USER_EXCHANGE}
                    });
                    channel.QueueBind(item.QueueName, MQ_DLX_EXCHANGE, string.Empty,
                        new Dictionary<string, object>() {
                            { "x-match", "all" },
                            { "RouteKey", item.QueueName }
                        });
                });
            }
            catch (Exception ex)
            {
            }
            finally
            {
                UnSubscribe(connection, channel);
            }
        }

        public void Pub<T>(MQMessage<T> message, Dictionary<string, object> headers = null)
        {
            IConnection connection = null;
            IModel channel = null;
            try
            {

                connection = CreateConnectFactory().CreateConnection();
                channel = connection.CreateModel();
                channel.BasicQos(0, 1, false);
                channel.ExchangeDeclare(MQ_DLX_EXCHANGE, ExchangeType.Headers, true, false, null);

                foreach (var item in MQ_RETRY_QUEUE_LIST)
                {
                    if (message.RejectTime.Equals(item.RejectTime))
                    {
                        channel.QueueDeclare(item.QueueName, true, false, false, new Dictionary<string, object>() {
                            { "x-message-ttl", item.TTL},
                            { "x-dead-letter-exchange", MQ_USER_EXCHANGE}
                        });
                        channel.QueueBind(item.QueueName, MQ_DLX_EXCHANGE, string.Empty,new Dictionary<string, object>() {
                            { "x-match", "all" },
                            { "RouteKey", item.QueueName }
                        });
                    }
                }
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2;//数据持久化
                properties.Headers = headers;
                var msgBytes = JSONHelper.SerializeToByte(message);
                channel.BasicPublish(MQ_DLX_EXCHANGE, string.Empty, properties, msgBytes);
                
            }
            catch (Exception ex)
            {
            }
            finally
            {
                UnSubscribe(connection, channel);
            }
        }

    }
}
