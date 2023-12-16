namespace Shortly.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequestAccessModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RequestAccess",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false),
                        Note = c.String(),
                        Granted = c.Boolean(nullable: false),
                        Url_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Url", t => t.Url_Id, cascadeDelete: true)
                .Index(t => t.Url_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RequestAccess", "Url_Id", "dbo.Url");
            DropIndex("dbo.RequestAccess", new[] { "Url_Id" });
            DropTable("dbo.RequestAccess");
        }
    }
}
