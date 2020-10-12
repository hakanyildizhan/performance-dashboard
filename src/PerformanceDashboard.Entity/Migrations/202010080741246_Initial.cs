namespace PerformanceDashboard.Entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Configuration",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 128),
                        Value = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Key);
            
            CreateTable(
                "dbo.TestRun",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Result = c.Double(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Scenario_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TestScenario", t => t.Scenario_Id)
                .Index(t => t.Scenario_Id);
            
            CreateTable(
                "dbo.TestScenario",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 150),
                        KPI = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TestRun", "Scenario_Id", "dbo.TestScenario");
            DropIndex("dbo.TestScenario", new[] { "Name" });
            DropIndex("dbo.TestRun", new[] { "Scenario_Id" });
            DropTable("dbo.TestScenario");
            DropTable("dbo.TestRun");
            DropTable("dbo.Configuration");
        }
    }
}
