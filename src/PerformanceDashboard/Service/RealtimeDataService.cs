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

        public async Task<Dictionary<string, string>> GetConfiguration()
        {
            var getConfig = _db.Config.ToListAsync();
            var result = new Dictionary<string, string>();
            (await getConfig).ForEach(c => result.Add(c.Key, c.Value));
            return result;
        }

        public async Task<IList<Scenario>> GetScenarios()
        {
            var scenariosEntity = await _db.TestScenarios.ToListAsync();
            var testDates = GetTestRunDates();
            var scenarios = new List<Scenario>();

            foreach (var scenarioEntity in scenariosEntity)
            {
                var scenario = new Scenario();
                scenario.KPI = scenarioEntity.KPI;
                scenario.Name = scenarioEntity.Name;
                var testRunsForScenario = await GetTestRunsForScenario(scenarioEntity.Name, false);
                if (testRunsForScenario.Count > 1 && testDates.Count > 1)
                {
                    double lastRunResult = GetTestRunResultForScenario(testDates[0], scenarioEntity.Name);
                    double previousRunResult = GetTestRunResultForScenario(testDates[1], scenarioEntity.Name);
                    scenario.LastTwoRuns = GetLastTwoRunComparison(lastRunResult, previousRunResult, scenarioEntity.KPI);

                }
                scenarios.Add(scenario);
            }
            return scenarios;
        }

        public async Task<Scenario> GetScenario(string scenarioName)
        {
            var scenarioEntity = _db.TestScenarios.Where(s => s.Name.Equals(scenarioName)).FirstOrDefault();
            if (scenarioEntity == null)
            {
                return null;
            }

            var getRuns = GetTestRunsForScenario(scenarioEntity.Name, false);
            var scenario = new Scenario();
            scenario.KPI = scenarioEntity.KPI;
            scenario.Name = scenarioEntity.Name;
            var runs = (await getRuns);
            if (runs.Count > 1)
            {
                double lastRunResult = runs.ElementAt(0).Value.First().Result;
                double previousRunResult = runs.ElementAt(1).Value.First().Result;
                scenario.LastTwoRuns = GetLastTwoRunComparison(lastRunResult, previousRunResult, scenarioEntity.KPI);
            }

            return scenario;
        }

        public IList<DateTime> GetTestRunDates()
        {
            return _db.TestRuns.ToList().Select(d => d.Date.Date).Distinct().OrderByDescending(d => d).ToList();
        }

        public async Task<SortedDictionary<DateTime, IList<Run>>> GetTestRunsForScenario(string scenarioName, bool sortAscending)
        {
            var testRunsForScenario = await _db.TestRuns.Where(r => r.Scenario.Name.Equals(scenarioName)).ToListAsync();
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
                result.Add(testRun.Date, new List<Run>() { new Run { ScenarioName = testRun.Scenario.Name, Result = testRun.Result } });
            }

            return result;
        }

        public async Task<SortedDictionary<DateTime, IList<Run>>> GetTestRuns(bool sortAscending)
        {
            var runDates = GetTestRunDates();
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
                        ScenarioName = testRunForDate.Scenario.Name,
                        Result = testRunForDate.Result
                    });
                }

                // if a scenario was not run at this date, insert 0
                foreach (string scenario in availableScenarios)
                {
                    if (!runs.Any(r => r.ScenarioName.Equals(scenario)))
                    {
                        runs.Add(new Run() { ScenarioName = scenario, Result = 0 });
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

        public async Task<IList<ScenarioRun>> GetScenarioRuns(bool sortAscending)
        {
            var testRuns = await GetTestRuns(sortAscending);
            var runs = new List<ScenarioRun>();

            foreach (var run in testRuns)
            {
                runs.Add(new ScenarioRun { Date = run.Key.ToTurkishString(), Results = run.Value.OrderBy(r => r.ScenarioName).Select(r => r.Result).ToList() });
            }

            return runs;
        }

        public async Task<IList<ScenarioRun>> GetScenarioRuns(string scenarioName, bool sortAscending)
        {
            var testRuns = await GetTestRunsForScenario(scenarioName, sortAscending);
            var runs = new List<ScenarioRun>();

            foreach (var run in testRuns)
            {
                runs.Add(new ScenarioRun { Date = run.Key.ToTurkishString(), Results = run.Value.OrderBy(r => r.ScenarioName).Select(r => r.Result).ToList() });
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

        public LastRunStatus GetLastRunStatus(double testResult, double kpi)
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

        public Change GetChange(double lastValue, double previousValue)
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


                if (!(lastValue < (previousValue * (tolerance + 1)) && lastValue > (previousValue * (1 - tolerance))))
                {
                    if (change > 1)
                    {
                        performanceChange = PerformanceChange.Decrease;
                        percentageChange = (float)Math.Round(percentageChange, 2);
                        direction = Direction.Worse;
                    }
                    else
                    {
                        performanceChange = PerformanceChange.Increase;
                        percentageChange = (float)Math.Round(percentageChange, 2);
                        direction = Direction.Better;
                    }

                }
                else
                {
                    if (change > 1)
                    {
                        performanceChange = PerformanceChange.Horizontal;
                        percentageChange = (float)Math.Round(percentageChange, 2);
                        direction = Direction.Worse;
                    }
                    else
                    {
                        performanceChange = PerformanceChange.Horizontal;
                        percentageChange = (float)Math.Round(percentageChange, 2);
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

        public LastTwoRuns GetLastTwoRunComparison(double lastValue, double previousValue, double kpi)
        {
            var comparison = new LastTwoRuns();
            comparison.LastRunStatus = GetLastRunStatus(lastValue, kpi);
            comparison.Change = GetChange(lastValue, previousValue);
            return comparison;
        }
    }
}