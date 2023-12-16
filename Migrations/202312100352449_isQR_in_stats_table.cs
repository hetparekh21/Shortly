namespace Shortly.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class isQR_in_stats_table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stats", "isQR", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stats", "isQR");
        }
    }
}
