using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.MongoDB
{

    public interface IEntityBase<T>
    {
        T ID { get; }
    }
    public abstract class AggregateBase : IEntityBase<Guid>
    {
        public Guid ID { get; set; }

        public AggregateBase()
        {
            ID = Guid.NewGuid();
        }
    }

}
