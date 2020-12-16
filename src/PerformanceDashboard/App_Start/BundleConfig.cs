using System.Web.Optimization;

namespace PerformanceDashboard
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                        "~/Content/lib/jquery/jquery.min.js",
                        "~/Content/lib/bootstrap/js/bootstrap.min.js",
                        "~/Content/lib/knockout/knockout-latest.min.js",
                        "~/Content/lib/moment/moment-with-locales.min.js",
                        "~/Content/lib/google-charts-loader.js",
                        "~/Content/site.js"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/Content/site.css",
                      "~/Content/lib/bootstrap/css/bootstrap.min.css"));
        }
    }
}
