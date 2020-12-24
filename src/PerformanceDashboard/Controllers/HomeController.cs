using PerformanceDashboard.Model;
using PerformanceDashboard.Service;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PerformanceDashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDataService _dataService;
        public HomeController(IDataService dataService)
        {
            _dataService = dataService;
        }

        public ActionResult Index()
        {
            return View("Dashboard");
        }

        public async Task<JsonResult> GetConfigurationData(int configurationId = 0)
        {
            var settings = await _dataService.GetSettings();
            var scenarios = await _dataService.GetScenarios(configurationId);
            var testRuns = await _dataService.GetTestRuns(configurationId, false);
            var projectName = settings.ContainsKey(AppConstants.PROJECT_NAME) ? settings[AppConstants.PROJECT_NAME] : string.Empty;
            var configurations = _dataService.GetConfigurations();
            var maxResultsToShow = _dataService.GetDaysToShow();

            return Json(new
            {
                ProjectName = projectName,
                Scenarios = scenarios,
                Configurations = configurations,
                TestRuns = testRuns?.ToArray(),
                MaxResultsToShow = maxResultsToShow
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetChartData(int configurationId, string scenario)
        {
            var data = new TableModel();

            if (scenario.Equals("All"))
            {
                data.ScenarioNames = (await _dataService.GetScenarios(configurationId)).Select(s => s.Name).ToList();
                data.Runs = await _dataService.GetScenarioRuns(configurationId, true);
            }
            else
            {
                data.ScenarioNames = new List<string> { scenario };
                data.Runs = await _dataService.GetScenarioRuns(configurationId, scenario, true);
            }

            return Json(data);
        }

        [HttpPost]
        public async Task<JsonResult> SetMaxDaysToShow(int daysToShow)
        {
            await _dataService.SetDaysToShow(daysToShow);
            return Json(new { Status = "OK" });
        }
    }
}