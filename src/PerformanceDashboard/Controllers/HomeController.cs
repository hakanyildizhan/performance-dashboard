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
            var model = new DashboardModel();
            var settings = await _dataService.GetSettings();
            model.ProjectName = settings.ContainsKey(AppConstants.PROJECT_NAME) ? settings[AppConstants.PROJECT_NAME] : string.Empty;
            model.Scenarios = await _dataService.GetScenarios(configurationId);
            model.Configurations = _dataService.GetConfigurations();
            model.TestRuns = await _dataService.GetTestRuns(configurationId, false);

            return Json(new 
            { 
                ProjectName = model.ProjectName, 
                Scenarios = model.Scenarios, 
                Configurations = model.Configurations, 
                TestRuns = model.TestRuns.ToArray() 
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
    }
}