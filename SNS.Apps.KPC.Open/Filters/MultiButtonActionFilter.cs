using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SNS.Apps.KPC.Open.Filters
{
	public class MultiButtonActionFilter : ActionNameSelectorAttribute
	{
		#region "Properties"
		public string Name { get; set; }
		#endregion

		#region "Constructs"
		public MultiButtonActionFilter(string name)
		{
			this.Name = name;
		}
		#endregion

		#region "Methods"
		public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
		{
			if (string.IsNullOrEmpty(this.Name))
			{
				return false;
			}

			return controllerContext.HttpContext.Request.Form.AllKeys.Contains(this.Name);
		} 
		#endregion
	}
}