namespace ProjectHappyFace.UI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProfilePicToUserInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApplicationUserInfoes", "ProfilePicture", c => c.Binary(nullable:true,defaultValue:null));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApplicationUserInfoes", "ProfilePicture");
        }
    }
}
