using Entity.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepository.Redis
{
    public interface IUserInfoRepository
    {
        UserInfo Register(string UserName, bool Gender, int Age);

    }
}
