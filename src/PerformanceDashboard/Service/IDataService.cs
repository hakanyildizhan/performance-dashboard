using PerformanceDashboard.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PerformanceDashboard.Service
{
    public interface IDataService
    {
        Task<SortedDictionary<DateTime, IList<Run>>> GetTestRunsForScenario(int configurationId, string scenarioName, bool sortAscending);
        IList<DateTime> GetTestRunDates(int configurationId);
        Task<SortedDictionary<DateTime, IList<Run>>> GetTestRuns(int configurationId, bool sortAscending);
        IList<string> GetScenarioNames();
        Task<IList<ScenarioRun>> GetScenarioRuns(int configurationId, bool sortAscending);
        Task<IList<ScenarioRun>> GetScenarioRuns(int configurationId, string scenarioName, bool sortAscending);
        Task<Dictionary<string, string>> GetSettings();
        Task<IList<Scenario>> GetScenarios(int configurationId);
        Task<Scenario> GetScenario(int configurationId, string scenarioName);
        double GetTolerance(double testResult);
        IList<Configuration> GetConfigurations();
    }
}
