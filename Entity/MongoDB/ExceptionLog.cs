using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.MongoDB
{
    public class ExceptionLog : AggregateBase
    {
        public string ExceptionType { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime RequestTime { get; set; }

        public string AppendInfo { get; set; }

        public ExceptionLog(Exception ex, string appendInfo = null)
        {
            ExceptionType = ex.GetType().ToString();
            Message = ex.Message;
            StackTrace = ex.StackTrace;
            RequestTime = DateTime.Now;
            AppendInfo = appendInfo;
        }
    }
}
