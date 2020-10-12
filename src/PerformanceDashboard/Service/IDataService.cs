using PerformanceDashboard.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PerformanceDashboard.Service
{
    public interface IDataService
    {
        Task<SortedDictionary<DateTime, IList<Run>>> GetTestRunsForScenario(string scenarioName, bool sortAscending);
        IList<DateTime> GetTestRunDates();
        Task<SortedDictionary<DateTime, IList<Run>>> GetTestRuns(bool sortAscending);
        IList<string> GetScenarioNames();
        Task<IList<ScenarioRun>> GetScenarioRuns(bool sortAscending);
        Task<IList<ScenarioRun>> GetScenarioRuns(string scenarioName, bool sortAscending);
        Task<Dictionary<string, string>> GetConfiguration();
        Task<IList<Scenario>> GetScenarios();
        Task<Scenario> GetScenario(string scenarioName);
        double GetTolerance(double testResult);
        LastRunStatus GetLastRunStatus(double testResult, double kpi);
        Change GetChange(double lastValue, double previousValue);
        LastTwoRuns GetLastTwoRunComparison(double lastValue, double previousValue, double kpi);
    }
}
