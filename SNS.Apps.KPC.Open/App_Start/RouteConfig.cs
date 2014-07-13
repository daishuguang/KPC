using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using SNS.Apps.KPC.Open.Extensions;

namespace SNS.Apps.KPC.Open
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.LowercaseUrls = true;

			routes.MapRoute(
				name: "Route 1",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Route", action = "Detail" },
				constraints: new { controller = @"Route|User", action = @"Detail|Edit|View|ViewProfile", id = @"^[0-9a-f]{8}\-(?:[0-9a-f]{4}\-){3}[0-9a-f]{12}$" },
				namespaces: new string[] { "SNS.Apps.KPC.Open" }
			);

            routes.MapRoute(
				name: "Route 2",
                url: "{controller}/{action}/{folio}",
                defaults: new { controller = "Order", action = "Detail" },
                constraints: new { controller = @"Order", action = @"Detail|Edit|Cancel|Confirm", folio = @"^\d{14}$" },
                namespaces: new string[] { "SNS.Apps.KPC.Open" }
            );

			routes.MapRoute(
				name: "Route 3",
				url: "{controller}/{action}/{folio}/{payMethod}",
				defaults: new { controller = "Payment", action = "Pay" },
				constraints: new { controller = "Payment", action = "Pay", folio = @"^\d{14}$", payMethod = @"^\d+$" },
				namespaces: new string[] { "SNS.Apps.KPC.Open" }
			);

			routes.MapRoute(
				name: "Route 4",
				url: "{controller}/{action}/{folio}",
				defaults: new { controller = "Insurance", action = "Detail", folio = UrlParameter.Optional },
				constraints: new { controller = "Insurance", action = @"Create|Detail", folio = new InsuranceRouteConstraint() },
				namespaces: new string[] { "SNS.Apps.KPC.Open" }
			);

			routes.MapRoute(
				name: "Route 5",
				url: "{action}",
				defaults: new { controller = "Common", action = "WhatsNew" },
				constraints: new { controller = "Common", action = new CommonRouteConstraint() },
				namespaces: new string[] { "SNS.Apps.KPC.Open" }
			);

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Route", action = "Search", id = UrlParameter.Optional },
				namespaces: new string[] { "SNS.Apps.KPC.Open" }
			);
		}
	}
}
