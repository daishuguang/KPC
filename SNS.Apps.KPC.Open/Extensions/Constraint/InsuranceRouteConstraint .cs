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
	public class InsuranceRouteConstraint : IRouteConstraint
	{
		public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
		{
			if (values.ContainsKey(parameterName) && httpContext.Request.Url.AbsolutePath.ToLower().StartsWith("/insurance/"))
			{
				var action = httpContext.Request.Url.AbsolutePath.ToLower().Substring("/insurance/".Length);
				var folio = string.Empty;
				var idx = action.IndexOf('/');
				
				if (idx != -1)
				{
					folio = action.Substring(idx + 1);
					action = action.Substring(0, idx);
				}

				var flag = (action == "create" && string.IsNullOrEmpty(folio)) || (action == "detail" && Regex.IsMatch(folio, @"^(?:\s*|\d{14})$"));

				return flag;
			}

			return false;
		}
	}
}