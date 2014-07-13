using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Models.WeChat;

namespace SNS.Apps.KPC.Libs.WeChatHelper
{
	public static class WeChatMessenger
	{
		public static T CreateMsgInstance<T>(string msgTemplateName)
		{
			var xmlFile = Path.Combine(HttpContext.Current.Server.MapPath("~/Messages"), string.Format("{0}.xml", msgTemplateName));

			if (!string.IsNullOrEmpty(xmlFile))
			{
				using (var reader = new StreamReader(xmlFile, Encoding.UTF8))
				{
					var ctor = typeof(T).GetConstructor(new Type[] { typeof(string) });
					var instance =  (T)ctor.Invoke(new object[] { reader.ReadToEnd() });

					reader.Close();

					return instance;
				}
			}

			return default(T);
		}
	}
}
