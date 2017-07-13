using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.MongoDB
{
    public class UserDetailEntity : AggregateBase
    {
        
        public Guid UserId { get; set; }

        public string UserName { get; set; }
    }
}
