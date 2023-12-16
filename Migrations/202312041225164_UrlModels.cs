namespace Shortly.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UrlModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Url",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RedirectTo = c.String(nullable: false, maxLength: 10),
                        Qr = c.Boolean(nullable: false),
                        User_id = c.String(nullable: false, maxLength: 128),
                        Auth = c.Boolean(nullable: false),
                        Password = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Url");
        }
    }
}
