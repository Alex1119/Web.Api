namespace Entity.EF
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class EFContext : DbContext
    {
        public EFContext()
            : base("name=CodeFirstEntities")
        {
        }

        public virtual DbSet<UserInfo> UserInfos { get; set; }

    }
    
}