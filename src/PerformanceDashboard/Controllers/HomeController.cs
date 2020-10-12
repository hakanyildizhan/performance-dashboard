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
            var model = new DashboardModel();
            var configuration = await _dataService.GetConfiguration();
            model.ProjectName = configuration.ContainsKey("ProjectName") ? configuration["ProjectName"] : string.Empty;
            model.Scenarios = await _dataService.GetScenarios();
            model.TestRuns = await _dataService.GetTestRuns(false);
            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> GetDataAsync(string selection)
        {
            var data = new TableModel();

            if (selection.Equals("All"))
            {
                data.ScenarioNames = _dataService.GetScenarioNames();
                data.Runs = await _dataService.GetScenarioRuns(true);
            }
            else
            {
                data.ScenarioNames = new List<string> { selection };
                data.Runs = await _dataService.GetScenarioRuns(selection, true);
            }

            return Json(data);
        }
    }
}