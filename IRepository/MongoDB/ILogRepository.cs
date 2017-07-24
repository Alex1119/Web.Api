using Entity.MongoDB;
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

    }
}
