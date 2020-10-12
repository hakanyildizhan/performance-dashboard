using PerformanceDashboard.Entity.Entity;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace PerformanceDashboard.Entity
{
    public class DashboardContext : DbContext
    {
        public DashboardContext() : base("DashboardContext")
        {
        }

        public DbSet<TestScenario> TestScenarios { get; set; }
        public DbSet<TestRun> TestRuns { get; set; }
        public DbSet<Configuration> Config { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}
