﻿using Entity.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IService
{
    public interface IUserService
    {
        int CountsFromSQLServer();

        int CountsFromMongo();

        int CountsFromRedis();

        UserInfo Register(string UserName, bool? Gender, int? Age);
        
      
    }
}