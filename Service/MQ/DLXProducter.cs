using Entity.EF;
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

        public void Pub<T>(T Data){
            var userInfo = Data as UserInfo;
            if (userInfo != null) {

                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(1000);
                        Pub<UserInfo>(new DLXMessage<UserInfo>(5000, MQ_USER_EXCHANGE, MQ_USER_ROUTEKEY, userInfo));
                        Pub<UserInfo>(new DLXMessage<UserInfo>(5000, MQ_USER_EXCHANGE, MQ_USER_ROUTEKEY2, userInfo));
                    }
                   
                });
                
            }
        }

        public void RePub<T>(DLXMessage<T> Message)
        {
            if (Message.MessageTTL > 10000) {
                return;
            }
            var userMessage = Message as DLXMessage<UserInfo>;
            if (userMessage != null)
            {
                userMessage.MessageTTL += 5000;
                Pub<UserInfo>(userMessage);
            }
        }

        public void Pub<T>(DLXMessage<T> Message)
        {
            try
            {
                var exchangeArg = new Dictionary<string, object>();
                //exchangeArg.Add("x-message-ttl", message.MessageTTL);//队列上消息过期时间，应小于队列过期时间  
                exchangeArg.Add("x-dead-letter-exchange", Message.ConsumerExchange);//过期消息转向路由  
                //exchangeArg.Add("x-dead-letter-routing-key", message.ConsumerRouingKey);//过期消息转向路由相匹配routingkey, 如果不指定沿用死信队列的routingkey

                connection = CreateConnectFactory().CreateConnection();
                channel = connection.CreateModel();
                channel.ExchangeDeclare(MQ_DLX_EXCHANGE, ExchangeType.Direct, true, true, null);
                channel.QueueDeclare(MQ_DLX_QUEUE, true, false, true, exchangeArg);
                channel.QueueBind(MQ_DLX_QUEUE, MQ_DLX_EXCHANGE, MQ_DLX_ROUTEKEY);
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2;
                properties.Expiration = Message.MessageTTL.ToString();
                var headers = new Dictionary<string, object>();
                headers.Add("x-match", "any");
                headers.Add("RouteKey", Message.ConsumerRouingKey);
                properties.Headers = headers;
                var msgBytes = JSONHelper.SerializeToByte(Message);
                channel.BasicPublish(MQ_DLX_EXCHANGE, MQ_DLX_ROUTEKEY, properties, msgBytes);
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
