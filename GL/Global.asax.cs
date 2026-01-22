using GL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace GL
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Application["LiveSessionsCount"] = 0;
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // Add session to the tracking list
            SessionManager.AddSession(Session.SessionID);
        }

        protected void Session_End(object sender, EventArgs e)
        {
            // Remove session from the tracking list
            SessionManager.RemoveSession(Session.SessionID);
        }

    }
}
