using System.Web;
using System.Web.Mvc;

using SNS.Apps.KPC.Libs.Configurations;

namespace SNS.Apps.KPC.Open
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new Filters.UserAuthFilter());
			filters.Add(new Filters.ErrorHandlingFilter());
			filters.Add(new Filters.TraceModelFilter());
			//filters.Add(new HandleErrorAttribute());
		}
	}
}
