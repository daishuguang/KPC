using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web.Security;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.IServices;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;

namespace SNS.Apps.KPC.Open.Filters
{
	public class ErrorHandlingFilter : HandleErrorAttribute
	{
		public override void OnException(ExceptionContext filterContext)
		{
			if (filterContext.ExceptionHandled)
			{
				return;
			}

			if (ConfigStore.CommonSettings.Maintain_Mode)
			{
				filterContext.Result = new ViewResult { ViewName = "Error_Maintenance" };
			}
			else
			{
				try
				{
					#region "写入错误日志"
					DBLogger.Instance.ErrorFormat(
						"ExceptionHandling - Controller: {0}, Action: {1}\r\nException: {2}",
						filterContext.RouteData.GetRequiredString("controller"),
						filterContext.RouteData.GetRequiredString("action"),
						filterContext.Exception.ToString()
					);
					#endregion

					#region "发送管理员消息"
					var clientW = default(IWeChatService);

					try
					{
						var sb = new StringBuilder();

						sb.AppendLine("快拼车管理员，m.kuaipinche.com 站点发生未处理的异常!");
						sb.AppendLine(string.Format("Controller: {0}, Action: {1}, User: {2}, IsAuth: {3}", filterContext.RouteData.GetRequiredString("controller"), filterContext.RouteData.GetRequiredString("action"), filterContext.HttpContext.User.Identity.Name, filterContext.HttpContext.User.Identity.IsAuthenticated));
						sb.AppendFormat("Exception: {0}", filterContext.Exception.Message);

						clientW = Libs.Utils.WCFServiceClientUtility.CreateServiceChanel<IWeChatService>(ConfigStore.APIServiceSettings.WeChat_Service_Entry);
						clientW.SendMessage(
							ConfigStore.CommonSettings.CustomerService_Administrator_OpenID,
							sb.ToString(),
							false
						);
					}
					finally
					{
						Libs.Utils.WCFServiceClientUtility.CloseServiceChannel(clientW);
					}
					#endregion
				}
				catch { }

				if (filterContext.Exception is UnauthorizedAccessException)
				{
					filterContext.Result = new ViewResult { ViewName = "Error_NoAuth" };
				}
				else
				{
					filterContext.Result = new ViewResult { ViewName = "Error" };
				}
			}

			filterContext.ExceptionHandled = true;
			filterContext.HttpContext.Response.Clear();
		}
	}
}