namespace PerformanceDashboard.Model
{
    public class Scenario
    {
        public string Name { get; set; }
        public double KPI { get; set; }
        public LastTwoRuns LastTwoRuns { get; set; }
    }
}