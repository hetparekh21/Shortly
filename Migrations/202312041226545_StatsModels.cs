namespace Shortly.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StatsModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Stats",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BrowserType = c.String(),
                        IpAddress = c.String(),
                        Location = c.String(),
                        DeviceType = c.String(),
                        HitAt = c.DateTime(nullable: false),
                        Url_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Url", t => t.Url_Id, cascadeDelete: true)
                .Index(t => t.Url_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Stats", "Url_Id", "dbo.Url");
            DropIndex("dbo.Stats", new[] { "Url_Id" });
            DropTable("dbo.Stats");
        }
    }
}
