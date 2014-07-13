using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web.Security;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.IServices;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;

namespace SNS.Apps.KPC.Open.Filters
{
	public class TraceModelFilter : ActionFilterAttribute
	{
		#region "Fields"
		private const string Version = "wap-1-0.1";
		private const int VisitDuration = 1800;
		private const int VisitorMaxAge = 31536000;

		private string[,] searchEngineList = {
			{"1", "baidu.com", "word|wd"}, 
			{"2", "google.com", "q"}, 
			{"4", "sogou.com", "query"}, 
			{"6", "search.yahoo.com", "p"}, 
			{"7", "yahoo.cn", "q"}, 
			{"8", "soso.com", "w"}, 
			{"11", "youdao.com", "q"}, 
			{"12", "gougou.com", "search"}, 
			{"13", "bing.com", "q"},
			{"14", "so.com", "q"}, 
			{"14", "so.360.cn", "q"}, 
			{"15", "jike.com", "q"}, 
			{"16", "qihoo.com", "kw"}, 
			{"17", "etao.com", "q"}, 
			{"18", "soku.com", "keyword"}
		};

		private string searchEngine = "";
		private string searchWord = "";
		private string siteId = ConfigStore.CommonSettings.Baidu_TrackPageView_SiteID;
		#endregion

		#region "Events"
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (ConfigStore.CommonSettings.Trace_Mode && filterContext.HttpContext.Request != null)
			{
				DBLogger.Instance.InfoFormat(
					"TRACE - OnActionExecuting: Controller: {0}, Action: {1}",
					filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
					filterContext.ActionDescriptor.ActionName
				);
			}

			base.OnActionExecuting(filterContext);
		}

		//public override void OnResultExecuting(ResultExecutingContext filterContext)
		//{
		//	filterContext.Controller.ViewBag.TrackPageView = TrackPageView();

		//	base.OnResultExecuting(filterContext);
		//}
		#endregion

		#region "Private Methods"
		private static String GetRandomNumber()
		{
			Random RandomClass = new Random();
			return RandomClass.Next(0x7fffffff).ToString();
		}

		private static int GetSecondsSinceEpoch(DateTime time)
		{
			return (int)(time - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
		}

		private static string GetQueryValue(string url, string key)
		{
			Match match = Regex.Match(url, "(^|&|\\?|#)(" + key + ")=([^&#]*)(&|$|#)");

			if (match.Success)
			{
				return match.Groups[3].ToString();
			}
			else
			{
				return "";
			}
		}

		private int GetSourceType(string path, string referer, int currentPageVisitTime, int lastPageVisitTime)
		{
			if (referer == "" || (new System.Uri(path)).Host == (new System.Uri(referer)).Host)
			{
				if (currentPageVisitTime - lastPageVisitTime > VisitDuration)
				{
					return 1;
				}
				else
				{
					return 4;
				}
			}
			else
			{
				for (int i = 0; i < searchEngineList.Length / 3; i++)
				{
					if (Regex.IsMatch(referer, searchEngineList[i, 1]))
					{
						searchWord = GetQueryValue(referer, searchEngineList[i, 2]);

						if (searchWord != "" || searchEngineList[i, 0] == "2" || searchEngineList[i, 0] == "14" || searchEngineList[i, 0] == "17")
						{
							searchEngine = searchEngineList[i, 0];
							return 2;
						}
					}
				}
				return 3;
			}
		}

		private string TrackPageView()
		{
			string path = "";
			if (HttpContext.Current.Request.Url != null)
			{
				path = HttpContext.Current.Request.Url.AbsoluteUri;
			}

			string referer = "";
			if (HttpContext.Current.Request.UrlReferrer != null && "" != HttpContext.Current.Request.UrlReferrer.ToString())
			{
				referer = HttpContext.Current.Request.UrlReferrer.ToString();
			}

			int currentPageVisitTime = GetSecondsSinceEpoch(DateTime.Now);
			int lastPageVisitTime = 0;
			HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("Hm_lpvt_" + siteId);

			if (cookie != null && cookie.Value != null)
			{
				lastPageVisitTime = int.Parse(cookie.Value);
			}

			string lt = "";
			cookie = HttpContext.Current.Request.Cookies.Get("Hm_lvt_" + siteId);

			if (cookie != null && cookie.Value != null)
			{
				lt = cookie.Value;
			}

			int sourceType = GetSourceType(path, referer, currentPageVisitTime, lastPageVisitTime);
			int isNewVisit = (sourceType == 4) ? 0 : 1;

			//
			HttpCookie lpvtCookie = new HttpCookie("Hm_lpvt_" + siteId);
			lpvtCookie.Value = currentPageVisitTime.ToString();
			lpvtCookie.Path = "/";
			HttpContext.Current.Response.Cookies.Add(lpvtCookie);
			/* End */

			//
			HttpCookie lvtCookie = new HttpCookie("Hm_lvt_" + siteId);
			lvtCookie.Value = currentPageVisitTime.ToString();
			lvtCookie.Path = "/";
			lvtCookie.Expires = DateTime.Now + TimeSpan.FromSeconds(VisitorMaxAge);
			HttpContext.Current.Response.Cookies.Add(lvtCookie);
			/* End */

			string utmGifLocation = "http://hm.baidu.com/hm.gif";
			string utmUrl = utmGifLocation + "?" +
				"si=" + siteId +
				"&et=0" +
				"&nv=" + isNewVisit +
				"&st=" + sourceType +
				(searchEngine != "" ? "&se=" + searchEngine : "") +
				(searchWord != "" ? "&sw=" + HttpUtility.UrlEncode(searchWord) : "") +
				(lt != "" ? "&lt=" + lt : "") +
				(referer != "" ? "&su=" + HttpUtility.UrlEncode(referer) : "") +
				"&v=" + Version +
				"&rnd=" + GetRandomNumber();

			return HttpUtility.HtmlEncode(utmUrl);
		} 
		#endregion
	}
}