using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PresentationLayer
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            // Add /MyVeryOwn/ folder to the default location scheme for STANDARD Views
            var razorEngine = ViewEngines.Engines.OfType<RazorViewEngine>().FirstOrDefault();
            razorEngine.ViewLocationFormats =
                razorEngine.ViewLocationFormats.Concat(new string[] {
            "~/PresentationLayer/Views/{1}/{0}.cshtml",
            "~/PresentationLayer/Views/{0}.cshtml"
                    // add other folders here (if any)
                }).ToArray();

            // Add /MyVeryOwnPartialFolder/ folder to the default location scheme for PARTIAL Views
            razorEngine.PartialViewLocationFormats =
                razorEngine.PartialViewLocationFormats.Concat(new string[] {
            "~/MyVeryOwnPartialFolder/Views/Shared{0}.cshtml"
                    // add other folders here (if any)
                }).ToArray();
        }
    }
}
