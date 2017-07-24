﻿using Entity.EF;
using IRepository.MongoDB;
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
    public class UserConsumer : MQBase
    {
        public IConnection connection;
        public IModel channel;
        public EventingBasicConsumer consumer;
        public IUserDetailRepository _IUserDetailRepository;

        public UserConsumer(IUserDetailRepository _IUserDetailRepository) {
            this._IUserDetailRepository = _IUserDetailRepository;
        }

        public void Sub() {
            try {
                var queueArg = new Dictionary<string, object>();  
                queueArg.Add("x-dead-letter-exchange", MQ_DLX_EXCHANGE);//过期消息转向路由  
                queueArg.Add("x-dead-letter-routing-key", MQ_DLX_ROUTEKEY);//过期消息转向路由相匹配routingkey, 如果不指定沿用死信队列的routingkey
                Subscribe(ref connection, ref channel, MQ_USER_EXCHANGE, MQ_USER_QUEUE, MQ_USER_ROUTEKEY, queueArg);
                var headers = new Dictionary<string, object>();
                //all:表示所有的键值对都匹配才能接受到消息  
                //any:表示只要有键值对匹配就能接受到消息 
                headers.Add("x-match", "all");
                headers.Add("RouteKey", MQ_USER_ROUTEKEY);
                channel.QueueBind(MQ_USER_QUEUE, MQ_USER_EXCHANGE, MQ_USER_ROUTEKEY, headers);
                consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    try {
                        var a = int.Parse("12w");
                        var userInfo = JSONHelper.DeserializeObject<MQMessage<UserInfo>>(ea.Body);
                        try {
                            if (_IUserDetailRepository.Add(userInfo.Data.UserID, userInfo.Data.UserName))
                            {
                                channel.BasicAck(ea.DeliveryTag, false);
                            }
                            else
                            {
                                //channel.BasicNack(ea.DeliveryTag, false, false);
                                Reject(channel, ea);
                            }
                        }
                        catch (Exception ex)
                        {
                            //channel.BasicNack(ea.DeliveryTag, false, false);
                            Reject(channel, ea);
                        }
                    }
                    catch (Exception ex) {
                        //channel.BasicNack(ea.DeliveryTag, false, false);
                        Reject(channel, ea);
                    }
                    
                };
                channel.BasicConsume(MQ_USER_QUEUE, false, consumer);
            }
            catch (Exception ex) {
            }
        }

    }
}
