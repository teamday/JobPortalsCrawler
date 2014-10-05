namespace crawler.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPortal : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Portals",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.id);
            
            AddColumn("dbo.Jobs", "idPortal", c => c.Int());
            CreateIndex("dbo.Jobs", "idPortal");
            AddForeignKey("dbo.Jobs", "idPortal", "dbo.Portals", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Jobs", "idPortal", "dbo.Portals");
            DropIndex("dbo.Jobs", new[] { "idPortal" });
            DropColumn("dbo.Jobs", "idPortal");
            DropTable("dbo.Portals");
        }
    }
}
