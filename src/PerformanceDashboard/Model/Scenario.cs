namespace PerformanceDashboard.Model
{
    public class Scenario
    {
        public string Name { get; set; }
        public double KPI { get; set; }
        public double PercentageChange { get; set; }
        public string PercentageChangeIndicatorColor { get; set; }
        public char ChangeDirection { get; set; }
        public LastRunStatus LastRunStatus { get; set; }
        public string LastRunIndicatorColor { get; set; }
    }
}