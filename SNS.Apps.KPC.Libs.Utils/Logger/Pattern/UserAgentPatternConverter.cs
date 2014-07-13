﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

using log4net.Core;
using log4net.Layout.Pattern;

namespace SNS.Apps.KPC.Libs.Utils.Logger.Pattern
{
	internal sealed class UserAgentPatternConverter : Base.CustomPatternConverter
	{
		#region "Constructs"
		public UserAgentPatternConverter() { this.PropertyName = CNSTR_PROPERTYNAME_USERAGENT; } 
		#endregion
	}
}
