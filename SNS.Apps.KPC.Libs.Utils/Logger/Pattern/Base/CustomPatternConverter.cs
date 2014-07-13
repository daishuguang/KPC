using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

using log4net.Core;
using log4net.Layout.Pattern;

namespace SNS.Apps.KPC.Libs.Utils.Logger.Pattern.Base
{
	abstract class CustomPatternConverter : PatternLayoutConverter
	{
		#region "Fields"
		protected const string CNSTR_PROPERTYNAME_BROWSER = "Browser";
		protected const string CNSTR_PROPERTYNAME_HOSTNAME = "HostName";
		protected const string CNSTR_PROPERTYNAME_MESSAGE = "Message";
		protected const string CNSTR_PROPERTYNAME_URL = "Url";
		protected const string CNSTR_PROPERTYNAME_USERAGENT = "UserAgent";
		protected const string CNSTR_PROPERTYNAME_USERID = "UserID";
		protected const string CNSTR_PROPERTYNAME_USERNAME = "UserName";
		#endregion

		#region "Events"
		protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
		{
			var msg = loggingEvent.MessageObject as Logger.Base.HttpLogMessage;

			if (msg != null)
			{
				switch (this.PropertyName)
				{
					case CNSTR_PROPERTYNAME_BROWSER:
						writer.Write(msg.Browser);
						break;
					case CNSTR_PROPERTYNAME_HOSTNAME:
						writer.Write(msg.HostName);
						break;
					case CNSTR_PROPERTYNAME_MESSAGE:
						writer.Write(msg.Message);
						break;
					case CNSTR_PROPERTYNAME_URL:
						writer.Write(msg.Url);
						break;
					case CNSTR_PROPERTYNAME_USERAGENT:
						writer.Write(msg.UserAgent);
						break;
					case CNSTR_PROPERTYNAME_USERID:
						writer.Write(msg.UserID);
						break;
					case CNSTR_PROPERTYNAME_USERNAME:
						if (HttpContext.Current.User.Identity.IsAuthenticated)
							writer.Write(msg.UserName);
						break;
				}
			}
		}
		#endregion

		#region "Properties"
		public virtual string PropertyName { get; set; }
		#endregion
	}
}
