using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web.WebPages;

namespace SNS.Apps.KPC.Open.Extensions
{
	public class CommonRouteConstraint : IRouteConstraint
	{
		public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
		{
			if (values.ContainsKey(parameterName) && (httpContext.Request.Url.AbsolutePath.ToLower().StartsWith("/whatsnew") || httpContext.Request.Url.AbsolutePath.ToLower().StartsWith("/award")))
			{
				return true;
			}

			return false;
		}
	}
}