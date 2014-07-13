using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

using SNS.Apps.KPC.Libs.Configurations;

namespace SNS.Apps.KPC.Libs.Models.WeChat
{
	#region "Simulate_Login"
	[Serializable]
	public class WeChat_Simulate_LoginInfo
	{
		#region "Properties"
		/// <summary>
		/// 登录后得到的令牌
		/// </summary>        
		public string Token { get; set; }

		/// <summary>
		/// 登录后得到的cookie
		/// </summary>
		public CookieContainer Cookie { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime? CreateDate { get; set; }

		public DateTime? ExpireDate { get { return (this.CreateDate != null && this.CreateDate.HasValue) ? (this.CreateDate.Value.AddMinutes(60)) : (DateTime.Now); } } 
		#endregion
	}

	[Serializable]
	public class WeChat_Simulate_Login_RetMsgInfo
	{
		#region "Properties"
		public int Ret { get; set; }

		public int ShowVerifyCode { get; set; }

		[Newtonsoft.Json.JsonProperty("errcode")]
		public int ErrCode { get; set; }

		[Newtonsoft.Json.JsonProperty("errmsg")]
		public string ErrMsg { get; set; } 
		#endregion
	}
	#endregion

	#region "Access Token (Base)"
	[Serializable]
	public class WeChat_Base_AccessTokenInfo
	{
		#region "Constructs"
		public WeChat_Base_AccessTokenInfo() { }

		public WeChat_Base_AccessTokenInfo(WeChat_Base_AccessToken_RetMsgInfo retInfo)
		{
			this.Token = retInfo.AccessToken;
			this.CreateDate = DateTime.Now;
			this.ExpireDate = DateTime.Now.AddSeconds(retInfo.ExpiresIn);
		} 
		#endregion

		#region "Properties"
		public string Token { get; set; }

		public DateTime? CreateDate { get; set; }

		public DateTime? ExpireDate { get; set; } 
		#endregion

		#region "Events"
		public override string ToString()
		{
			return string.Format("Token: {0}, CreateDate: {1}, ExpireDate: {2}", this.Token, (this.CreateDate != null && this.CreateDate.HasValue ? this.CreateDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "NULL"), (this.ExpireDate != null && this.ExpireDate.HasValue ? this.ExpireDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "NULL"));
		}
		#endregion
	}

	[Serializable]
	public class WeChat_Base_AccessToken_RetMsgInfo
	{
		[Newtonsoft.Json.JsonProperty("access_token")]
		public string AccessToken { get; set; }

		[Newtonsoft.Json.JsonProperty("expires_in")]
		public int ExpiresIn { get; set; }

		[Newtonsoft.Json.JsonProperty("errcode")]
		public int ErrCode { get; set; }

		[Newtonsoft.Json.JsonProperty("errmsg")]
		public string ErrMsg { get; set; }
	}
	#endregion

	#region "Access Token (OAuth 2)"
	[DataContract]
	public class WeChat_OAuth2_AccessTokenInfo
	{
		#region "Constructs"
		public WeChat_OAuth2_AccessTokenInfo() { }

		public WeChat_OAuth2_AccessTokenInfo(WeChat_OAuth2_AccessToken_RetMsgInfo retInfo)
		{
			if (retInfo != null && retInfo.ErrCode == 0)
			{
				this.Token = retInfo.AccessToken;
				this.RefreshToken = retInfo.RefreshToken;
				this.OpenID = retInfo.OpenID;
				this.Scope = (string.Compare(retInfo.Scope, WeChat_OAuth2_AuthScope.SNSAPI_UserInfo.ToString(), true) == 0) ? WeChat_OAuth2_AuthScope.SNSAPI_UserInfo : WeChat_OAuth2_AuthScope.SNSAPI_Base;
				this.CreateDate = DateTime.Now;
				this.ExpireDate = DateTime.Now.AddSeconds(retInfo.ExpiresIn);
			}
		}
		#endregion

		#region "Properties"
		[DataMember]
		public string Token { get; set; }

		[DataMember]
		public string OpenID { get; set; }

		[DataMember]
		public string RefreshToken { get; set; }

		[DataMember]
		public WeChat_OAuth2_AuthScope Scope { get; set; }

		[DataMember]
		public DateTime? CreateDate { get; set; }

		[DataMember]
		public DateTime? ExpireDate { get; set; }
		#endregion

		#region "Methods"
		public override string ToString()
		{
			return string.Format("Token: {0}, OpenID: {1}, RefreshToken: {2}, Scope: {3}, CreateDate: {4}, ExpireDate: {5}", this.Token, this.OpenID, this.RefreshToken, this.Scope, (this.CreateDate != null && this.CreateDate.HasValue ? this.CreateDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "NULL"), (this.ExpireDate != null && this.ExpireDate.HasValue ? this.ExpireDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "NULL"));
		}
		#endregion
	}

	public class WeChat_OAuth2_AccessToken_RetMsgInfo
	{
		#region "Properties"
		[Newtonsoft.Json.JsonProperty("access_token")]
		public string AccessToken { get; set; }

		[Newtonsoft.Json.JsonProperty("expires_in")]
		public int ExpiresIn { get; set; }

		[Newtonsoft.Json.JsonProperty("refresh_token")]
		public string RefreshToken { get; set; }

		[Newtonsoft.Json.JsonProperty("openid")]
		public string OpenID { get; set; }

		[Newtonsoft.Json.JsonProperty("scope")]
		public string Scope { get; set; }

		[Newtonsoft.Json.JsonProperty("errcode")]
		public int ErrCode { get; set; }

		[Newtonsoft.Json.JsonProperty("errmsg")]
		public string ErrMsg { get; set; }
		#endregion
	}

	public enum WeChat_OAuth2_AuthScope
	{
		SNSAPI_Base,
		SNSAPI_UserInfo
	}

	public static class WeChat_OAuth2_Util
	{
		#region "Methods"
		public static string BuildAuthUrl(string returnUrl)
		{
			return BuildAuthUrl(returnUrl, WeChat_OAuth2_AuthScope.SNSAPI_UserInfo);
		}

		public static string BuildAuthUrl(string returnUrl, string state)
		{
			return BuildAuthUrl(returnUrl, WeChat_OAuth2_AuthScope.SNSAPI_UserInfo, state);
		}

		public static string BuildAuthUrl(string returnUrl, WeChat_OAuth2_AuthScope scope = WeChat_OAuth2_AuthScope.SNSAPI_UserInfo, string state = "0")
		{
			return string.Format(ConfigStore.WeChatAPISettings.WeChat_MP_OAuth2_AuthURI, ConfigStore.WeChatAPISettings.WeChat_MP_AppID, returnUrl, scope.ToString().ToLower(), state);
		}
		#endregion
	}
	#endregion

	#region "QRCode"
	[Serializable]
	public class WeChat_QRCodeTicket_ReqInfo
	{
		#region "Constructs"
		public WeChat_QRCodeTicket_ReqInfo(QRCodeActionType actionType, int sceneID)
		{
			switch (actionType)
			{
				case QRCodeActionType.QR_SCENE:
					this.ExpiresIn = 1800;
					break;
			}

			this.ActionName = actionType.ToString();
			this.ActionInfo = new WeChat_QRCodeTicket_ReqActionInfo { Scene = new WeChat_QRCodeTicket_ReqActionSceneInfo { SceneID = sceneID } };
		}
		#endregion

		#region "Properties"
		[Newtonsoft.Json.JsonProperty("expire_seconds ")]
		public int ExpiresIn { get; set; }

		[Newtonsoft.Json.JsonProperty("action_name")]
		public string ActionName { get; set; }

		[Newtonsoft.Json.JsonProperty("action_info")]
		public WeChat_QRCodeTicket_ReqActionInfo ActionInfo { get; set; }
		#endregion
	}

	[Serializable]
	public class WeChat_QRCodeTicket_ReqActionInfo
	{
		#region "Properties"
		[Newtonsoft.Json.JsonProperty("scene")]
		public WeChat_QRCodeTicket_ReqActionSceneInfo Scene { get; set; } 
		#endregion
	}

	[Serializable]
	public class WeChat_QRCodeTicket_ReqActionSceneInfo
	{
		#region "Properties"
		[Newtonsoft.Json.JsonProperty("scene_id")]
		public int SceneID { get; set; } 
		#endregion
	}

	[Serializable]
	public class WeChat_QRCodeTicket_RetMsgInfo
	{
		#region "Properties"
		[Newtonsoft.Json.JsonProperty("ticket")]
		public string Ticket { get; set; }

		[Newtonsoft.Json.JsonProperty("expire_seconds ")]
		public int ExpiresIn { get; set; }

		[Newtonsoft.Json.JsonProperty("errcode")]
		public int ErrCode { get; set; }

		[Newtonsoft.Json.JsonProperty("errmsg")]
		public string ErrMsg { get; set; }
		#endregion
	}
	#endregion

	#region "UserInfo"
	[DataContract]
	public class WeChat_UserInfo
	{
		#region "Constructs"
		public WeChat_UserInfo() { }

		public WeChat_UserInfo(WeChat_UserInfo_RetMsgInfo ret)
		{
			this.Subscribe = ret.Subscribe;
			this.OpenID = ret.OpenID;
			this.NickName = ret.NickName;
			this.Gender = (ret.Sex == 1) ? (true) : (ret.Sex == 2 ? false : (bool?)null);
			this.Language = ret.Language;
			this.Country = ret.Country;
			this.Province = ret.Province;
			this.City = ret.City;
			this.PortraitsUrl = ret.HeadImgUrl;
		} 
		#endregion

		#region "Properties"
		[DataMember]
		public int Subscribe { get; set; }

		[DataMember]
		public string OpenID { get; set; }

		[DataMember]
		public string NickName { get; set; }

		[DataMember]
		public Nullable<bool> Gender { get; set; }

		[DataMember]
		public string Language { get; set; }

		[DataMember]
		public string Country { get; set; }

		[DataMember]
		public string Province { get; set; }

		[DataMember]
		public string City { get; set; }

		[DataMember]
		public string PortraitsUrl { get; set; }
		#endregion

		#region "Properties"
		[DataMember]
		public bool IsExtended { get; set; }

		[DataMember]
		public string ExtendChannel { get; set; } 
		#endregion

		#region "Methods"
		public override string ToString()
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(this);
		} 
		#endregion
	}

	[Serializable]
	public class WeChat_UserInfo_RetMsgInfo
	{
		#region "Properties"
		[Newtonsoft.Json.JsonProperty("subscribe")]
		public int Subscribe { get; set; }

		[Newtonsoft.Json.JsonProperty("openid")]
		public string OpenID { get; set; }

		[Newtonsoft.Json.JsonProperty("nickname")]
		public string NickName { get; set; }

		[Newtonsoft.Json.JsonProperty("sex")]
		public int Sex { get; set; }

		[Newtonsoft.Json.JsonProperty("language")]
		public string Language { get; set; }

		[Newtonsoft.Json.JsonProperty("country")]
		public string Country { get; set; }

		[Newtonsoft.Json.JsonProperty("province")]
		public string Province { get; set; }

		[Newtonsoft.Json.JsonProperty("city")]
		public string City { get; set; }

		[Newtonsoft.Json.JsonProperty("headimgurl")]
		public string HeadImgUrl { get; set; }

		[Newtonsoft.Json.JsonProperty("errcode")]
		public int ErrCode { get; set; }

		[Newtonsoft.Json.JsonProperty("errmsg")]
		public string ErrMsg { get; set; }
		#endregion
	}
	#endregion
}
