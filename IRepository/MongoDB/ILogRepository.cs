using Entity.EF;
using Entity.MongoDB;
using Service.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepository.MongoDB
{
    public interface ILogRepository
    {
        void AddExpection(ExceptionLog log);
        void AddMQLog(MQMessage<UserInfo> log);
    }
}
