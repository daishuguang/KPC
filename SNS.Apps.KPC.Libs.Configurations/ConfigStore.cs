using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SNS.Apps.KPC.Libs.Configurations
{
	public static class ConfigStore
	{
		public static class CommonSettings
		{
			#region "Fields"
			const string CNSTR_CSKEY_APP_SITE_URL = "App_Site_Url";
			const string CNSTR_CSKEY_APP_VERSION = "App_Version";

			const string CNSTR_CSKEY_OFFICIAL_SITE = "Official_Site";
			const string CNSTR_CSKEY_OFFICIAL_CHANNEL = "Official_Channel";
			const string CNSTR_CSKEY_OFFICIAL_WEIBO = "Official_Weibo";
			const string CNSTR_CSKEY_OFFICIAL_WEIXIN = "Official_Weixin";
			const string CNSTR_CSKEY_OFFICIAL_EMAIL = "Official_Email";
			const string CNSTR_CSKEY_OFFICIAL_TEL = "Official_Tel";

			const string CNSTR_CSKEY_CUSTOMERSERVICE_SUPERVISOR_OPENID = "CustomerService_Supervisor_OpenID";
			const string CNSTR_CSKEY_CUSTOMERSERVICE_SUPERVISOR_QQ = "CustomerService_Supervisor_QQ";
			const string CNSTR_CSKEY_CUSTOMERSERVICE_ADMINISTRATOR_OPENID = "CustomerService_Administrator_OpenID";

			const string CNSTR_CSKEY_PORTRAITS_FOLDER = "Portraits_Folder";
			const string CNSTR_CSKEY_PORTRAITS_IMAGE = "Portraits_Image";
			const string CNSTR_CSKEY_PORTRAITS_IMAGETHUMB = "Portraits_ImageThumb";
			const string CNSTR_CSKEY_PORTRAITS_IMAGEDEFAULT = "Portraits_ImageDefault";
			const string CNSTR_CSKEY_PORTRAITS_TEMPICON = "Portraits_TempIcon";
			const string CNSTR_CSKEY_PORTRAITS_TEMPICON_RANGEMIN = "Portraits_TempIcon_RangeMin";
			const string CNSTR_CSKEY_PORTRAITS_TEMPICON_RANGEMAX = "Portraits_TempIcon_RangeMax";

			const string CNSTR_CSKEY_WEBREQUEST_USERAGENT = "WebRequest_UserAgent";

			const string CNSTR_CSKEY_DEBUG_MODE = "Debug_Mode";
			const string CNSTR_CSKEY_DEBUG_User = "Debug_User";
			const string CNSTR_CSKEY_TRACE_MODE = "Trace_Mode";
			const string CNSTR_CSKEY_MAINTAIN_MODE = "Maintain_Mode";

			const string CNSTR_CSKEY_IMPORTDATA_ENABLED = "ImportData_Enabled";
			const string CNSTR_CSKEY_IMPORTDATA_STARTEDID = "ImportData_StartedID";

			const string CNSTR_CSKEY_EXTERNALDATA_ENABLED = "ExternalData_Enabled";
			const string CNSTR_CSKEY_EXTERNALDATA_STARTEDID = "ExternalData_StartedID";

			const string CNSTR_CSKEY_MEMCACHE_EXPIRESIN = "MemCache_ExpiresIn";
			const string CNSTR_CSKEY_MEMCACHE_ENABLED = "MemCache_Enabled";

			const string CNSTR_CSKEY_BAIDU_TRACKPAGEVIEW_SITEID = "Baidu_TrackPageView_SiteID";
			#endregion

			#region "Properties: App Settings"
			public static string App_Site_Url
			{
				get
				{
					return ConfigurationManager.AppSettings[CNSTR_CSKEY_APP_SITE_URL].TrimEnd(' ', '/');
				}
			}

			public static string App_Version { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_APP_VERSION]; } }
			#endregion

			#region "Properties: Official"
			public static string Official_Site { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_APP_SITE_URL]; } }

			public static string Official_Channel { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_OFFICIAL_CHANNEL]; } }

			public static string Official_Weibo { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_OFFICIAL_WEIBO]; } }

			public static string Official_Weixin { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_OFFICIAL_WEIXIN]; } }

			public static string Official_Email { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_OFFICIAL_EMAIL]; } }

			public static string Official_Tel { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_OFFICIAL_TEL]; } }
			#endregion

			#region "Properties: CustomerService"
			public static string CustomerService_Supervisor_OpenID { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_CUSTOMERSERVICE_SUPERVISOR_OPENID]; } }

			public static string CustomerService_Supervisor_QQ { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_CUSTOMERSERVICE_SUPERVISOR_QQ]; } }

			public static string CustomerService_Administrator_OpenID { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_CUSTOMERSERVICE_ADMINISTRATOR_OPENID]; } }
			#endregion

			#region "Properties: Debug/Trace Mode"
			public static bool Debug_Mode { get { return Convert.ToBoolean(ConfigurationManager.AppSettings[CNSTR_CSKEY_DEBUG_MODE]); } }

			public static string Debug_User { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_DEBUG_User]; } }

			public static bool Trace_Mode { get { return Convert.ToBoolean(ConfigurationManager.AppSettings[CNSTR_CSKEY_TRACE_MODE]); } }

			public static bool Maintain_Mode { get { return Convert.ToBoolean(ConfigurationManager.AppSettings[CNSTR_CSKEY_MAINTAIN_MODE]); } }
			#endregion

			#region "Properties: Portraits"
			public static string Portraits_Folder
			{
				get
				{
					return ConfigurationManager.AppSettings[CNSTR_CSKEY_PORTRAITS_FOLDER];
				}
			}

			public static string Portraits_ImageDefault
			{
				get
				{
					return ConfigurationManager.AppSettings[CNSTR_CSKEY_PORTRAITS_IMAGEDEFAULT];
				}
			}

			public static string Portraits_Image
			{
				get
				{
					return ConfigurationManager.AppSettings[CNSTR_CSKEY_PORTRAITS_IMAGE];
				}
			}

			public static string Portraits_ImageThumb
			{
				get
				{
					return ConfigurationManager.AppSettings[CNSTR_CSKEY_PORTRAITS_IMAGETHUMB];
				}
			}

			public static string Portraits_TempIcon
			{
				get
				{
					return ConfigurationManager.AppSettings[CNSTR_CSKEY_PORTRAITS_TEMPICON];
				}
			}

			public static int Portraits_TempIcon_RangeMin
			{
				get
				{
					return Convert.ToInt32(ConfigurationManager.AppSettings[CNSTR_CSKEY_PORTRAITS_TEMPICON_RANGEMIN]);
				}
			}

			public static int Portraits_TempIcon_RangeMax
			{
				get
				{
					return Convert.ToInt32(ConfigurationManager.AppSettings[CNSTR_CSKEY_PORTRAITS_TEMPICON_RANGEMAX]);
				}
			}
			#endregion

			#region "Properties: WebRequest"
			public static string WebRequest_UserAgent { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_WEBREQUEST_USERAGENT]; } }
			#endregion

			#region "Properties: ImportData"
			public static bool ImportData_Enabled { get { return Convert.ToBoolean(ConfigurationManager.AppSettings[CNSTR_CSKEY_IMPORTDATA_ENABLED]); } }

			public static long ImportData_StartedID { get { return Convert.ToInt64(ConfigurationManager.AppSettings[CNSTR_CSKEY_IMPORTDATA_STARTEDID]); } }
			#endregion

			#region "Properties: ExternalData"
			public static bool ExternalData_Enabled { get { return Convert.ToBoolean(ConfigurationManager.AppSettings[CNSTR_CSKEY_EXTERNALDATA_ENABLED]); } }

			public static long ExternalData_StartedID { get { return Convert.ToInt64(ConfigurationManager.AppSettings[CNSTR_CSKEY_EXTERNALDATA_STARTEDID]); } }
			#endregion

			#region "Properties: MemCache"
			public static int MemCache_ExpiresIn { get { return Convert.ToInt32(ConfigurationManager.AppSettings[CNSTR_CSKEY_MEMCACHE_EXPIRESIN]); } }

			public static bool MemCache_Enabled { get { return Convert.ToBoolean(ConfigurationManager.AppSettings[CNSTR_CSKEY_MEMCACHE_ENABLED]); } }
			#endregion

			#region "Properties: Baidu_TrackPageView_SiteID"
			public static string Baidu_TrackPageView_SiteID { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_BAIDU_TRACKPAGEVIEW_SITEID]; } }
			#endregion
		}

		public static class MapAPISettings
		{
			#region "Fields"
			const string CNSTR_CSDATA_USERAGENT_WINNT = "Windows NT";
			const string CNSTR_CSDATA_USERAGENT_MSIE = "compatible; MSIE";
			const string CNSTR_CSDATA_USERAGENT_ANDROID = "Android";

			// MAPAPI
			const string CNSTR_CSKEY_MAPAPIKEY_INDICATOR_SERVER = "MapAPIKey_Server";
			const string CNSTR_CSKEY_MAPAPIKEY_INDICATOR_MOBILE = "MapAPIKey_Mobile";
			const string CNSTR_CSKEY_MAPAPIKEY_INDICATOR_BROWSER = "MapAPIKey_Browser";

			const string CNSTR_CSKEY_BAIDU_LOCATION_GEOCODING_URI = "Baidu_Location_Geocoding_URI";
			const string CNSTR_CSKEY_BAIDU_LOCATION_REVERSE_URI = "Baidu_Location_Reverse_URI";

			const string CNSTR_CSKEY_BAIDU_MAP_URI = "Baidu_Map_URI";
			#endregion

			#region "Properties"
			/// <summary>
			/// 
			/// </summary>
			public static string MapAPIKey
			{
				get
				{
					var indicator = CNSTR_CSKEY_MAPAPIKEY_INDICATOR_BROWSER;

					if (HttpContext.Current != null && HttpContext.Current.Request != null)
					{
						var agent = HttpContext.Current.Request.UserAgent;

						if (!string.IsNullOrEmpty(agent) && agent.IndexOf(CNSTR_CSDATA_USERAGENT_ANDROID) != -1)
						{
							indicator = CNSTR_CSKEY_MAPAPIKEY_INDICATOR_MOBILE;
						}
					}

					return ConfigurationManager.AppSettings[indicator];
				}
			}

			public static string MapAPIKey_Mobile { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_MAPAPIKEY_INDICATOR_MOBILE]; } }

			public static string MapAPIKey_Browser { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_MAPAPIKEY_INDICATOR_BROWSER]; } }

			public static string MapAPIKey_Server { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_MAPAPIKEY_INDICATOR_SERVER]; } }

			public static string Baidu_Location_Geocoding_URI { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_BAIDU_LOCATION_GEOCODING_URI]; } }

			public static string Baidu_Location_Reverse_URI { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_BAIDU_LOCATION_REVERSE_URI]; } }

			public static string Baidu_Map_URI { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_BAIDU_MAP_URI]; } }
			#endregion
		}

		public static class TaskServiceSettings
		{
			#region "Fields"
			const string CNSTR_CSKEY_TASKSERVICE_INTERVAL = "TaskService_Interval";
			#endregion

			#region "Properties"
			public static int TaskService_Interval { get { return Convert.ToInt32(ConfigurationManager.AppSettings[CNSTR_CSKEY_TASKSERVICE_INTERVAL]); } }
			#endregion
		}

		public static class RouteMatrixServiceSettings
		{
			#region "Fields"
			const string CNSTR_CSKEY_ROUTEMATRIX_EXPIRESINDAYS_CITYWIDE = "RouteMatrix_ExpiresInDays_CityWide";
			const string CNSTR_CSKEY_ROUTEMATRIX_EXPIRESINDAYS_INTERCITY = "RouteMatrix_ExpiresInDays_Intercity";

			const string CNSTR_CSKEY_ROUTEMATRIX_MATCHRATE = "RouteMatrix_MatchRate";
			const string CNSTR_CSKEY_ROUTEMATRIX_MATCHDIST_CITYWIDE = "RouteMatrix_MatchDist_Citywide";
			const string CNSTR_CSKEY_ROUTEMATRIX_MATCHDIST_INTERCITY = "RouteMatrix_MatchDist_Intercity";
			const string CNSTR_CSKEY_ROUTEMATRIX_MATCHPERC_INTERCITY = "RouteMatrix_MatchPerc_Intercity";
			#endregion

			#region "Properties"
			/// <summary>
			/// 即时拼车失效期
			/// </summary>
			public static int RouteMatrix_ExpiresInDays_CityWide { get { return Convert.ToInt32(ConfigurationManager.AppSettings[CNSTR_CSKEY_ROUTEMATRIX_EXPIRESINDAYS_CITYWIDE]); } }

			/// <summary>
			/// 城际拼车失效期
			/// </summary>
			public static int RouteMatrix_ExpiresInDays_Intercity { get { return Convert.ToInt32(ConfigurationManager.AppSettings[CNSTR_CSKEY_ROUTEMATRIX_EXPIRESINDAYS_INTERCITY]); } }

			/// <summary>
			/// 满足匹配最小分值
			/// </summary>
			public static int RouteMatrix_MatchRate { get { return Convert.ToInt32(ConfigurationManager.AppSettings[CNSTR_CSKEY_ROUTEMATRIX_MATCHRATE]); } }

			/// <summary>
			/// 即时拼车接送距离最大容忍度（公里）
			/// </summary>
			public static int RouteMatrix_MatchDist_Citywide { get { return Convert.ToInt32(ConfigurationManager.AppSettings[CNSTR_CSKEY_ROUTEMATRIX_MATCHDIST_CITYWIDE]); } }

			/// <summary>
			/// 城际拼车接送距离最大容忍度（公里）
			/// </summary>
			public static int RouteMatrix_MatchDist_Intercity { get { return Convert.ToInt32(ConfigurationManager.AppSettings[CNSTR_CSKEY_ROUTEMATRIX_MATCHDIST_INTERCITY]); } }

			/// <summary>
			/// 城际拼车接送距离容忍度（百分比）
			/// </summary>
			public static double RouteMatrix_MatchPerc_Intercity { get { return Convert.ToDouble(ConfigurationManager.AppSettings[CNSTR_CSKEY_ROUTEMATRIX_MATCHPERC_INTERCITY]); } }
			#endregion
		}

		public static class WeChatAPISettings
		{
			#region "Fields"

			#region "WeChat Account"
			const string CNSTR_CSKEY_WECHAT_MP_TOKEN = "WeChat_MP_Token";

			const string CNSTR_CSKEY_WECHAT_MP_ACCOUNT = "WeChat_MP_Account";
			const string CNSTR_CSKEY_WECHAT_MP_PASSWORD = "WeChat_MP_Password";
			const string CNSTR_CSKEY_WECHAT_MP_APPID = "WeChat_MP_AppID";
			const string CNSTR_CSKEY_WECHAT_MP_APPSECRET = "WeChat_MP_AppSecret";
			#endregion

			#region "WeChat API Settings"
			const string CNSTR_CSKEY_WECHAT_MP_APIEXECUTOR_MAXNUM = "WeChat_MP_APIExecutor_MaxNum";
			#endregion

			#region "URI: Login"
			const string CNSTR_CSKEY_WECHAT_MP_LOGINURI = "WeChat_MP_LoginURI";
			const string CNSTR_CSKEY_WECHAT_MP_LOGINUSERAGENT = "WeChat_MP_LoginUserAgent";
			#endregion

			#region "URI: AccessToken (Base)"
			const string CNSTR_CSKEY_WECHAT_MP_GETACCESSTOKENURI = "WeChat_MP_GetAccessTokenURI";
			#endregion

			#region "URI: QRCode"
			const string CNSTR_CSKEY_WECHAT_MP_GENERATEQRCODETICKETURI = "WeChat_MP_GenerateQRCodeTicketURI";
			const string CNSTR_CSKEY_WECHAT_MP_GENERATEQRCODEURI = "WeChat_MP_GenerateQRCodeURI";
			#endregion

			#region "URI: UserInfo"
			const string CNSTR_CSKEY_WECHAT_MP_GETUSERINFOURI = "WeChat_MP_GetUserInfoURI";
			#endregion

			#region "URI: UserList"
			const string CNSTR_CSKEY_WECHAT_MP_GETUSERLISTURI = "WeChat_MP_GetUserListURI";
			#endregion

			#region "URI: SendMessage"
			const string CNSTR_CSKEY_WECHAT_MP_SENDMESSAGEURI = "WeChat_MP_SendMessageURI";
			#endregion

			#region "URI: Access Token (oAuth 2)"
			const string CNSTR_CSKEY_WECHAT_MP_OAUTH2_AUTHURI = "WeChat_MP_OAuth2_AuthURI";
			const string CNSTR_CSKEY_WECHAT_MP_OAUTH2_GETACCESSTOKENURI = "WeChat_MP_OAuth2_GetAccessTokenURI";
			const string CNSTR_CSKEY_WECHAT_MP_OAUTH2_REFACCESSTOKENURI = "WeChat_MP_OAuth2_RefAccessTokenURI";
			const string CNSTR_CSKEY_WECHAT_MP_OAUTH2_GETUSERINFOURI = "WeChat_MP_OAuth2_GetUserInfoURI";
			#endregion

			#endregion

			#region "Properties"

			#region "WeChat Account"
			/// <summary>
			/// 
			/// </summary>
			public static string WeChat_MP_Token { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_WECHAT_MP_TOKEN]; } }

			/// <summary>
			/// 
			/// </summary>
			public static string WeChat_MP_Account { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_WECHAT_MP_ACCOUNT]; } }

			/// <summary>
			/// 
			/// </summary>
			public static string WeChat_MP_Password { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_WECHAT_MP_PASSWORD]; } }

			/// <summary>
			/// 
			/// </summary>
			public static string WeChat_MP_AppID { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_WECHAT_MP_APPID]; } }

			/// <summary>
			/// 
			/// </summary>
			public static string WeChat_MP_AppSecret { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_WECHAT_MP_APPSECRET]; } }
			#endregion

			#region "WeChat API Settings"
			public static int WeChat_MP_APIExecutor_MaxNum { get { return (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[CNSTR_CSKEY_WECHAT_MP_APIEXECUTOR_MAXNUM])) ? (Convert.ToInt32(ConfigurationManager.AppSettings[CNSTR_CSKEY_WECHAT_MP_APIEXECUTOR_MAXNUM])) : (3); } }
			#endregion

			#region "URI: Login"
			/// <summary>
			/// 
			/// </summary>
			public static string WeChat_MP_LoginURI { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_WECHAT_MP_LOGINURI]; } }
			#endregion

			#region "URI: AccessToken (Base)"
			/// <summary>
			/// 
			/// </summary>
			public static string WeChat_MP_GetAccessTokenURI { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_WECHAT_MP_GETACCESSTOKENURI]; } }
			#endregion

			#region "URI: QRCode"
			/// <summary>
			/// 
			/// </summary>
			public static string WeChat_MP_GenerateQRCodeTicketURI { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_WECHAT_MP_GENERATEQRCODETICKETURI]; } }

			/// <summary>
			/// 
			/// </summary>
			public static string WeChat_MP_GenerateQRCodeURI { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_WECHAT_MP_GENERATEQRCODEURI]; } }
			#endregion

			#region "URI: UserInfo"
			public static string WeChat_MP_GetUserInfoURI { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_WECHAT_MP_GETUSERINFOURI]; } }
			#endregion

			#region "URI: UserList"
			public static string WeChat_MP_GetUserListURI { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_WECHAT_MP_GETUSERLISTURI]; } }
			#endregion

			#region "URI: SendMessage"
			public static string WeChat_MP_SendMessageURI { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_WECHAT_MP_SENDMESSAGEURI]; } }
			#endregion

			#region "URI: Access Token (oAuth 2)"
			public static string WeChat_MP_OAuth2_AuthURI { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_WECHAT_MP_OAUTH2_AUTHURI]; } }

			public static string WeChat_MP_OAuth2_GetAccessTokenURI { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_WECHAT_MP_OAUTH2_GETACCESSTOKENURI]; } }

			public static string WeChat_MP_OAuth2_RefAccessTokenURI { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_WECHAT_MP_OAUTH2_REFACCESSTOKENURI]; } }

			public static string WeChat_MP_OAuth2_GetUserInfoURI { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_WECHAT_MP_OAUTH2_GETUSERINFOURI]; } }
			#endregion

			#endregion
		}

		public static class APIServiceSettings
		{
			#region "Fields"
			const string CNSTR_CSKEY_REPOSITORY_SERVICE_ENTRY = "Repository_Service_Entry";
			const string CNSTR_CSKEY_ROUTEMATRIX_SERVICE_ENTRY = "RouteMatrix_Service_Entry";
			const string CNSTR_CSKEY_INSURANCE_SERVICE_ENTRY = "Insurance_Service_Entry";
			const string CNSTR_CSKEY_WECHAT_SERVICE_ENTRY = "WeChat_Service_Entry";
			const string CNSTR_CSKEY_SMS_SERVICE_ENTRY = "SMS_Service_Entry";
			const string CNSTR_CSKEY_LOG_SERVICE_ENTRY = "Log_Service_Entry";
			#endregion

			#region "Properties"
			public static string Repository_Service_Entry { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_REPOSITORY_SERVICE_ENTRY]; } }

			public static string RouteMatrix_Service_Entry { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_ROUTEMATRIX_SERVICE_ENTRY]; } }

			public static string Insurance_Service_Entry { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_INSURANCE_SERVICE_ENTRY]; } }

			public static string WeChat_Service_Entry { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_WECHAT_SERVICE_ENTRY]; } }

			public static string SMS_Service_Entry { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_SMS_SERVICE_ENTRY]; } }

			public static string Log_Service_Entry { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_LOG_SERVICE_ENTRY]; } }
			#endregion
		}

		public static class PaymentSettings
		{
			public static class AlipaySettings
			{
				#region "Fields"
				const string CNSTR_CSKEY_PAYMENT_ALIPAY_GATEWAY_URI = "Payment_Alipay_Gateway_URI";

				const string CNSTR_CSKEY_PAYMENT_ALIPAY_SELLER_ACCOUNT = "Payment_Alipay_Seller_Account";
				#endregion

				#region "Properties"
				public static string Payment_Alipay_Gateway_URI { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_PAYMENT_ALIPAY_GATEWAY_URI]; } }

				public static string Payment_Alipay_Seller_Account { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_PAYMENT_ALIPAY_SELLER_ACCOUNT]; } }
				#endregion
			}
		}

		public static class InsuranceSettings
		{
			#region "Fields"
			const string CNSTR_CSKEY_INSURANCE_AVAILABLE_CITIES = "Insurance_Available_Cities";

			const string CNSTR_CSKEY_INSURANCE_SETTINGS_SYSTEMNAME = "Insurance_Settings_SystemName";
			const string CNSTR_CSKEY_INSURANCE_SETTINGS_OCCUPATIONCODE = "Insurance_Settings_OccupationCode";
			const string CNSTR_CSKEY_INSURANCE_SETTINGS_PRESENTCODE = "Insurance_Settings_PresentCode";
			const string CNSTR_CSKEY_INSURANCE_SETTINGS_TSRCODE = "Insurance_Settings_TSRCode";
			const string CNSTR_CSKEY_INSURANCE_SETTINGS_SMSENABLED = "Insurance_Settings_SMSEnabled";

			const string CNSTR_CSKEY_INSURANCE_SETTINGS_SERVICE_URI_METLIFE_UAT = "Insurance_Settings_Service_URI_Metlife_UAT";
			const string CNSTR_CSKEY_INSURANCE_SETTINGS_SERVICE_URI_METLIFE_PRD = "Insurance_Settings_Service_URI_Metlife_PRD";
			#endregion

			#region "Properties"
			public static string Insurance_Available_Cities { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_INSURANCE_AVAILABLE_CITIES]; } }

			public static string Insurance_Settings_SystemName { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_INSURANCE_SETTINGS_SYSTEMNAME]; } }

			public static string Insurance_Settings_OccupationCode { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_INSURANCE_SETTINGS_OCCUPATIONCODE]; } }

			public static string Insurance_Settings_PresentCode { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_INSURANCE_SETTINGS_PRESENTCODE]; } }

			public static string Insurance_Settings_TSRCode { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_INSURANCE_SETTINGS_TSRCODE]; } }

			public static bool Insurance_Settings_SMSEnabled { get { return Convert.ToBoolean(ConfigurationManager.AppSettings[CNSTR_CSKEY_INSURANCE_SETTINGS_SMSENABLED]); } }

			public static string Insurance_Settings_Service_URI_Metlife_UAT { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_INSURANCE_SETTINGS_SERVICE_URI_METLIFE_UAT]; } }

			public static string Insurance_Settings_Service_URI_Metlife_PRD { get { return ConfigurationManager.AppSettings[CNSTR_CSKEY_INSURANCE_SETTINGS_SERVICE_URI_METLIFE_PRD]; } }
			#endregion
		}

		public static class LoggingSettings
		{
			#region "Fields"
			const string CNSTR_CSKEY_LOGGING_ENABLEEXTEND_APP = "Logging_EnableExtend_App";
			const string CNSTR_CSKEY_LOGGING_ENABLEEXTEND_WX = "Logging_EnableExtend_WX";
			#endregion

			#region "Properties"
			public static bool EnableExtend_App { get { return Convert.ToBoolean(ConfigurationManager.AppSettings[CNSTR_CSKEY_LOGGING_ENABLEEXTEND_APP]); } }

			public static bool EnableExtend_WX { get { return Convert.ToBoolean(ConfigurationManager.AppSettings[CNSTR_CSKEY_LOGGING_ENABLEEXTEND_WX]); } }
			#endregion
		}
	}
}
