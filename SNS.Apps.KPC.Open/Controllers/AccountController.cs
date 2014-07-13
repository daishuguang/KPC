using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.IServices;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;

namespace SNS.Apps.KPC.Open.Controllers
{
	public class AccountController : Base.BaseController
	{
		#region "Action: WechatAuth"
		public ActionResult WechatAuth()
		{
			var code = Request.QueryString["Code"];
			var state = Request.QueryString["State"];
			var returnUrl = Request.QueryString["ReturnUrl"];

			var clientW = CreateServiceClient<IWeChatService>();
			var clientR = CreateServiceClient<IRepositoryService>();

			try
			{
				var openID = string.Empty;
				var userInstance = default(User);
				var userWeChat = default(Libs.Models.WeChat.WeChat_UserInfo);

				if (string.Compare(code, "authdeny", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					#region "转向前一个页面或关闭授权页"
					var idx = -1;

					if (!string.IsNullOrEmpty(returnUrl))
					{
						idx = returnUrl.ToLower().IndexOf("&prevurl=");

						if (idx == -1)
						{
							idx = returnUrl.ToLower().IndexOf("?prevurl=");
						}
					}

					if (idx == -1)
					{
						var sbContent = new System.Text.StringBuilder();

						sbContent.Append("<script>");
						sbContent.Append("	alert('很抱歉，未能获得您的授权，页面将关闭。。。');");
						sbContent.Append("	document.addEventListener('WeixinJSBridgeReady', function onBridgeReady() { WeixinJSBridge.call('closeWindow'); });");
						sbContent.Append("</script>");

						if (ConfigStore.CommonSettings.Trace_Mode)
						{
							// Log
							DBLogger.Instance.InfoFormat("授权失败：用户拒绝授权，页面将关闭！\r\nUrl: {0}, Code: {1}, State: {2}, ReturnUrl: {3}", HttpContext.Request.RawUrl, code, state, returnUrl);
						}

						return new ContentResult() { Content = sbContent.ToString() };
					}
					else
					{
						var prevurl = HttpUtility.UrlDecode(returnUrl.Substring(idx + 9));

						if (ConfigStore.CommonSettings.Trace_Mode)
						{
							// Log
							DBLogger.Instance.InfoFormat("授权失败：用户拒绝授权，将转向页面：{4}！\r\nUrl: {0}, Code: {1}, State: {2}, ReturnUrl: {3}", HttpContext.Request.RawUrl, code, state, returnUrl, prevurl);
						}

						return Redirect(prevurl);
					}
					#endregion
				}
				else if (!string.IsNullOrEmpty(code))
				{
					if (ConfigStore.CommonSettings.Trace_Mode)
					{
						// 用户已授权
						DBLogger.Instance.InfoFormat("授权成功：用户已授权，即将获取用户信息！\r\nUrl: {0}, Code: {1}, OpenID: {2}, State: {3}, ReturnUrl: {4}", HttpContext.Request.RawUrl, code, openID, state, returnUrl);
					}

					#region "获取授权令牌"
					var accessToken = clientW.AuthenticateUser(code, Libs.Models.WeChat.WeChat_OAuth2_AuthScope.SNSAPI_UserInfo);

					if (accessToken != null)
					{
						openID = accessToken.OpenID;
						userInstance = clientR.GetUser(openID, true);
						userWeChat = clientW.GetUserInfo(openID, accessToken.Token, accessToken.RefreshToken);

						if (userWeChat == null)
						{
							DBLogger.Instance.ErrorFormat("获取用户信息失败（API）！OpenID: {0}, AccessToken: {1}, RefreshToken: {2}, Scope: {3}, State: {4}", openID, accessToken.Token, accessToken.RefreshToken, accessToken.Scope, state);

							if (userInstance == null)
							{
								throw new Exception(string.Format("获取用户信息失败（API & DB）！OpenID: {0}, AccessToken: {1}, RefreshToken: {2}, Scope: {3}, State: {4}", openID, accessToken.Token, accessToken.RefreshToken, accessToken.Scope, state));
							}
						}
						else
						{
							// 非快拼车公众号用户（微信环境）
							userWeChat.IsExtended = (userWeChat.Subscribe != 0);

							if (userWeChat.IsExtended && userInstance == null)
							{
								// this.ExtendChannel == 0	: 朋友圈分享，或别的公众平台链接进入（非合作平台）；
								// this.ExtendChannel != 0	: 其它公众平台（合作平台）
								userWeChat.ExtendChannel = this.ExtendChannel; 				
							}

							if (ConfigStore.CommonSettings.Trace_Mode)
							{
								DBLogger.Instance.InfoFormat("成功获取用户信息（API）！OpenID: {0}, AccessToken: {1}, RefreshToken: {2}, Scope: {3}, State: {4}", openID, accessToken.Token, accessToken.RefreshToken, accessToken.Scope, state);
							}
						}

						#region "未关注的用户：加关注用户"
						if (userInstance == null)
						{
							// 加关注用户（写入数据库）
							userInstance = clientR.Subscribe(openID, userWeChat);
						} 
						#endregion

						#region "已关注用户：更新用户信息"
						else if (userWeChat != null)
						{
							System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(Async_UpdateProfileData), new { UserID = userInstance.ID, OpenID = userInstance.OpenID, UserWeChat = userWeChat });
						} 
						#endregion
					}
					else
					{
						// 获取授权令牌失败
						DBLogger.Instance.ErrorFormat("获取授权令牌失败：用户已授权，但未能获取用户令牌，详见日志文件：task.log.data！\r\nUrl: {0}, Code: {1}, OpenID: {2}, State: {3}, ReturnUrl: {4}", HttpContext.Request.RawUrl, code, openID, state, returnUrl);
					}
					#endregion

					#region "写入授权 Cookie"
					if (!string.IsNullOrEmpty(openID) && userInstance != null)
					{
						#region "写入授权 Cookie"
						//FormsAuthentication.SetAuthCookie(userInstance.UserGUID.ToString(), false);
						CustomAuthentication.SetAuthCookie(userInstance);
						#endregion

						if (ConfigStore.CommonSettings.Trace_Mode)
						{
							DBLogger.Instance.InfoFormat("用户 {0} ({1}, {2}) 已成功登陆系统，转向URL：{3}", userInstance.NickName, userInstance.ID, userInstance.UserGUID, returnUrl);
						}

						if (!string.IsNullOrEmpty(returnUrl))
						{
							return Redirect(returnUrl);
						}

						return RedirectToAction("Search", "Route");
					}
					#endregion
				}

				// 授权失败，且未处理
				DBLogger.Instance.ErrorFormat("授权失败：发生异常或未处理的授权异常！\r\nUrl: {0}, Code: {1}, OpenID: {2}, State: {3}, ReturnUrl: {4}", HttpContext.Request.RawUrl, code, openID, state, returnUrl);

				throw new UnauthorizedAccessException();
			}
			catch
			{
				if (!string.IsNullOrEmpty(returnUrl))
				{
					var authUrl = Libs.Models.WeChat.WeChat_OAuth2_Util.BuildAuthUrl(HttpUtility.UrlEncode(string.Format("{0}/account/wechatauth?returnurl={1}", ConfigStore.CommonSettings.App_Site_Url.TrimEnd('/'), returnUrl)), this.ExtendChannel);
					
					ViewBag.AuthUrl = authUrl;
				}

				return View("Error_NoAuth");
			}
			finally
			{
				CloseServiceClient(clientW);
				CloseServiceClient(clientR);
			}
		} 
		#endregion

		#region "Private Methods"
		void Async_UpdateProfileData(dynamic state)
		{
			if (state == null)
			{
				return;
			}

			var useriD = state.UserID;
			var openID = state.OpenID;
			var userWeChat = state.UserWeChat;

			if (useriD != 0 && !string.IsNullOrEmpty(openID) && userWeChat != null)
			{
				var clientR = CreateServiceClient<IRepositoryService>();

				try
				{
					clientR.UpdateProfile(useriD, openID, userWeChat);

					if (ConfigStore.CommonSettings.Trace_Mode)
					{
						DBLogger.Instance.InfoFormat("Success to update UserProfile for user: UserID: {0}, OpenID: {1}", useriD, openID);
					}
				}
				catch (Exception ex)
				{
					DBLogger.Instance.InfoFormat("Fail to update UserProfile for user: UserID: {0}, OpenID: {1}, with exception: {2}", useriD, openID, ex.ToString());
				}
				finally
				{
					CloseServiceClient(clientR);
				}
			}
		} 
		#endregion
	}
}
