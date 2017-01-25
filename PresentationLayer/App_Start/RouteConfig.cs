using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PresentationLayer
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //URL with format /action/ will autamatically use the controller Console witghout showing it in URL
            //routes.MapRoute(
            //    "ConsoleRoute",
            //    "{action}",
            //    new { controller = "Console", action = "Calendar", id = UrlParameter.Optional }
            //    );


            //url with format controller/action will call the controller and action specified in the url
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Calendar", id = UrlParameter.Optional }
            );
        }
    }
}
