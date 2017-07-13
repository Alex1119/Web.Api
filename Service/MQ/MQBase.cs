using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public const string MQ_USER_QUEUE2 = "MQ_USER_QUEUE2";
        public const string MQ_USER_ROUTEKEY2 = "MQ_USER_ROUTEKEY2";
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

        public void Subscribe(ref IConnection Connection, ref IModel Channel, string Exchange, string Queue) {
            Connection = CreateConnectFactory().CreateConnection();
            Channel = Connection.CreateModel();
            Channel.BasicQos(0, 1, false);
            Channel.ExchangeDeclare(Exchange, ExchangeType.Headers, true, false, null);
            Channel.QueueDeclare(Queue, true, false, false, null);
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
    }
}
