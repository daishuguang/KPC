using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using SNS.Apps.KPC.Libs.Configurations;

namespace SNS.Apps.KPC.Libs.Utils
{
	public static class WebClientUtility
	{
		#region "Methods: CreateWebRequest"
		public static HttpWebRequest CreateWebRequest(HttpRequestMethod method, string url)
		{
			return CreateWebRequest(method, url, null, null, null);
		}

		public static HttpWebRequest CreateWebRequest(HttpRequestMethod method, string url, string refUrl)
		{
			return CreateWebRequest(method, url, refUrl, null, null);
		}

		public static HttpWebRequest CreateWebRequest(HttpRequestMethod method, string url, string refUrl, string data)
		{
			return CreateWebRequest(method, url, refUrl, data, null);
		}

		public static HttpWebRequest CreateWebRequest(HttpRequestMethod method, string url, string refUrl, string data, CookieContainer cookie = null)
		{
			var webRequest = (HttpWebRequest)WebRequest.Create(url);

			webRequest.Referer = (!string.IsNullOrEmpty(refUrl)) ? (refUrl) : (string.Empty);
			webRequest.Method = method.ToString();
			webRequest.UserAgent = ConfigStore.CommonSettings.WebRequest_UserAgent;

			if (cookie != null)
			{
				webRequest.CookieContainer = cookie;
			}

			switch (method)
			{
				case HttpRequestMethod.GET:
					webRequest.ContentType = "text/html; charset=UTF-8";
					break;
				case HttpRequestMethod.POST:
					webRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";

					if (!string.IsNullOrEmpty(data))
					{
						var byteData = Encoding.UTF8.GetBytes(data);

						webRequest.ContentLength = byteData.Length;

						using (Stream stream = webRequest.GetRequestStream())
						{
							// Send the data.            
							stream.Write(byteData, 0, byteData.Length);
							stream.Close();
						}
					}

					break;
			}

			return webRequest;
		} 
		#endregion
	}

	public enum HttpRequestMethod
	{
		GET,
		POST
	}
}
