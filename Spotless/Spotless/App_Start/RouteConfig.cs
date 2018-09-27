using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Spotless
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "services-details",
                url: "services/{id}/{title}",
                defaults: new { controller = "services", action = "details"},
                constraints: null,
                namespaces: new[] { "Spotless.Controllers" }
            );
            routes.MapRoute(
                name: "services",
                url: "services",
                defaults: new { controller = "services", action = "index"},
                constraints: null,
                namespaces: new[] { "Spotless.Controllers" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints:null,
                namespaces: new[] { "Spotless.Controllers" }
            );
        }
    }
}