using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.MQ
{
    [Serializable]
    public class MQMessage<T>
    {
        //该条消息需要被重新分发
        public MQMessageType Rediliver { get; set; }
        //消息过期时间
        public long MessageTTL { get; set; }
        //过期消息转向的Exchange
        public string ConsumerExchange { get; set; }
        //过期消息转向的Exchange
        public string ConsumerQueue { get; set; }
        //转向路由的RoutingKey
        public string ConsumerRouingKey { get; set; }
        //消息主体
        public T Data { get; set; }

        public MQMessage(long MessageTTL, string ConsumerExchange, string ConsumerQueue, string ConsumerRouingKey, T Data){
            this.Rediliver = MQMessageType.UNACCEPT;
            this.MessageTTL = MessageTTL;
            this.ConsumerExchange = ConsumerExchange;
            this.ConsumerQueue = ConsumerQueue;
            this.ConsumerRouingKey = ConsumerRouingKey;
            this.Data = Data;
        }
    }
}
