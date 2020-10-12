namespace PerformanceDashboard.Model
{
    public class Change
    {
        public double PercentageChange { get; set; } // value
        public PerformanceChange PerformanceChange { get; set; } // bg color
        public Direction Direction { get; set; } // arrow
    }
}