namespace WEB.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "CodeFirst.UserInfo",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        UserID = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(maxLength: 50),
                        Gender = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserID);
            
        }
        
        public override void Down()
        {
            DropTable("CodeFirst.UserInfo");
        }
    }
}
