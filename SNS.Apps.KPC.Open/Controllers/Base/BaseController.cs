using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net;
using System.Web;
using System.Web.Mvc;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.IServices;
using SNS.Apps.KPC.Libs.Utils;

using SNS.Apps.KPC.Open.Filters;

namespace SNS.Apps.KPC.Open.Controllers.Base
{
	[ErrorHandlingFilter]
    public class BaseController : System.Web.Mvc.Controller
    {
        #region "Fields"
        User _currUser = null;

		const string CNSTR_EXTEND_CHANNEL = "EXTEND_CHANNEL";
		const string CNSTR_USERIDENTITY_PATTERN = @"^.+\s+\((\d+)\,\s+([0-9a-f]{8}\-(?:[0-9a-f]{4}\-){3}[0-9a-f]{12})\)*$";
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
						var client = CreateServiceClient<IRepositoryService>();

						try
						{
							_currUser = client.GetUser(this.CurrentUserIdentityName, true);

							Session["Curr_User"] = _currUser;
							Session["Curr_User_NickName"] = (_currUser != null) ? (_currUser.NickName) : (null);
						}
						finally
						{
							CloseServiceClient(client);
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

		public dynamic CurrentUserIdentityName
		{
			get
			{
				if (HttpContext.User.Identity.IsAuthenticated)
				{
					var uID = default(long);
					var uGUID = Guid.Empty;
					var match = Regex.Match(HttpContext.User.Identity.Name, CNSTR_USERIDENTITY_PATTERN);

					if (match.Success)
					{
						if (long.TryParse(match.Groups[1].Value, out uID))
						{
							if (uID != 0)
							{
								return uID;
							}
						}

						if (Guid.TryParse(match.Groups[2].Value, out uGUID))
						{
							return uGUID;
						}
					}

					if (Guid.TryParse(HttpContext.User.Identity.Name, out uGUID))
					{
						return uGUID;
					}
				}

				throw new Exception(string.Format("Fail to identity curent user's Auth Cookies Values: {0}", HttpContext.User.Identity.Name));
			}
		}
        #endregion

		#region "Properties"
		public string ExtendChannel { get { return Session[CNSTR_EXTEND_CHANNEL] == null ? "0" : Convert.ToString(Session[CNSTR_EXTEND_CHANNEL]); } set { Session[CNSTR_EXTEND_CHANNEL] = value; } } 

		public bool IsWeChatBrowser
		{
			get 
			{
				return (HttpContext.Request != null && !string.IsNullOrEmpty(HttpContext.Request.UserAgent) && HttpContext.Request.UserAgent.ToLower().Contains("micromessenger"));
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