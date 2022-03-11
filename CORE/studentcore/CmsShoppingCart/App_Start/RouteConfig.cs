using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CmsShoppingCart
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute("Account", "Account/{action}/{id}", new { controller = "Account", action = "Index", id = UrlParameter.Optional }, new[] { "CmsShoppingCart.Controllers" });
            routes.MapRoute("Default", "", new { controller = "Home", action = "Home" }, new[] { "CmsShoppingCart.Controllers" });
            routes.MapRoute("Home", "Home/{action}/{name}", new { controller = "Home", action = "Index", name = UrlParameter.Optional }, new[] { "CmsShoppingCart.Controllers" });
            routes.MapRoute("Admin", "Admin/{action}/{name}", new { controller = "Admin", action = "Index", name = UrlParameter.Optional }, new[] { "CmsShoppingCart.Controllers" });
            routes.MapRoute("Teacher", "Teacher/{action}/{name}", new { controller = "Teacher", action = "Index", name = UrlParameter.Optional }, new[] { "CmsShoppingCart.Controllers" });
            routes.MapRoute("Child", "Child/{action}/{name}", new { controller = "Child", action = "Index", name = UrlParameter.Optional }, new[] { "CmsShoppingCart.Controllers" });
            routes.MapRoute("Grades", "Grades/{action}/{name}", new { controller = "Grades", action = "Index", name = UrlParameter.Optional }, new[] { "CmsShoppingCart.Controllers" });
            routes.MapRoute("Dashboard", "Dashboard/{action}/{name}", new { controller = "Dashboard", action = "Index", name = UrlParameter.Optional }, new[] { "CmsShoppingCart.Controllers" });
            // routes.MapRoute(
            //  name: "Default",
            //  url: "{controller}/{action}/{id}",
            // defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //  );


        }
    }
}
