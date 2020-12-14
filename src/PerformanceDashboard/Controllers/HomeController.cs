using PerformanceDashboard.Model;
using PerformanceDashboard.Service;
using System.Collections.Generic;
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
        public async Task<ActionResult> Index()
        {
            var model = await GetConfigurationData(0);
            return View(model);
        }

        public async Task<ActionResult> GetDataForConfigurationAsync(int configurationId)
        {
            var model = await GetConfigurationData(0);
            return View(model);
        }

        private async Task<DashboardModel> GetConfigurationData(int configurationId)
        {
            var model = new DashboardModel();
            var settings = await _dataService.GetSettings();
            model.ProjectName = settings.ContainsKey(AppConstants.PROJECT_NAME) ? settings[AppConstants.PROJECT_NAME] : string.Empty;
            model.Scenarios = await _dataService.GetScenarios(configurationId);
            model.Configurations = _dataService.GetConfigurations();
            model.TestRuns = await _dataService.GetTestRuns(0, false);
            return model;
        }

        [HttpPost]
        public async Task<JsonResult> GetScenarioDataForConfigurationAsync(int configurationId, string scenario)
        {
            var data = new TableModel();

            if (scenario.Equals("All"))
            {
                data.ScenarioNames = _dataService.GetScenarioNames();
                data.Runs = await _dataService.GetScenarioRuns(configurationId, true);
            }
            else
            {
                data.ScenarioNames = new List<string> { scenario };
                data.Runs = await _dataService.GetScenarioRuns(configurationId, scenario, true);
            }

            return Json(data);
        }
    }
}