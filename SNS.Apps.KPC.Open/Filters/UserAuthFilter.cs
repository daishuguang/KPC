using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.IServices;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;

namespace SNS.Apps.KPC.Open.Filters
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
	public class UserAuthFilter : AuthorizeAttribute
	{
		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			if (filterContext.HttpContext.User.Identity.IsAuthenticated && filterContext.ActionDescriptor.IsDefined(typeof(AuthorizeAttribute), true) && !filterContext.ActionDescriptor.IsDefined(typeof(UserAuthNoRegisterFilter), true))
			{
				var controller = (filterContext.Controller as SNS.Apps.KPC.Open.Controllers.Base.BaseController);

				if (controller == null)
				{
					throw new Exception(string.Format("未知异常：未能获取当前 Controller 类型！Controller: {0}, Action: {1}, Url: {2}", filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, filterContext.ActionDescriptor.ActionName, HttpContext.Current.Request.Url.AbsoluteUri));
				}

				#region "已认证但未注册或者需补充手机号的用户"
				var user = controller.CurrentUser;

				if (user == null)
				{
					#region "处理因异常情况未能写入数据库的用户"
					throw new Exception(string.Format("未知异常：未能获取当前用户数据：{0}", HttpContext.Current.User.Identity.Name));
					//DBLogger.Instance.WarnFormat("未能在数据库中找到用户‘{0}’的注册记录，再次加关注该用户！", HttpContext.Current.User.Identity.Name);

					//var client = WCFServiceClientUtility.CreateServiceChanel<IRepositoryService>(ConfigStore.APIServiceSettings.Repository_Service_Entry);

					//try
					//{
					//	user = client.Subscribe(HttpContext.Current.User.Identity.Name, null);
					//}
					//finally
					//{
					//	WCFServiceClientUtility.CloseServiceChannel(client);
					//} 
					#endregion
				}

				if ((!(string.Compare(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, "User", true) == 0 && string.Compare(filterContext.ActionDescriptor.ActionName, "Register", true) == 0)) &&
					(!(string.Compare(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, "User", true) == 0 && string.Compare(filterContext.ActionDescriptor.ActionName, "EditProfile", true) == 0 && user.Status == UserStatus.Registered)))
				{
					var currUrl = HttpContext.Current.Request.Url.AbsolutePath;

					if (user.Status != UserStatus.Registered)
					{
						#region "未注册状态"
						var retuUrl = string.Format("{0}/user/register", ConfigStore.CommonSettings.App_Site_Url);
						var authUrl = string.Format("{0}{1}{2}", retuUrl, ((retuUrl.Contains("?")) ? ("&") : ("?")), Uri.EscapeUriString(string.Format("ReturnUrl={0}", currUrl)));

						filterContext.Result = new RedirectResult(authUrl);

						if (ConfigStore.CommonSettings.Trace_Mode)
						{
							DBLogger.Instance.InfoFormat("开始转向用户注册页面：{0}", authUrl);
						}

						return;
						#endregion
					}
					else if (string.IsNullOrEmpty(user.Mobile))
					{
						#region "已注册未填手机号"
						var retuUrl = string.Format("{0}/user/editprofile", ConfigStore.CommonSettings.App_Site_Url);
						var authUrl = string.Format("{0}{1}{2}", retuUrl, ((retuUrl.Contains("?")) ? ("&") : ("?")), Uri.EscapeUriString(string.Format("ReturnUrl={0}", currUrl)));

						filterContext.Result = new RedirectResult(authUrl);

						if (ConfigStore.CommonSettings.Trace_Mode)
						{
							DBLogger.Instance.InfoFormat("开始转向用户Profile编辑页面：{0}", authUrl);
						}

						return;
						#endregion
					}
				}
				#endregion
			}
		}
	}

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
	public class UserAuthNoRegisterFilter : Attribute
	{

	}
}