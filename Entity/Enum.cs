using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public enum MQMessageType
    {
        [Description("处理成功")]
        UNACCEPT = 1,
        [Description("处理成功")]
        ACCEPT = 2,
        [Description("可以重试的错误")]
        RETRY = 3,
        [Description("无需重试的错误")]
        REJECT = 4
    }
}
