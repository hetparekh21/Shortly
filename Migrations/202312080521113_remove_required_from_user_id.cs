namespace Shortly.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_required_from_user_id : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Url", "User_id", c => c.String(maxLength: 128));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Url", "User_id", c => c.String(nullable: false, maxLength: 128));
        }
    }
}
