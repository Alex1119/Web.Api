using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace MQConsumer.Config
{

    [XmlRoot("Root")]
    public class MQRetryQueueInfo
    {

        [XmlElement("item")]
        public List<MQRetryQueueItem> Items { get; set; }

    }

    public class MQRetryQueueItem {

        [XmlAttribute("RejectTime")]
        public int RejectTime { get; set; }

        [XmlAttribute("QueueName")]
        public string QueueName { get; set; }

        [XmlAttribute("TTL")]
        public long TTL { get; set; }

    }
}