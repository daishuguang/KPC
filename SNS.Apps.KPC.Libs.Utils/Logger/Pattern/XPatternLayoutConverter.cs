using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

using log4net.Core;
using log4net.Layout.Pattern;

namespace SNS.Apps.KPC.Libs.Utils.Logger.Pattern
{
	public class XPatternLayoutConverter : PatternLayoutConverter
	{
		protected override void Convert(System.IO.TextWriter writer, log4net.Core.LoggingEvent loggingEvent)
		{
			if (this.Option != null)
			{
				// Write the value for the specified key
				WriteObject(writer, loggingEvent.Repository, LookupProperty(Option, loggingEvent));
			}
			else
			{
				// Write all the key value pairs
				WriteDictionary(writer, loggingEvent.Repository, loggingEvent.GetProperties());
			}
		}

		/// <summary>
		/// 通过反射获取传入的日志对象的某个属性的值
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		private Object LookupProperty(String property, log4net.Core.LoggingEvent loggingEvent)
		{
			Object propertyValue = String.Empty;
			PropertyInfo propertyInfo;

			propertyInfo = loggingEvent.MessageObject.GetType().GetProperty(property);

			if (propertyInfo != null)
			{
				propertyValue = propertyInfo.GetValue(loggingEvent.MessageObject, null);
			}

			return propertyValue;
		}
	}
}
