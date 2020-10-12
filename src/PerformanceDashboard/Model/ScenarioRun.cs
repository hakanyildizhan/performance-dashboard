using System.Collections.Generic;

namespace PerformanceDashboard.Model
{
    public class ScenarioRun
    {
        public string Date { get; set; }
        public IList<double> Results { get; set; }
    }
}