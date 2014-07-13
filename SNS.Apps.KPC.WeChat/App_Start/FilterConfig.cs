using System.Web;
using System.Web.Mvc;

namespace SNS.Apps.KPC.WeChat
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new Filters.ErrorHandlingFilter());
			//filters.Add(new HandleErrorAttribute());
		}
	}
}
