using System.Web;
using System.Web.Optimization;

namespace PresentationLayer
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/PresentationLayer/bundles/jquery").Include(
                        "~/PresentationLayer/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/PresentationLayer/bundles/jqueryval").Include(
                        "~/PresentationLayer/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/PresentationLayer/bundles/modernizr").Include(
                        "~/PresentationLayer/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/PresentationLayer/bundles/bootstrap").Include(
                      "~/PresentationLayer/Scripts/bootstrap.js",
                      "~/PresentationLayer/Scripts/respond.js",
                       "~/PresentationLayer/Scripts/jquery-ui-1.12.1.js",
                       "~/PresentationLayer/Scripts/jquery.unobtrusive-ajax.js",
                       "~/PresentationLayer/Scripts/jquery.signalR-2.2.1.js"
                       
                      ));

            bundles.Add(new StyleBundle("~/PresentationLayer/Content/css").Include(
                      "~/PresentationLayer/Content/bootstrap-paper.css",
                      "~/PresentationLayer/Content/site.css",
                      "~/PresentationLayer/Content/dashboard.css",
                      "~/PresentationLayer/Content/calendar.css",
                      "~/PresentationLayer/fonts/css/font-awesome.css"));
        }
    }
}
