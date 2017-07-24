using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace Service.MQ
{
    public class MQBase
    {
        #region
        string hostName = ConfigurationManager.AppSettings["MQHostName"];
        int port = int.Parse(ConfigurationManager.AppSettings["MQPort"]);

        public const string MQ_DLX_EXCHANGE = "MQ_DLX_EXCHANGE";
        public const string MQ_DLX_QUEUE = "MQ_DLX_QUEUE";
        public const string MQ_DLX_ROUTEKEY = "MQ_DLX_ROUTEKEY";

        public const string MQ_USER_EXCHANGE = "MQ_USER_EXCHANGE";
        public const string MQ_USER_QUEUE = "MQ_USER_QUEUE";
        public const string MQ_USER_ROUTEKEY = "MQ_USER_ROUTEKEY";
        #endregion

        protected ConnectionFactory CreateConnectFactory()
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = hostName,
                Port = port,
                UserName = "guest",
                Password = "guest",
                Protocol = Protocols.AMQP_0_9_1,
                RequestedFrameMax = UInt32.MaxValue,
                RequestedHeartbeat = UInt16.MaxValue,
                AutomaticRecoveryEnabled = true
            };
            return connectionFactory;
        }

        public void Subscribe(ref IConnection Connection, ref IModel Channel, string Exchange, string Queue, string Routekey,Dictionary<string, object> queueArg = null) {
            Connection = CreateConnectFactory().CreateConnection();
            Channel = Connection.CreateModel();
            Channel.BasicQos(0, 1, false);
            Channel.ExchangeDeclare(Exchange, ExchangeType.Headers, true, false, null);
            Channel.QueueDeclare(Queue, true, false, false, queueArg);
            Channel.QueueBind(Queue, Exchange, Routekey);
        }

        public void UnSubscribe(IConnection Connection, IModel Channel, EventingBasicConsumer Consumer = null)
        {
            if (Consumer != null)
            {
                Consumer = null;
            }
            if (Channel != null && Channel.IsOpen)
            {
                Channel.Close();
                Channel.Dispose();
            }
            if (Connection != null && Connection.IsOpen)
            {
                Connection.Close();
                Connection.Dispose();
            }
        }

        public void Pub<T>(MQMessage<T> message)
        {
            IConnection connection = null;
            IModel channel = null;
            try
            {
                var queueArg = new Dictionary<string, object>();
                //exchangeArg.Add("x-message-ttl", message.MessageTTL);//队列上消息过期时间，应小于队列过期时间  
                queueArg.Add("x-dead-letter-exchange", MQ_DLX_EXCHANGE);//过期消息转向路由
                queueArg.Add("x-dead-letter-routing-key", MQ_DLX_ROUTEKEY);//过期消息转向路由相匹配routingkey, 如果不指定沿用死信队列的routingkey
                Subscribe(ref connection, ref channel, message.ConsumerExchange, message.ConsumerQueue, message.ConsumerRouingKey, queueArg);
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2;//数据持久化
                var headers = new Dictionary<string, object>();
                headers.Add("x-match", "all");
                headers.Add("RouteKey", message.ConsumerRouingKey);
                properties.Headers = headers;
                var msgBytes = JSONHelper.SerializeToByte(message);
                channel.BasicPublish(message.ConsumerExchange, message.ConsumerRouingKey, properties, msgBytes);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                UnSubscribe(connection, channel);
            }
        }

        public void Reject(IModel channel, BasicDeliverEventArgs ea)
        {
            var rePubList = new List<object>();
            if (ea.BasicProperties.Headers.Keys.Contains("x-death")) {
                rePubList = (List<object>)ea.BasicProperties.Headers["x-death"];
            };
            if (rePubList.Count() < 10) //死信队列中以过期形式重新转向，所以也会增加一次 x-dead 次数
            {
                channel.BasicNack(ea.DeliveryTag, false, false);
            }
            else {
                channel.BasicAck(ea.DeliveryTag, false);
            }
        }
    }
}
