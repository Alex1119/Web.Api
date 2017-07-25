using Entity.EF;
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

        public void Pub<T>(T Data)
        {
            var userInfo = Data as UserInfo;
            if (userInfo != null)
            {
                Pub<UserInfo>(new MQMessage<UserInfo>(MQ_USER_EXCHANGE, MQ_USER_QUEUE, MQ_USER_ROUTEKEY, userInfo));
            }
        }
        
    }
}
