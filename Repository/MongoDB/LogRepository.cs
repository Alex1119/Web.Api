using IRepository.MongoDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.MongoDB;
using Service.MQ;
using Entity.EF;

namespace Repository.MongoDB
{
    public class LogRepository : ILogRepository
    {
        #region
        public const string MONGODB_CONFIG_SELECTION_KEY = "MongoConfig";
        public const string EXPECTION_LOG_COLLECTION_NAME = "EXPECTION_LOG";
        public const string MQ_LOG_COLLECTION_NAME = "MQ_LOG";

        private DoMongoRepoistory<ExceptionLog> _ExceptionRepostory
        {
            get { return new DoMongoRepoistory<ExceptionLog>(MONGODB_CONFIG_SELECTION_KEY, EXPECTION_LOG_COLLECTION_NAME); }
        }
        private DoMongoRepoistory<MQMessage<UserInfo>> _MQLogRepostory
        {
            get { return new DoMongoRepoistory<MQMessage<UserInfo>>(MONGODB_CONFIG_SELECTION_KEY, MQ_LOG_COLLECTION_NAME); }
        }

        #endregion

        public void AddExpection(ExceptionLog log)
        {
            try
            {
                _ExceptionRepostory.Add(log);
            } catch (Exception ex) {
            }
        }

        public void AddMQLog(MQMessage<UserInfo> log) {
            try
            {
                log.CreateTime = DateTime.Now;
                _MQLogRepostory.Add(log);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
