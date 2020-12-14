using PerformanceDashboard.Entity;
using PerformanceDashboard.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace PerformanceDashboard.Service
{
    public class RealtimeDataService : IDataService
    {
        private readonly DashboardContext _db;

        public RealtimeDataService(DashboardContext db)
        {
            _db = db;
        }

        public async Task<Dictionary<string, string>> GetSettings()
        {
            var getSettings = _db.Settings.ToListAsync();
            var result = new Dictionary<string, string>();
            (await getSettings).ForEach(c => result.Add(c.Key, c.Value));
            return result;
        }

        public async Task<IList<Scenario>> GetScenarios(int configurationId)
        {
            var scenariosEntity = await _db.TestScenarios.ToListAsync();

            if (configurationId == 0)
            {
                configurationId = _db.TestConfigurations.First().Id;
            }

            var testDates = GetTestRunDates(configurationId);
            var scenarios = new List<Scenario>();

            foreach (var scenarioEntity in scenariosEntity)
            {
                var scenario = new Scenario();
                scenario.KPI = scenarioEntity.KPI;
                scenario.Name = scenarioEntity.Name;
                var testRunsForScenario = await GetTestRunsForScenario(configurationId, scenarioEntity.Name, false);
                if (testRunsForScenario.Count > 1 && testDates.Count > 1)
                {
                    var runResults = GetLastTwoTestRunResultsForScenario(scenarioEntity.Name, testDates[testDates.Count - 1], testDates[0]);
                    var lastTwoRuns = GetLastTwoRunComparison(runResults.Item1, runResults.Item2, scenarioEntity.KPI);
                    BuildScenarioLastRunData(scenario, lastTwoRuns);
                }
                scenarios.Add(scenario);
            }
            return scenarios;
        }

        public async Task<Scenario> GetScenario(int configurationId, string scenarioName)
        {
            var scenarioEntity = _db.TestScenarios.Where(s => s.Name.Equals(scenarioName)).FirstOrDefault();
            if (scenarioEntity == null)
            {
                return null;
            }

            var getRuns = GetTestRunsForScenario(configurationId, scenarioEntity.Name, false);
            var scenario = new Scenario();
            scenario.KPI = scenarioEntity.KPI;
            scenario.Name = scenarioEntity.Name;
            var runs = (await getRuns);
            if (runs.Count > 1)
            {
                double lastRunResult = runs.ElementAt(0).Value.First().Result;
                double previousRunResult = runs.ElementAt(1).Value.First().Result;
                var lastTwoRuns = GetLastTwoRunComparison(lastRunResult, previousRunResult, scenarioEntity.KPI);
                BuildScenarioLastRunData(scenario, lastTwoRuns);
            }

            return scenario;
        }

        private void BuildScenarioLastRunData(Scenario scenario, LastTwoRuns lastTwoRuns)
        {
            switch (lastTwoRuns.Change.Direction)
            {
                case Direction.Better:
                    scenario.ChangeDirection = '↗';
                    break;
                case Direction.Worse:
                    scenario.ChangeDirection = '↘';
                    break;
                case Direction.None:
                    scenario.ChangeDirection = '→';
                    break;
                default:
                    break;
            }

            scenario.PercentageChange = lastTwoRuns.Change.PercentageChange;

            switch (lastTwoRuns.Change.PerformanceChange)
            {
                case PerformanceChange.Decrease:
                    scenario.PercentageChangeIndicatorColor = "#ffbd9a";
                    break;
                case PerformanceChange.Increase:
                    scenario.PercentageChangeIndicatorColor = "#e3ffd5";
                    break;
                case PerformanceChange.Horizontal:
                    scenario.PercentageChangeIndicatorColor = "#ffffff";
                    break;
                default:
                    break;
            }

            scenario.LastRunStatus = lastTwoRuns.LastRunStatus;

            switch (lastTwoRuns.LastRunStatus)
            {
                case LastRunStatus.Pass:
                    scenario.LastRunIndicatorColor = "#78ff00";
                    break;
                case LastRunStatus.Fail:
                    scenario.LastRunIndicatorColor = "#ff8454";
                    break;
                case LastRunStatus.OK:
                    scenario.LastRunIndicatorColor = "#feff0e";
                    break;
                default:
                    break;
            }
        }

        public IList<DateTime> GetTestRunDates(int configurationId)
        {
            int daysToShow = GetDaysToShow();
            return _db.TestRuns.Where(r => r.Configuration.Id == configurationId).ToList()
                .Select(d => d.Date.Date)
                .Distinct()
                .OrderByDescending(d => d)
                .Take(daysToShow)
                .ToList();
        }

        public async Task<SortedDictionary<DateTime, IList<Run>>> GetTestRunsForScenario(int configurationId, string scenarioName, bool sortAscending)
        {
            var testRunsForScenario = await _db.TestRuns
                .Where(r => r.Scenario.Name.Equals(scenarioName) && r.Configuration.Id == configurationId)
                .ToListAsync();

            var testRuns = testRunsForScenario.GroupBy(g => g.Date.Date)
                .Select(r => new
                {
                    Scenario = r.OrderByDescending(a => a.Date).FirstOrDefault().Scenario,
                    Result = r.OrderByDescending(a => a.Date).FirstOrDefault().Result,
                    Date = r.Key
                }).ToList();

            IComparer<DateTime> comparer = sortAscending ?
                    Comparer<DateTime>.Create((x, y) => x.CompareTo(y)) :
                    Comparer<DateTime>.Create((x, y) => y.CompareTo(x));

            var result = new SortedDictionary<DateTime, IList<Run>>(comparer);

            foreach (var testRun in testRuns)
            {
                result.Add(testRun.Date, new List<Run>() 
                { 
                    new Run 
                    { 
                        Scenario = testRun.Scenario.Name, 
                        Result = testRun.Result
                    } 
                });
            }

            return result;
        }

        public async Task<SortedDictionary<DateTime, IList<Run>>> GetTestRuns(int configurationId, bool sortAscending)
        {
            if (configurationId == 0)
            {
                configurationId = _db.TestConfigurations.First().Id;
            }

            var runDates = GetTestRunDates(configurationId);
            var testRuns = await _db.TestRuns.ToListAsync();
            IList<string> availableScenarios = await _db.TestScenarios.Select(s => s.Name).ToListAsync();
            IComparer<DateTime> comparer = sortAscending ?
                    Comparer<DateTime>.Create((x, y) => x.CompareTo(y)) :
                    Comparer<DateTime>.Create((x, y) => y.CompareTo(x));

            var result = new SortedDictionary<DateTime, IList<Run>>(comparer);

            foreach (var runDate in runDates)
            {
                var testRunsForDate = testRuns.Where(r => r.Date.Date.Equals(runDate))
                    .GroupBy(g => g.Scenario)
                    .Select(r => new Entity.Entity.TestRun
                    {
                        Scenario = r.Key,
                        Result = r.OrderByDescending(a => a.Date).First().Result,
                        Date = r.Max(x => x.Date)
                    }).ToList();

                var runs = new List<Run>();

                foreach (var testRunForDate in testRunsForDate)
                {
                    runs.Add(new Run()
                    {
                        Scenario = testRunForDate.Scenario.Name,
                        Result = testRunForDate.Result
                    });
                }

                // if a scenario was not run at this date, insert 0
                foreach (string scenario in availableScenarios)
                {
                    if (!runs.Any(r => r.Scenario.Equals(scenario)))
                    {
                        runs.Add(new Run() { Scenario = scenario, Result = 0 });
                    }
                }

                result.Add(runDate, runs);
            }

            return result;
        }

        public IList<string> GetScenarioNames()
        {
            return _db.TestScenarios.Select(s => s.Name).Distinct().OrderBy(name => name).ToList();
        }

        public async Task<IList<ScenarioRun>> GetScenarioRuns(int configurationId, bool sortAscending)
        {
            var testRuns = await GetTestRuns(configurationId, sortAscending);
            var runs = new List<ScenarioRun>();

            foreach (var run in testRuns)
            {
                runs.Add(new ScenarioRun 
                { 
                    Date = run.Key.ToTurkishString(), 
                    Results = run.Value.OrderBy(r => r.Scenario).Select(r => r.Result).ToList() 
                });
            }

            return runs;
        }

        public async Task<IList<ScenarioRun>> GetScenarioRuns(int configurationId, string scenarioName, bool sortAscending)
        {
            var testRuns = await GetTestRunsForScenario(configurationId, scenarioName, sortAscending);
            var runs = new List<ScenarioRun>();

            foreach (var run in testRuns)
            {
                runs.Add(new ScenarioRun 
                { 
                    Date = run.Key.ToTurkishString(), 
                    Results = run.Value.OrderBy(r => r.Scenario).Select(r => r.Result).ToList() 
                });
            }

            return runs;
        }

        public double GetTestRunResultForScenario(DateTime date, string scenarioName)
        {
            var run = _db.TestRuns.Where(r => r.Scenario.Name.Equals(scenarioName) && r.Date.Equals(date)).FirstOrDefault();
            return run == null ? 0 : run.Result;
        }

        public double GetTolerance(double testResult)
        {
            if (testResult > 20)
            {
                return 0.10;
            }
            else if (testResult <= 20 && testResult > 10)
            {
                return 0.5;
            }
            else if (testResult <= 10 && testResult > 2)
            {
                return 0.3;
            }
            else if (testResult < 2 && testResult > 0)
            {
                return 0.4;
            }
            else
            {
                return 0;
            }
        }

        private LastRunStatus GetLastRunStatus(double testResult, double kpi)
        {
            double tolerance = GetTolerance(testResult);

            if (testResult < 2)
            {
                if (testResult >= (kpi * (tolerance + 1)))
                {
                    return LastRunStatus.Fail;
                }
                else
                {
                    return LastRunStatus.Pass;
                }
            }
            else
            {
                if (testResult >= (kpi * (tolerance + 1)))
                {
                    return LastRunStatus.Fail;
                }
                else if (testResult < (kpi * (tolerance + 1)) && testResult > (kpi * (1 - tolerance)))
                {
                    return LastRunStatus.OK;
                }
                else
                {
                    return LastRunStatus.Pass;
                }
            }
        }

        private Change GetChange(double lastValue, double previousValue)
        {
            double percentageChange = 0;
            PerformanceChange performanceChange = PerformanceChange.Horizontal;
            var change = lastValue / previousValue;
            var tolerance = GetTolerance(lastValue);
            Direction direction = Direction.None;

            if (!(lastValue == 0 || previousValue == 0))
            {
                if (change < 1)
                {
                    percentageChange = (1 - change)*100;
                }
                else
                {
                    percentageChange = (change - 1)*100;
                }

                percentageChange = (float)Math.Round(percentageChange, 2);

                if (!(lastValue < (previousValue * (tolerance + 1)) && lastValue > (previousValue * (1 - tolerance))))
                {
                    if (change > 1)
                    {
                        performanceChange = PerformanceChange.Decrease;
                        direction = Direction.Worse;
                    }
                    else if (change == 1)
                    {
                        performanceChange = PerformanceChange.Horizontal;
                        direction = Direction.None;
                    }
                    else
                    {
                        performanceChange = PerformanceChange.Increase;
                        direction = Direction.Better;
                    }
                }
                else
                {
                    if (change > 1)
                    {
                        performanceChange = PerformanceChange.Horizontal;
                        direction = Direction.Worse;
                    }
                    else if (change == 1)
                    {
                        performanceChange = PerformanceChange.Horizontal;
                        direction = Direction.None;
                    }
                    else
                    {
                        performanceChange = PerformanceChange.Horizontal;
                        direction = Direction.Better;
                    }
                }
            }
            else
            {
                performanceChange = PerformanceChange.Horizontal;
                percentageChange = 0;
                direction = Direction.None;
            }

            return new Change() { Direction = direction, PercentageChange = Math.Round(percentageChange,2), PerformanceChange = performanceChange };
        }

        private LastTwoRuns GetLastTwoRunComparison(double lastValue, double previousValue, double kpi)
        {
            var comparison = new LastTwoRuns();
            comparison.LastRunStatus = GetLastRunStatus(lastValue, kpi);
            comparison.Change = GetChange(lastValue, previousValue);
            return comparison;
        }

        public IList<Configuration> GetConfigurations()
        {
            return _db.TestConfigurations
                .Select(c => new Configuration() { Id = c.Id, Name = c.Name })
                .ToList();
        }

        /// <summary>
        /// Gets last two runtimes for a scenario between given two dates.
        /// </summary>
        /// <param name="scenario"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private Tuple<double,double> GetLastTwoTestRunResultsForScenario(string scenario, DateTime startDate, DateTime endDate)
        {
            var runs = _db.TestRuns.Where(r => r.Scenario.Name.Equals(scenario) && r.Date >= startDate && r.Date <= endDate).OrderByDescending(r => r.Date).ToList();

            if (runs.Any())
            {
                if (runs.Count == 1)
                {
                    return new Tuple<double, double>(runs[0].Result, 0);
                }
                else
                {
                    return new Tuple<double, double>(runs[0].Result, runs[1].Result);
                }
            }

            return new Tuple<double, double>(0, 0);
        }

        private int GetDaysToShow()
        {
            var daysToShowSetting = _db.Settings.FirstOrDefault(s => s.Key == AppConstants.DAYS_TO_SHOW);
            return daysToShowSetting != null ? Convert.ToInt32(daysToShowSetting.Value) : AppConstants.DAYS_TO_SHOW_DEFAULT;
        }
    }
}