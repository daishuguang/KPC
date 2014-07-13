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
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Models.WeChat;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;

namespace SNS.Apps.KPC.Libs.WeChatHelper
{
	public static class WeChatAPIExecutor
	{
		#region "Properties"
		public static WeChat_Base_AccessTokenInfo AccessToken 
		{ 
			get 
			{ 
				var accessToken = WeChat_Base_AccessToken_Manager.Instance.AccessToken;

				if (accessToken == null || accessToken.ExpireDate == null || !accessToken.ExpireDate.HasValue || DateTime.Now.CompareTo(accessToken.ExpireDate.Value) >= 0)
				{
					accessToken = WeChat_Base_AccessToken_Manager.Instance.Refresh();
				}

				return accessToken;
			} 
		}
		#endregion

		#region "Private Methods"
		//static WeChat_LoginInfo ExecLogin()
		//{
		//	var url = ConfigStore.WeChatAPISettings.WeChat_MP_LoginURI;	//请求登录的URL
		//	var data = "username=" + HttpUtility.UrlEncode(ConfigStore.WeChatAPISettings.WeChat_MP_Account) + "&pwd=" + CommonUtility.GetMd5Str32(ConfigStore.WeChatAPISettings.WeChat_MP_Password) + "&imgcode=&f=json";

		//	try
		//	{
		//		#region "Send Request"
		//		var cc = new CookieContainer();//接收缓存
		//		var byteArray = Encoding.UTF8.GetBytes(data); // 转化

		//		var webRequest = (HttpWebRequest)WebRequest.Create(url);
		//		webRequest.CookieContainer = cc;
		//		webRequest.Method = "POST";
		//		webRequest.ContentType = "application/json; charset=UTF-8";
		//		webRequest.ContentLength = byteArray.Length;
		//		webRequest.UserAgent = ConfigStore.CommonSettings.WebRequest_UserAgent;
		//		webRequest.Referer = "https://mp.weixin.qq.com/";

		//		using (var stream = webRequest.GetRequestStream())
		//		{
		//			// Set the data.
		//			stream.Write(byteArray, 0, byteArray.Length);    //写入参数
		//			stream.Close();
		//		}
		//		#endregion

		//		#region "Process Response"
		//		using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse())
		//		{
		//			var strResponse = (new StreamReader(response.GetResponseStream(), Encoding.Default)).ReadToEnd();
		//			var retInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<WeChat_Login_RetMsgInfo>(strResponse);

		//			switch (retInfo.ErrCode)
		//			{
		//				case 0:
		//				case 65201:
		//				case 65202:
		//					if (!string.IsNullOrEmpty(retInfo.ErrMsg))
		//					{
		//						var instance = new WeChat_LoginInfo();

		//						instance.LoginCookie = cc;
		//						instance.Token = retInfo.ErrMsg.Split(new char[] { '&' })[2].Split(new char[] { '=' })[1].ToString(); //取得令牌
		//						instance.CreateDate = DateTime.Now;

		//						return instance;
		//					}
		//					break;
		//				case -1:
		//					throw new Exception("系统错误，请稍候再试。");
		//				case -2:
		//					throw new Exception("帐号或密码错误。");
		//				case -3:
		//					throw new Exception("您输入的帐号或者密码不正确，请重新输入。");
		//				case -4:
		//					throw new Exception("不存在该帐户。");
		//				case -5:
		//					throw new Exception("您目前处于访问受限状态。");
		//				case -6:
		//					throw new Exception("请输入图中的验证码。");
		//				case -7:
		//					throw new Exception("此帐号已绑定私人微信号，不可用于公众平台登录。");
		//				case -8:
		//					throw new Exception("邮箱已存在。");
		//				case -32:
		//					throw new Exception("您输入的验证码不正确，请重新输入。");
		//				case -200:
		//					throw new Exception("因频繁提交虚假资料，该帐号被拒绝登录。");
		//				case -94:
		//					throw new Exception("请使用邮箱登陆。");
		//				case 10:
		//					throw new Exception("该公众会议号已经过期，无法再登录使用。");
		//				default:
		//					throw new Exception("未知的返回");
		//			}
		//		}
		//		#endregion
		//	}
		//	catch (Exception ex)
		//	{
		//		throw ex;
		//	}

		//	return null;
		//}

		//static WeChat_Base_AccessTokenInfo Refresh_BaseAccessToken()
		//{
		//	var url = string.Format(ConfigStore.WeChatAPISettings.WeChat_MP_GetAccessTokenURI, ConfigStore.WeChatAPISettings.WeChat_MP_AppID, ConfigStore.WeChatAPISettings.WeChat_MP_AppSecret);
		//	var webRequest = CreateWebRequest(HttpRequestMethod.GET, url, null, null);

		//	using (var response = (HttpWebResponse)webRequest.GetResponse())
		//	{
		//		var str = new StreamReader(response.GetResponseStream()).ReadToEnd();
		//		var ret = Newtonsoft.Json.JsonConvert.DeserializeObject<WeChat_Base_AccessToken_RetMsgInfo>(str);

		//		if (ret != null && !string.IsNullOrEmpty(ret.AccessToken))
		//		{
		//			var instance = new WeChat_Base_AccessTokenInfo();

		//			instance.AccessToken = ret.AccessToken;
		//			instance.CreateDate = DateTime.Now;
		//			instance.ExpiresDate = instance.CreateDate.Value.AddMinutes(LoginMins);

		//			return instance;
		//		}
		//		else
		//		{
		//			throw new Exception("试图获取 AccessToken 失败！");
		//		}
		//	}
		//}

		//static WeChat_OAuth2_AccessTokenInfo Refresh_OAuth2AccessToen()
		//{
		//	if (!string.IsNullOrEmpty(_oauth2_RefreshToken))
		//	{
		//		var url = string.Format(ConfigStore.WeChatAPISettings.WeChat_MP_OAuth2_RefAccessTokenURI, ConfigStore.WeChatAPISettings.WeChat_MP_AppID, _oauth2_RefreshToken);
		//		var webRequest = CreateWebRequest(HttpRequestMethod.GET, url, null, null, false);

		//		using (var response = (HttpWebResponse)webRequest.GetResponse())
		//		{
		//			var str = new StreamReader(response.GetResponseStream()).ReadToEnd();
		//			var ret = Newtonsoft.Json.JsonConvert.DeserializeObject<WeChat_OAuth2_AccessToken_RetMsgInfo>(str);

		//			if (ret != null && ret.ErrCode == 0)
		//			{
		//				_oauth2_RefreshToken = ret.RefreshToken;

		//				return new WeChat_OAuth2_AccessTokenInfo(ret);
		//			}
		//		}
		//	}

		//	// RefreshToken 已失效
		//	_oauth2_RefreshToken = null;

		//	return null;
		//}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="method"></param>
		/// <param name="url"></param>
		/// <param name="refUrl"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		//public static HttpWebRequest CreateWebRequest(HttpRequestMethod method, string url, string refUrl, string data)
		//{
		//	return CreateWebRequest(method, url, refUrl, data, false);
		//}

		///// <summary>
		///// 
		///// </summary>
		///// <param name="method"></param>
		///// <param name="url"></param>
		///// <param name="refUrl"></param>
		///// <param name="data"></param>
		///// <param name="needLogin"></param>
		///// <returns></returns>
		//public static HttpWebRequest CreateWebRequest(HttpRequestMethod method, string url, string refUrl, string data, bool needLogin)
		//{
		//	var webRequest = (HttpWebRequest)WebRequest.Create(url);

		//	webRequest.Referer = (!string.IsNullOrEmpty(refUrl)) ? (refUrl) : (string.Empty);
		//	webRequest.Method = method.ToString();
		//	webRequest.UserAgent = ConfigStore.CommonSettings.WebRequest_UserAgent;

		//	if (needLogin)
		//	{
		//		webRequest.CookieContainer = LoginInfo.LoginCookie;
		//	}

		//	switch (method)
		//	{
		//		case HttpRequestMethod.GET:
		//			webRequest.ContentType = "text/html; charset=UTF-8";
		//			break;
		//		case HttpRequestMethod.POST:
		//			webRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";

		//			if (!string.IsNullOrEmpty(data))
		//			{
		//				var byteData = Encoding.UTF8.GetBytes(data);

		//				webRequest.ContentLength = byteData.Length;

		//				using (Stream stream = webRequest.GetRequestStream())
		//				{
		//					// Send the data.            
		//					stream.Write(byteData, 0, byteData.Length);
		//					stream.Close();
		//				}
		//			}

		//			break;
		//	}

		//	return webRequest;
		//}
		#endregion

		#region "Public Methods: (QRCode)"
		public static string CreateQRCode(QRCodeActionType actionType, int sceneID)
		{
			var ticket = CreateQRCodeTicket(actionType, sceneID);

			if (!string.IsNullOrEmpty(ticket))
			{
				return string.Format(ConfigStore.WeChatAPISettings.WeChat_MP_GenerateQRCodeURI, HttpUtility.UrlEncode(ticket));
			}

			return null;
		}

		static string CreateQRCodeTicket(QRCodeActionType actionType, int sceneID)
		{
			var url = string.Format(ConfigStore.WeChatAPISettings.WeChat_MP_GenerateQRCodeTicketURI, AccessToken.Token);
			var qrRequest = new WeChat_QRCodeTicket_ReqInfo(actionType, sceneID);
			var webRequest = WebClientUtility.CreateWebRequest(HttpRequestMethod.POST, url, null, Newtonsoft.Json.JsonConvert.SerializeObject(qrRequest));

			using (var response = (HttpWebResponse)webRequest.GetResponse())
			{
				var str = new StreamReader(response.GetResponseStream()).ReadToEnd();
				var ret = Newtonsoft.Json.JsonConvert.DeserializeObject<WeChat_QRCodeTicket_RetMsgInfo>(str);

				if (ret != null && !string.IsNullOrEmpty(ret.Ticket))
				{
					return ret.Ticket;
				}
			}

			return null;
		}
		#endregion

		#region "Public Methods: (UserInfo)"
		public static WeChat_UserInfo GetUserInfo(string openID)
		{
			var userInfo = default(WeChat_UserInfo);
			var errMsg = string.Empty;
			var count = 0;

			while (count < ConfigStore.WeChatAPISettings.WeChat_MP_APIExecutor_MaxNum && !TryGetUserInfo(openID, out userInfo, out errMsg))
			{
				count++;

				if (ConfigStore.CommonSettings.Trace_Mode)
				{
					DBLogger.Instance.ErrorFormat(
						"WeChat API: GetUserInfo({0}), 第 {1:D2} 次失败，错误信息：{2}",
						openID,
						count,
						errMsg
					);
				}

				System.Threading.Thread.Sleep(100);

				// 更新 Access Token
				WeChat_Base_AccessToken_Manager.Instance.Refresh();
			}

			return userInfo;
		}

		static bool TryGetUserInfo(string openID, out WeChat_UserInfo userInfo, out string errMsg)
		{
			var url = string.Format(ConfigStore.WeChatAPISettings.WeChat_MP_GetUserInfoURI, AccessToken.Token, openID);
			var webRequest = WebClientUtility.CreateWebRequest(HttpRequestMethod.GET, url, null, null);

			userInfo = null;
			errMsg = string.Empty;

			using (var response = (HttpWebResponse)webRequest.GetResponse())
			{
				var str = new StreamReader(response.GetResponseStream()).ReadToEnd();
				var ret = Newtonsoft.Json.JsonConvert.DeserializeObject<WeChat_UserInfo_RetMsgInfo>(str);

				if (ret != null)
				{
					if (ret.ErrCode == 0)
					{
						if (ret.Subscribe > 0)
						{
							userInfo = new WeChat_UserInfo { Subscribe = ret.Subscribe, OpenID = ret.OpenID, NickName = ret.NickName, Gender = (ret.Sex == 1), Language = ret.Language, Country = ret.Country, Province = ret.Province, City = ret.City, PortraitsUrl = ret.HeadImgUrl };
						}
						else
						{
							errMsg = string.Format("用户 {0} 已经取消关注！", openID);
						}
					}
					else
					{
						errMsg = string.Format("发生错误未能获取用户 {0} 的 GetUserInfo 的正确结果！错误信息：ErrCode: {1}, ErrMsg: {2}", openID, ret.ErrCode, ret.ErrMsg);
					}
				}
				else
				{
					errMsg = string.Format("发生异常错误，未能获取用户 {0} 的 GetUserInfo 的正确结果！", openID);
				}
			}

			return (userInfo != null && string.IsNullOrEmpty(errMsg));
		}
		#endregion

		#region "Public Methods: (oAuth 2)"
		public static WeChat_OAuth2_AccessTokenInfo AuthticateUser(string code, WeChat_OAuth2_AuthScope mode = WeChat_OAuth2_AuthScope.SNSAPI_UserInfo)
		{
			var accessToken = default(WeChat_OAuth2_AccessTokenInfo);
			var count = 0;

			while (count < ConfigStore.WeChatAPISettings.WeChat_MP_APIExecutor_MaxNum && !TryAuthticateUser(code, out accessToken))
			{
				count++;

				DBLogger.Instance.ErrorFormat("WeChat API: oAuth 2 - AuthticateUser({0}, {1}), 第 {2:D2} 次失败！", code, mode, count);

				System.Threading.Thread.Sleep(100);
			}

			return accessToken;
		}

		static bool TryAuthticateUser(string code, out WeChat_OAuth2_AccessTokenInfo accessToken)
		{
			accessToken = WeChat_OAuth2_AccessToken_Manager.Instance.GetAuthToken(code);

			return (accessToken != null);
		}

		public static WeChat_UserInfo GetAuthUserInfo(string openID, string accessToken, string refreshToken)
		{
			var userInfo = default(WeChat_UserInfo);
			var count = 0;

			while (count < ConfigStore.WeChatAPISettings.WeChat_MP_APIExecutor_MaxNum && !TryGetAuthUserInfo(openID, accessToken, out userInfo))
			{
				count++;

				DBLogger.Instance.ErrorFormat("WeChat API: oAuth 2 - GetAuthUserInfo({0}, {1}), 第 {2:D2} 次失败！", openID, accessToken, count);

				System.Threading.Thread.Sleep(100);

				#region "更新 Access Token"
				var auth_accessToken = WeChat_OAuth2_AccessToken_Manager.Instance.RefreshAuthToken(refreshToken);

				if (auth_accessToken != null)
				{
					accessToken = auth_accessToken.Token;
					refreshToken = auth_accessToken.RefreshToken;
				} 
				#endregion
			}

			return userInfo;
		}

		static bool TryGetAuthUserInfo(string openID, string accessToken, out WeChat_UserInfo userInfo)
		{
			var url = string.Format(ConfigStore.WeChatAPISettings.WeChat_MP_OAuth2_GetUserInfoURI, accessToken, openID);
			var webRequest = WebClientUtility.CreateWebRequest(HttpRequestMethod.GET, url, null, null);

			userInfo = null;

			try
			{
				using (var response = (HttpWebResponse)webRequest.GetResponse())
				{
					var str = new StreamReader(response.GetResponseStream()).ReadToEnd();
					var ret = Newtonsoft.Json.JsonConvert.DeserializeObject<WeChat_UserInfo_RetMsgInfo>(str);

					if (ret != null && ret.ErrCode == 0)
					{
						userInfo = new WeChat_UserInfo(ret);
					}
					else
					{
						throw new Exception(string.Format("发生错误！未能成功使用 OpenID: {0}, AccessToken: {1} 获取用户信息！错误信息：ErrCode: {1}, ErrMsg: {2}", openID, accessToken, ret.ErrCode, ret.ErrMsg));
					}
				}
			}
			catch (Exception ex)
			{
				DBLogger.Instance.Error(ex.ToString());
			}

			return (userInfo != null);
		}
		#endregion

		#region "Public Methods: (GetUserList)"
		public static WeChat_UserList GetUserList()
		{
			var url = string.Format(ConfigStore.WeChatAPISettings.WeChat_MP_GetUserListURI, AccessToken.Token);
			var webRequest = WebClientUtility.CreateWebRequest(HttpRequestMethod.GET, url, null, null);

			using (var response = (HttpWebResponse)webRequest.GetResponse())
			{
				var str = new StreamReader(response.GetResponseStream()).ReadToEnd();
				var ret = Newtonsoft.Json.JsonConvert.DeserializeObject<WeChat_UserList>(str);
				return ret;
			}
		}
		#endregion

		#region "Public Methods: (SendMessage)"
		public static bool SendMessage(string openID, string msg)
		{
			return SendMessage(new CustomerMessage_Text(openID, msg));
		}

		public static bool SendMessage(ICustomerMessageBase msg)
		{
			return SendMessageContent(Newtonsoft.Json.JsonConvert.SerializeObject(msg));
		}

		static bool SendMessageContent(string msg)
		{
			var url = string.Format(ConfigStore.WeChatAPISettings.WeChat_MP_SendMessageURI, AccessToken.Token);
			var webRequest = WebClientUtility.CreateWebRequest(HttpRequestMethod.POST, url, null, msg);

			using (var response = (HttpWebResponse)webRequest.GetResponse())
			{
				var str = new StreamReader(response.GetResponseStream()).ReadToEnd();
				var ret = Newtonsoft.Json.JsonConvert.DeserializeObject<CustomerMessage_RetMsg>(str);

				if (ret != null && ret.ErrCode == 0)
				{
					//if (ConfigStore.CommonSettings.Trace_Mode)
					//{
					//	DBLogger.Instance.InfoFormat("Success to send customer message: {0}", msg);
					//}

					return true;
				}
				else
				{
					DBLogger.Instance.InfoFormat("Fail to send customer message: [{0}], ErrorCode: {1}, ErrorMsg: {2}", msg, ret.ErrCode, ret.ErrMsg);
				}
			}

			return false;
		}
		#endregion
	}

	class WeChat_Base_AccessToken_Manager
	{
		#region "Fields"
		static Lazy<WeChat_Base_AccessToken_Manager> _instance = new Lazy<WeChat_Base_AccessToken_Manager>(() =>
		{
			return new WeChat_Base_AccessToken_Manager();
		});
		#endregion

		#region "Constructs"
		private WeChat_Base_AccessToken_Manager() { }
		#endregion

		#region "Properties"
		public static WeChat_Base_AccessToken_Manager Instance { get { return _instance.Value; } }
		#endregion

		#region "Properties"
		public WeChat_Base_AccessTokenInfo AccessToken { get; private set; }
		#endregion

		#region "Methods"
		public WeChat_Base_AccessTokenInfo Refresh()
		{
			try
			{
				var url = string.Format(ConfigStore.WeChatAPISettings.WeChat_MP_GetAccessTokenURI, ConfigStore.WeChatAPISettings.WeChat_MP_AppID, ConfigStore.WeChatAPISettings.WeChat_MP_AppSecret);
				var webRequest = WebClientUtility.CreateWebRequest(HttpRequestMethod.GET, url);

				using (var response = (HttpWebResponse)webRequest.GetResponse())
				{
					var str = new StreamReader(response.GetResponseStream()).ReadToEnd();
					var ret = Newtonsoft.Json.JsonConvert.DeserializeObject<WeChat_Base_AccessToken_RetMsgInfo>(str);

					if (ret != null && ret.ErrCode == 0)
					{
						if (this.AccessToken == null)
						{
							this.AccessToken = new WeChat_Base_AccessTokenInfo(ret);
						}
						else
						{
							this.AccessToken.Token = ret.AccessToken;
							this.AccessToken.CreateDate = DateTime.Now;
							this.AccessToken.ExpireDate = DateTime.Now.AddSeconds(ret.ExpiresIn);
						}

						DBLogger.Instance.InfoFormat("成功获取基础支持 AccessToken: [{0}]", this.AccessToken);
					}
					else
					{
						throw new Exception(string.Format("返回错误结果：{0}", str));
					}
				}
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat("试图获取基础支持 AccessToken 失败！Exception: {0}", ex.Message);
			}

			return this.AccessToken;
		} 
		#endregion
	}

	class WeChat_OAuth2_AccessToken_Manager
	{
		#region "Fields"
		static Lazy<WeChat_OAuth2_AccessToken_Manager> _instance = new Lazy<WeChat_OAuth2_AccessToken_Manager>(() =>
		{
			return new WeChat_OAuth2_AccessToken_Manager();
		});
		#endregion

		#region "Constructs"
		private WeChat_OAuth2_AccessToken_Manager() { }
		#endregion

		#region "Properties"
		public static WeChat_OAuth2_AccessToken_Manager Instance { get { return _instance.Value; } }
		#endregion

		#region "Methods"
		public WeChat_OAuth2_AccessTokenInfo GetAuthToken(string code, WeChat_OAuth2_AuthScope scope = WeChat_OAuth2_AuthScope.SNSAPI_UserInfo)
		{
			var url = string.Format(ConfigStore.WeChatAPISettings.WeChat_MP_OAuth2_GetAccessTokenURI, ConfigStore.WeChatAPISettings.WeChat_MP_AppID, ConfigStore.WeChatAPISettings.WeChat_MP_AppSecret, code);
			var webRequest = WebClientUtility.CreateWebRequest(HttpRequestMethod.GET, url);

			try
			{
				using (var response = (HttpWebResponse)webRequest.GetResponse())
				{
					var str = new StreamReader(response.GetResponseStream()).ReadToEnd();
					var ret = Newtonsoft.Json.JsonConvert.DeserializeObject<WeChat_OAuth2_AccessToken_RetMsgInfo>(str);
					var accessToken = default(WeChat_OAuth2_AccessTokenInfo);

					if (ret != null && ret.ErrCode == 0)
					{
						accessToken = new WeChat_OAuth2_AccessTokenInfo(ret);

						if (ConfigStore.CommonSettings.Trace_Mode)
						{
							DBLogger.Instance.InfoFormat("WeChat API: oAuth 2 - GetAuthToken({0}, {1}), 成功获取 AccessToken: [{2}]", code, scope, accessToken);
						}
					}
					else
					{
						accessToken = null;

						throw new Exception(string.Format("发生错误！未能成功使用 Code: {0} 换取对应的 OpenID！错误信息：ErrCode: {1}, ErrMsg: {2}", code, ret.ErrCode, ret.ErrMsg));
					}

					return accessToken;
				}
			}
			catch (Exception ex)
			{
				DBLogger.Instance.Error(ex.ToString());
			}

			return null;
		}

		public WeChat_OAuth2_AccessTokenInfo RefreshAuthToken(string refreshToken)
		{
			var url = string.Format(ConfigStore.WeChatAPISettings.WeChat_MP_OAuth2_RefAccessTokenURI, ConfigStore.WeChatAPISettings.WeChat_MP_AppID, ConfigStore.WeChatAPISettings.WeChat_MP_AppSecret);
			var webRequest = WebClientUtility.CreateWebRequest(HttpRequestMethod.GET, url);

			try
			{
				using (var response = (HttpWebResponse)webRequest.GetResponse())
				{
					var str = new StreamReader(response.GetResponseStream()).ReadToEnd();
					var ret = Newtonsoft.Json.JsonConvert.DeserializeObject<WeChat_OAuth2_AccessToken_RetMsgInfo>(str);
					var accessToken = default(WeChat_OAuth2_AccessTokenInfo);

					if (ret != null && ret.ErrCode == 0)
					{
						accessToken = new WeChat_OAuth2_AccessTokenInfo(ret);

						if (ConfigStore.CommonSettings.Trace_Mode)
						{
							DBLogger.Instance.InfoFormat("WeChat API: oAuth 2 - RefreshAuthToken({0}), 成功获取 AccessToken: [{1}]", refreshToken, accessToken);
						}
					}
					else
					{
						accessToken = null;

						throw new Exception(string.Format("发生错误！未能成功使用 RefreshToken: {0} 刷新 AccessToken！错误信息：ErrCode: {1}, ErrMsg: {2}", refreshToken, ret.ErrCode, ret.ErrMsg));
					}

					return accessToken;
				}
			}
			catch (Exception ex)
			{
				DBLogger.Instance.Error(ex.ToString());
			}

			return null;
		}
		#endregion
	}
}
