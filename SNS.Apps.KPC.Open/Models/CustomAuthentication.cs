using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Text;
using System.Text.RegularExpressions;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.Models;

namespace SNS.Apps.KPC.Open
{
	public class CustomAuthentication
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="userInstance"></param>
		public static void SetAuthCookie(User userInstance)
		{
			var userData = new CustomPrincipalModel { ID = userInstance.ID, UserID = userInstance.UserGUID, NickName = userInstance.NickName };
			var userDataTicket = new FormsAuthenticationTicket(1, userData.UserID.ToString(), DateTime.Now, DateTime.Now.AddTicks(FormsAuthentication.Timeout.Ticks), false, Newtonsoft.Json.JsonConvert.SerializeObject(userData));
			var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(userDataTicket));

			if (HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName] != null)
			{
				HttpContext.Current.Request.Cookies.Remove(FormsAuthentication.FormsCookieName);
			}

			HttpContext.Current.Response.Cookies.Add(authCookie);
		}
	}
}