using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SNS.Apps.KPC.Libs.Utils.Logger.Base
{
	public abstract class LogMessage
	{
		#region "Properties"
		public string Message { get; set; } 
		#endregion
	}

	public sealed class HttpLogMessage : LogMessage
	{
		#region "Fields"
		protected const string CNSTR_USERIDENTITY_PATTERN = @"^(.+)\s+\((\d+)\,\s+([0-9a-f]{8}\-(?:[0-9a-f]{4}\-){3}[0-9a-f]{12})\)*$";
		protected const string CNSTR_PROPERTYNAME_BROWSER = "browser";
		protected const string CNSTR_PROPERTYNAME_HOSTNAME = "hostname";
		protected const string CNSTR_PROPERTYNAME_URL = "url";
		protected const string CNSTR_PROPERTYNAME_USERAGENT = "useragent";
		protected const string CNSTR_PROPERTYNAME_USERID = "userid";
		protected const string CNSTR_PROPERTYNAME_USERNAME = "username";
		#endregion

		#region "Constructs"
		private HttpLogMessage()
		{
			if (HttpContext.Current != null && HttpContext.Current.Request != null)
			{
				this.Browser = HttpContext.Current.Request.Browser.Browser;
				this.Url = HttpContext.Current.Request.RawUrl;
				this.UserAgent = HttpContext.Current.Request.UserAgent;
				this.HostName = HttpContext.Current.Request.UserHostName;

				if (HttpContext.Current.User != null && HttpContext.Current.User.Identity != null && HttpContext.Current.User.Identity.IsAuthenticated)
				{
					var match = Regex.Match(HttpContext.Current.User.Identity.Name, CNSTR_USERIDENTITY_PATTERN);

					if (match.Success)
					{
						this.UserID = match.Groups[2].Value;
						this.UserName = match.Groups[1].Value;
					}
				}
			}
		}

		public HttpLogMessage(string message)
			: this()
		{
			this.Message = message;
		} 
		#endregion

		#region "Properties"
		public string UserID { get; set; }

		public string UserName { get; set; }

		public string UserAgent { get; set; }

		public string Url { get; set; }

		public string HostName { get; set; }

		public string Browser { get; set; } 
		#endregion
	}

	public sealed class WXLogMessage : LogMessage
	{
		#region "Properties"
		public string UserID { get; set; }
		#endregion
	}
}
