using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Service.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace Service.MQ
{
    public class DLXConsumer : MQBase
    {
        public IConnection connection;
        public IModel channel;
        public EventingBasicConsumer consumer;

        public void Sub()
        {
            try
            {
                var queueArg = new Dictionary<string, object>();
                //queueArg.Add("x-message-ttl", 10000);//队列上消息过期时间，应小于队列过期时间
                queueArg.Add("x-dead-letter-exchange", MQ_USER_EXCHANGE);//过期消息转向路由  
                queueArg.Add("x-dead-letter-routing-key", MQ_USER_ROUTEKEY);//过期消息转向路由相匹配routingkey, 如果不指定沿用死信队列的routingkey
                Subscribe(ref connection, ref channel, MQ_DLX_EXCHANGE, MQ_DLX_QUEUE, MQ_DLX_ROUTEKEY, null, queueArg);
                consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        var rePubList = (List<object>)ea.BasicProperties.Headers["x-death"];
                        var reQueueCount = rePubList.Count;
                        if (reQueueCount >= 10)
                        {
                            channel.BasicAck(ea.DeliveryTag, false);
                        }
                        else {
                            var unixTimeStamp = ((AmqpTimestamp)(((IDictionary<string, object>)rePubList.FirstOrDefault())["time"])).UnixTime;
                            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
                            DateTime latestReQueueTime = startTime.AddSeconds(unixTimeStamp);
                            if (DateTime.Now >= latestReQueueTime.AddMilliseconds(5000))
                            {
                                channel.BasicReject(ea.DeliveryTag, false);
                            }
                            else
                            {
                                channel.BasicReject(ea.DeliveryTag, true);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        channel.BasicNack(ea.DeliveryTag, false, false);
                    }

                };
                channel.BasicConsume(MQ_DLX_QUEUE, false, consumer);
            }
            catch (Exception ex)
            {
            }
        }

    }
}
