using System.Web.Optimization;

namespace PerformanceDashboard
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Content/jquery-3.5.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/site").Include(
                      "~/Content/site.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/site.css"));
        }
    }
}
