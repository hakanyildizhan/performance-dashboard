using System.Collections.Generic;

namespace PerformanceDashboard.Model
{
    public class TableModel
    {
        public IList<string> ScenarioNames { get; set; }
        public IList<ScenarioRun> Runs { get; set; }
    }
}