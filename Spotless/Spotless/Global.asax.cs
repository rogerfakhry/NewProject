using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebMatrix.WebData;

namespace Spotless
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            if (!WebSecurity.Initialized)
                WebSecurity.InitializeDatabaseConnection("DefaultConnectionString", "UserProfile", "UserId", "UserName", autoCreateTables: true);
        }
        //public void RegisterRoutes(RouteCollection routes)
        //{
        //    routes.MapRoute(
        //        "EMS_default",
        //        "/",
        //        new { Area="EMS",action = "index", controller = "home", id = UrlParameter.Optional },
        //        constraints: null,
        //        namespaces: new[] { "Spotless.Controllers" }

        //    );
        //}
    }
}