namespace Shortly.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_backhalf_to_urlmodels : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Url", "BackHalf", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.Url", "RedirectTo", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Url", "RedirectTo", c => c.String(nullable: false, maxLength: 10));
            DropColumn("dbo.Url", "BackHalf");
        }
    }
}
