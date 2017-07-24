using Entity.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepository.EF
{
    public interface IUserRepository
    {

        int Counts();

        UserInfo Register(string UserName, bool Gender, int Age);
    }
}
