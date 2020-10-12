namespace PerformanceDashboard.Entity.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<PerformanceDashboard.Entity.DashboardContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(PerformanceDashboard.Entity.DashboardContext context)
        {
            //context.Config.AddOrUpdate(c => c.Key,
            //    new Entity.Configuration() { Key = "ProjectName", Value = "" });
        }
    }
}
