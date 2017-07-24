using Entity.EF;
using IRepository.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.EF
{
    public class UserRepository : BaseRepository<UserInfo>, IUserRepository
    {
        public int Counts()
        {
            return Count();
        }

        public UserInfo Register(string UserName, bool Gender, int Age)
        {
            var entity = new UserInfo
            {
                UserID = Guid.NewGuid().ToString(),
                UserName = UserName,
                Gender = Gender,
                Age = Age,
                CreateTime = DateTime.Now,
                LastUpdateTime = DateTime.Now
            };
            if (Add(entity))
            {
                return entity;
            }
            else {
                return null;
            }
        }
    }
}
