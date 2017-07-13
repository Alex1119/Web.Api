using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepository.cs.MongoDB
{
    public interface IUserDetailRepository
    {

        int Counts();

        bool Add(string UserID, string UserName);

    }
}
