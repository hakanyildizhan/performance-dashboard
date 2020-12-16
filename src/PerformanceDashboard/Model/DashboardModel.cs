using System;
using System.Collections.Generic;

namespace PerformanceDashboard.Model
{
    public class DashboardModel
    {
        /// <summary>
        /// Name of the project.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// All available scenarios.
        /// </summary>
        public IList<Scenario> Scenarios { get; set; }

        /// <summary>
        /// All distinct test configurations.
        /// </summary>
        public IList<Configuration> Configurations { get; set; }

        /// <summary>
        /// Test results by date.
        /// </summary>
        public SortedDictionary<DateTime, IList<Run>> TestRuns { get; set; }

        /// <summary>
        /// Maximum number of results to show in the chart & results table.
        /// </summary>
        public int MaxResultsToShow { get; set; }
    }
}