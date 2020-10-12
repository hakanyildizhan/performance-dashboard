using System;
using System.Collections.Generic;

namespace PerformanceDashboard.Model
{
    public class DashboardModel
    {
        public string ProjectName { get; set; }
        public IList<Scenario> Scenarios { get; set; }
        public SortedDictionary<DateTime, IList<Run>> TestRuns { get; set; }
    }
}