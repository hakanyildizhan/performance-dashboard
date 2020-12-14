using System.Web.Optimization;

namespace PerformanceDashboard
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Content/lib/jquery/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Content/lib/bootstrap/js/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/site").Include(
                      "~/Content/site.js"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/Content/site.css",
                      "~/Content/lib/bootstrap/css/bootstrap.min.css"));
        }
    }
}
