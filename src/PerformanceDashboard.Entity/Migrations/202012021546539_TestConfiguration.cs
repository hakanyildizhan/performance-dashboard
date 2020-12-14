namespace PerformanceDashboard.Entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TestConfiguration : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Configuration", newName: "Settings");
            CreateTable(
                "dbo.TestConfiguration",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            AddColumn("dbo.TestRun", "Configuration_Id", c => c.Int());
            CreateIndex("dbo.TestRun", "Configuration_Id");
            AddForeignKey("dbo.TestRun", "Configuration_Id", "dbo.TestConfiguration", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TestRun", "Configuration_Id", "dbo.TestConfiguration");
            DropIndex("dbo.TestRun", new[] { "Configuration_Id" });
            DropIndex("dbo.TestConfiguration", new[] { "Name" });
            DropColumn("dbo.TestRun", "Configuration_Id");
            DropTable("dbo.TestConfiguration");
            RenameTable(name: "dbo.Settings", newName: "Configuration");
        }
    }
}
