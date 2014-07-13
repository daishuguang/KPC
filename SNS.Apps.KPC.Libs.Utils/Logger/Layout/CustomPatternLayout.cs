using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4net.Core;
using log4net.Layout.Pattern;
using log4net.Util;
using log4net.Layout;

namespace SNS.Apps.KPC.Libs.Utils.Logger.Layout
{
	public class CustomPatternLayout : log4net.Layout.PatternLayout
	{
		public CustomPatternLayout()
		{
			// Methods: 2
			//this.AddConverter("property", typeof(Pattern.XPatternLayoutConverter));
			/* End */

			// Methods: 3
			//this.AddConverter("Browser", typeof(Pattern.BrowserPatternConverter));
			//this.AddConverter("HostName", typeof(Pattern.HostNamePatternConverter));
			//this.AddConverter("Message", typeof(Pattern.MessagePatternConverter));
			//this.AddConverter("Url", typeof(Pattern.UrlPatternConverter));
			//this.AddConverter("UserAgent", typeof(Pattern.UserAgentPatternConverter));
			//this.AddConverter("UserID", typeof(Pattern.UserIDPatternConverter));
			//this.AddConverter("UserName", typeof(Pattern.UserNamePatternConverter));
			/* End */
		}
	}
}
