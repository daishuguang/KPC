using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web.WebPages;

namespace SNS.Apps.KPC.Open.Extensions
{
	public static class RazorExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="items"></param>
		/// <param name="itemTemplate"></param>
		/// <param name="alternatingItemTemplate"></param>
		/// <param name="separatorTemplate"></param>
		/// <returns></returns>
		public static HelperResult Repeater<T>(
			   this IEnumerable<T> items,
			   Func<T, HelperResult> itemTemplate,
			   Func<T, HelperResult> alternatingItemTemplate = null,
			   Func<T, HelperResult> separatorTemplate = null)
		{
			return new HelperResult(writer =>
			{
				if (items.Count() > 0)
				{
					var last = items.Last();
					var i = 0;

					foreach (var item in items)
					{
						var func = (i % 2 != 0 && alternatingItemTemplate != null) ? alternatingItemTemplate : itemTemplate;

						func(item).WriteTo(writer);

						if (separatorTemplate != null && !item.Equals(last))
						{
							separatorTemplate(item).WriteTo(writer);
						}

						i++;
					}
				}
			});
		}
	}
}