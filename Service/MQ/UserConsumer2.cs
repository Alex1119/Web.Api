using Entity.EF;
using IRepository.cs.MongoDB;
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
    public class UserConsumer2: MQBase
    {
        public IConnection connection;
        public IModel channel;
        public EventingBasicConsumer consumer;
        public IUserDetailRepository _IUserDetailRepository;

        public UserConsumer2(IUserDetailRepository _IUserDetailRepository)
        {
            this._IUserDetailRepository = _IUserDetailRepository;
        }

        public void Sub()
        {
            try
            {
                Subscribe(ref connection, ref channel, MQ_USER_EXCHANGE, MQ_USER_QUEUE2);
                var headers = new Dictionary<string, object>();
                //all:表示所有的键值对都匹配才能接受到消息  
                //any:表示只要有键值对匹配就能接受到消息 
                headers.Add("x-match", "any");
                headers.Add("RouteKey", MQ_USER_ROUTEKEY2);
                channel.QueueBind(MQ_USER_QUEUE2, MQ_USER_EXCHANGE, MQ_USER_ROUTEKEY, headers);
                consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        var userInfo = JSONHelper.DeserializeObject<DLXMessage<UserInfo>>(ea.Body);
                        try
                        {
                            //if (_IUserDetailRepository.Add(userInfo.Data.UserID, userInfo.Data.UserName))
                            //{
                            //    channel.BasicAck(ea.DeliveryTag, false);
                            //}
                            //else
                            //{
                            //channel.BasicReject(ea.DeliveryTag, true);
                            new DLXProducter().RePub(userInfo);
                            channel.BasicAck(ea.DeliveryTag, false);
                            //}
                        }
                        catch (Exception ex)
                        {
                            //channel.BasicReject(ea.DeliveryTag, true);
                            new DLXProducter().RePub(userInfo);
                            channel.BasicAck(ea.DeliveryTag, false);
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                };
                channel.BasicConsume(MQ_USER_QUEUE2, false, consumer);
            }
            catch (Exception ex)
            {
            }
        }

    }
}
