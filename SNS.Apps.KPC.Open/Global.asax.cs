using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Optimization;
using System.Web.Routing;

namespace SNS.Apps.KPC.Open
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
#if DEBUG
			//BundleTable.EnableOptimizations = true;
#else
			BundleTable.EnableOptimizations = true;
#endif
			AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

		protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
		{
			var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];

			if (authCookie != null)
			{
				var userDataTicket = FormsAuthentication.Decrypt(authCookie.Value);
				var userData = Newtonsoft.Json.JsonConvert.DeserializeObject<CustomPrincipalModel>(userDataTicket.UserData);

				if (userData == null)
				{
					return;
				}

				HttpContext.Current.User = new CustomPrincipal(userData);
			}
		}
    }
}
