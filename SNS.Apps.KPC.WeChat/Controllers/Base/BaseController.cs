using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.IServices;
using SNS.Apps.KPC.Libs.Utils;

using SNS.Apps.KPC.WeChat.Filters;

namespace SNS.Apps.KPC.WeChat.Controllers.Base
{
	[ErrorHandlingFilter]
	public class BaseController : System.Web.Mvc.Controller
	{
		#region "Fields"
		User _currUser = null;
		#endregion

		#region "Properties"
		public User CurrentUser
		{
			get
			{
				if (HttpContext.User.Identity.IsAuthenticated)
				{
					if (_currUser == null)
					{
						if (Session["Curr_User"] == null)
						{
							var client = CreateServiceClient<IRepositoryService>();

							try
							{
								_currUser = client.GetUser(HttpContext.User.Identity.Name);

								Session["Curr_User"] = _currUser;
								Session["Curr_User_NickName"] = (_currUser != null) ? (_currUser.NickName) : (null);
							}
							finally
							{
								CloseServiceClient(client);
							}
						}
						else
						{
							_currUser = Session["Curr_User"] as User;
						}
					}
				}
				else
				{
					_currUser = null;

					Session["Curr_User"] = null;
					Session["Curr_User_NickName"] = null;
				}

				return _currUser;
			}
			set
			{
				_currUser = value;

				if (_currUser != null)
				{
					Session["Curr_User"] = _currUser;
					Session["Curr_User_NickName"] = _currUser.NickName;
				}
				else
				{
					Session["Curr_User"] = null;
					Session["Curr_User_NickName"] = null;
				}
			}
		}

		public string CurrentSiteUrl
		{
			get
			{
				return ConfigStore.CommonSettings.App_Site_Url;
			}
		}
		#endregion

		#region "Methods: Service Client"
		protected IContract CreateServiceClient<IContract>()
		{
			var t = typeof(IContract);

			if (t.Equals(typeof(IRepositoryService)))
			{
				return WCFServiceClientUtility.CreateServiceChanel<IContract>(ConfigStore.APIServiceSettings.Repository_Service_Entry);
			}
			else if (t.Equals(typeof(IRouteMatrixService)))
			{
				return WCFServiceClientUtility.CreateServiceChanel<IContract>(ConfigStore.APIServiceSettings.RouteMatrix_Service_Entry);
			}
			else if (t.Equals(typeof(IInsuranceService)))
			{
				return WCFServiceClientUtility.CreateServiceChanel<IContract>(ConfigStore.APIServiceSettings.Insurance_Service_Entry);
			}
			else if (t.Equals(typeof(IWeChatService)))
			{
				return WCFServiceClientUtility.CreateServiceChanel<IContract>(ConfigStore.APIServiceSettings.WeChat_Service_Entry);
			}
			else if (t.Equals(typeof(ISMSService)))
			{
				return WCFServiceClientUtility.CreateServiceChanel<IContract>(ConfigStore.APIServiceSettings.SMS_Service_Entry);
			}
			else if (t.Equals(typeof(ILogService)))
			{
				return WCFServiceClientUtility.CreateServiceChanel<IContract>(ConfigStore.APIServiceSettings.Log_Service_Entry);
			}

			return default(IContract);
		}

		protected void CloseServiceClient<IContract>(IContract client)
		{
			WCFServiceClientUtility.CloseServiceChannel(client);
		} 
		#endregion
	}
}