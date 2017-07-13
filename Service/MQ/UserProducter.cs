using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace Service.MQ
{
    public class UserProducter :MQBase
    {
        public IConnection connection;
        public IModel channel;

        public void Pub<T>(T message)
        {
            try
            {
                connection = CreateConnectFactory().CreateConnection();
                channel = connection.CreateModel();
                channel.ExchangeDeclare(MQ_USER_EXCHANGE, ExchangeType.Direct, true, true, null);
                channel.QueueDeclare(MQ_USER_QUEUE, true, false, true, null);
                channel.QueueBind(MQ_USER_QUEUE, MQ_USER_EXCHANGE, MQ_USER_ROUTEKEY);
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2;
                var msgBytes = JSONHelper.SerializeToByte(message);
                channel.BasicPublish(MQ_USER_EXCHANGE, MQ_USER_ROUTEKEY, properties, msgBytes);
            }
            catch (Exception ex)
            {
            }
            finally {
                UnSubscribe(connection, channel);
            }
            

        }
        
    }
}
