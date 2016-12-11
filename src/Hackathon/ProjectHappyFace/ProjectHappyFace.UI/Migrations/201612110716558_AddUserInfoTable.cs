namespace ProjectHappyFace.UI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserInfoTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationUserInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Age = c.Int(nullable: false),
                        Gender = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AspNetUsers", "UserInfo_Id", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "UserInfo_Id");
            AddForeignKey("dbo.AspNetUsers", "UserInfo_Id", "dbo.ApplicationUserInfoes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "UserInfo_Id", "dbo.ApplicationUserInfoes");
            DropIndex("dbo.AspNetUsers", new[] { "UserInfo_Id" });
            DropColumn("dbo.AspNetUsers", "UserInfo_Id");
            DropTable("dbo.ApplicationUserInfoes");
        }
    }
}
