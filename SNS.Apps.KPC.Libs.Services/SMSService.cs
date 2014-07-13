using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Security.Cryptography;
using System.Security.Policy;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.IServices;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Models.SMS;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;

namespace SNS.Apps.KPC.Libs.Services
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class SMSService : ISMSService
    {
        #region "Fields"
        static Lazy<IEnumerable<SMSServiceChannel>> _channels = new Lazy<IEnumerable<SMSServiceChannel>>(() =>
        {
            var lst = new List<SMSServiceChannel>();
            var settings = ConfigurationManager.GetSection("smsServiceSettings") as SMSServiceConfiguration;

            if (settings != null)
            {
                foreach (SMSServiceChannel channel in settings.Channels)
                {
                    lst.Add(channel);
                }
            }

            return lst.ToArray();
        });
        #endregion

        #region "Properties"
        public static IEnumerable<SMSServiceChannel> Channels { get { return _channels.Value; } }
        #endregion

		#region "Public Methods"

		#region "Methods: GetBlance"
		public int GetBlance(int channel = 0)
		{
			try
			{
				var webRequest = CreateSMSServiceRequest(SMSAction.GetBalance, new SMSMessageSendRequest { Channel = channel });


				using (var resStream = new StreamReader(webRequest.GetResponse().GetResponseStream()))
				{
					var content = resStream.ReadToEnd();

					#region "Parse Response Result"
					var xmlDoc = new XmlDocument();

					xmlDoc.LoadXml(content);

					content = xmlDoc.DocumentElement.InnerText;
					#endregion

					return (!string.IsNullOrEmpty(content)) ? (int.Parse(content)) : (-1);
				}
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters:\r\nException: {2}",
					"SMSService",
					"int GetBlance()",
					ex.ToString()
				);
			}

			return -1;
		} 
		#endregion

		#region "Methods: SendSMS"
		public SMSMessageSendResult SendSMS_VerifyCode(SMSMessageSendRequest requestInfo)
		{
			return SendSMS(requestInfo);
		} 

		public SMSMessageSendResult SendSMS_Notification(SMSMessageSendRequest requestInfo)
		{
			return SendSMS(new SMSMessageSendRequest { Channel = 0, Mobiles = requestInfo.Mobiles, Content = requestInfo.Content });
		}

		SMSMessageSendResult SendSMS(SMSMessageSendRequest requestInfo)
		{
			try
			{
				var ret = default(SMSMessageSendResult);

				if (ConfigStore.CommonSettings.Debug_Mode)
				{
					ret = new SMSMessageSendResult { Success = true };
				}
				else
				{
					var webRequest = CreateSMSServiceRequest(SMSAction.SendSMS, requestInfo);

					using (var resStream = new StreamReader(webRequest.GetResponse().GetResponseStream()))
					{
						var content = resStream.ReadToEnd();

						#region "Parse Response Content"
						switch (requestInfo.Channel.Value)
						{
							case 0:
								{
									#region "Result: Channel 0"
									var xmlDoc = new XmlDocument();

									xmlDoc.LoadXml(content);

									content = xmlDoc.DocumentElement.InnerText;

									var num = 0;

									if (int.TryParse(content, out num))
									{
										ret = new SMSMessageSendResult { Success = true };
									}
									else
									{
										ret = new SMSMessageSendResult { Success = false, ErrorMsg = content };
									}
									#endregion
								}
								break;
							case 1:
								{
									#region "Result: Channel 1"
									var xmlDoc = new XmlDocument();

									xmlDoc.LoadXml(content);

									content = xmlDoc.LastChild.FirstChild.InnerText;

									var num = 0;

									if (int.TryParse(content, out num))
									{
										if (num == 2)
										{
											ret = new SMSMessageSendResult { Success = true };
										}
										else
										{
											ret = new SMSMessageSendResult { Success = false, ErrorMsg = content };
										}
									}
									else
									{
										ret = new SMSMessageSendResult { Success = false, ErrorMsg = content };
									}
									#endregion
								}
								break;
							case 2:
								{
									#region "Result: Channel 2"
									var num = default(Int64);

									if (Int64.TryParse(content, out num))
									{
										if (num < 0 || num == 1)
										{
											ret = new SMSMessageSendResult { Success = false, ErrorMsg = content };
										}
										else
										{
											ret = new SMSMessageSendResult { Success = true };
										}
									}
									else
									{
										ret = new SMSMessageSendResult { Success = true };
									}
									#endregion
								}
								break;
							case 3:
								{
									#region "Result: Channel 3"
									if (content == "100")
									{
										ret = new SMSMessageSendResult { Success = true };
									}
									else
									{
										ret = new SMSMessageSendResult { Success = false, ErrorMsg = content };
									}
									#endregion
								}
								break;
							default:
								break;
						}
						#endregion

						if (ConfigStore.CommonSettings.Trace_Mode)
						{
							DBLogger.Instance.InfoFormat(
								"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\n: Result: [{3}]",
								"SMSService",
								"SMSMessageSendResult SendSMS(SMSMessageSendRequest requestInfo)",
								requestInfo.ToString(),
								ret.Success.ToString() + (ret.Success ? string.Empty : (", ErrorMsg: " + ret.ErrorMsg))
							);
						}
					}
				}

				return ret;
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: [{3}]",
					"SMSService",
					"SMSMessageSendResult SendSMS(SMSMessageSendRequest requestInfo)",
					requestInfo.ToString(),
					ex.ToString()
				);

				return new SMSMessageSendResult { Success = false, ErrorMsg = ex.Message };
			}
		}
		#endregion

		//public SMSMessageReceiveResult ReceiveSMS()
		//{
		//	try
		//	{
		//		var webRequest = CreateSMSServiceRequest(SMSAction.RecvSmsEx, null);
		//		var content = string.Format("username={0}&password={1}", ConfigStore.SMSServiceSettings.SMS_Service_User, ConfigStore.SMSServiceSettings.SMS_Service_Password);
		//		var data = System.Text.Encoding.UTF8.GetBytes(content);

		//		webRequest.ContentLength = data.Length;

		//		using (var reqStream = webRequest.GetRequestStream())
		//		{
		//			reqStream.Write(data, 0, data.Length);
		//		}

		//		using (var resStream = new StreamReader(webRequest.GetResponse().GetResponseStream()))
		//		{
		//			var ret = new SMSMessageReceiveResult(resStream.ReadToEnd());

		//			if (ConfigStore.CommonSettings.Trace_Mode)
		//			{
		//				DBLogger.Instance.ErrorFormat(
		//					"Service: [{0}], Method: [{1}]\r\nParameters:\r\n: Result: [{2}]",
		//					"SMSService",
		//					"SMSMessageReceiveResult ReceiveSMS()",
		//					ret.Success.ToString() + (ret.Success ? (", Count: " + ret.Count) : (", ErrorMsg: " + ret.ErrorMsg))
		//				);
		//			}

		//			return ret;
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		DBLogger.Instance.ErrorFormat(
		//			"Service: [{0}], Method: [{1}]\r\nParameters:\r\nException: {2}",
		//			"SMSService",
		//			"SMSMessageReceiveResult ReceiveSMS()",
		//			ex.ToString()
		//		);

		//		return new SMSMessageReceiveResult { };
		//	}
		//} 

		#endregion

        #region "Private Methods"
        HttpWebRequest CreateSMSServiceRequest(SMSAction action, SMSMessageSendRequest requestInfo)
        {
            var smsChannel = Channels.FirstOrDefault(p => (p.Channel == ((requestInfo == null || !requestInfo.Channel.HasValue) ? (0) : (requestInfo.Channel.Value))) && p.IsEnabled);

            if (smsChannel == null)
            {
                throw new Exception(string.Format("未能找到标识为‘{0}’的短信通道，请查验相关配置文件！", requestInfo.Channel ?? 0));
            }

			var webRequest = this.GetType().GetMethod(string.Format("CreateSMSServiceRequest_Channel_{0}", requestInfo.Channel), BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public);

			return (webRequest != null) ? ((HttpWebRequest)webRequest.Invoke(this, new object[] { action, smsChannel, requestInfo })) : (null);
        }

        HttpWebRequest CreateSMSServiceRequest_Channel_0(SMSAction action, SMSServiceChannel channel, SMSMessageSendRequest requestInfo)
        {
            var url = string.Format("{0}/{1}", channel.ServiceEntry.TrimEnd('/'), action.ToString());
            var webRequest = (HttpWebRequest)WebRequest.Create(url);

            webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.UserAgent = ConfigStore.CommonSettings.WebRequest_UserAgent;
            webRequest.Referer = url;

            switch (action)
            {
                case SMSAction.SendSMS:
                    {
						var content = string.Format("username={0}&password={1}&mobile={2}&smscontent={3}", channel.UserName, channel.Password, requestInfo.SerializeMobiles(), requestInfo.Content);
                        var data = System.Text.Encoding.UTF8.GetBytes(content);

                        webRequest.ContentLength = data.Length;

                        using (var reqStream = webRequest.GetRequestStream())
                        {
                            reqStream.Write(data, 0, data.Length);
                        }
                    }
                    break;
                case SMSAction.GetBalance:
                    {
                        var content = string.Format("username={0}&password={1}", channel.UserName, channel.Password);
                        var data = System.Text.Encoding.GetEncoding("GB2312").GetBytes(content);

                        webRequest.ContentLength = data.Length;

                        using (var reqStream = webRequest.GetRequestStream())
                        {
                            reqStream.Write(data, 0, data.Length);
                        }
                    }
                    break;
            }

            return webRequest;
        }

		HttpWebRequest CreateSMSServiceRequest_Channel_1(SMSAction action, SMSServiceChannel channel, SMSMessageSendRequest requestInfo)
		{
			var url = channel.ServiceEntry;
			var webRequest = (HttpWebRequest)WebRequest.Create(url);

			webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
			webRequest.Method = "POST";
			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.UserAgent = ConfigStore.CommonSettings.WebRequest_UserAgent;
			webRequest.Referer = url;

			switch (action)
			{
				case SMSAction.SendSMS:
					{
						var encoding = System.Text.Encoding.GetEncoding("GB2312");
						var content = string.Format("account={0}&password={1}&mobile={2}&content={3}", channel.UserName, Libs.Utils.CommonUtility.GetMd5Str32(channel.Password), requestInfo.SerializeMobiles(), System.Web.HttpUtility.UrlEncode(requestInfo.Content, encoding));
						var data = encoding.GetBytes(content);

						webRequest.ContentLength = data.Length;

						using (var reqStream = webRequest.GetRequestStream())
						{
							reqStream.Write(data, 0, data.Length);
						}
					}
					break;
				case SMSAction.GetBalance:
					{
						//var url = string.Format("{0}/z_balance.aspx?sn={1}&pwd={2}", channel.ServiceEntry.TrimEnd('/'), channel.UserName, channel.Password);

						//webRequest = (HttpWebRequest)WebRequest.Create(url);
						//webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
						//webRequest.Method = "POST";
						//webRequest.ContentType = "application/x-www-form-urlencoded";
						//webRequest.UserAgent = ConfigStore.CommonSettings.WebRequest_UserAgent;
						//webRequest.Referer = url;
					}
					break;
			}

			return webRequest;
		}

		HttpWebRequest CreateSMSServiceRequest_Channel_2(SMSAction action, SMSServiceChannel channel, SMSMessageSendRequest requestInfo)
		{
			var webRequest = default(HttpWebRequest);

			switch (action)
			{
				case SMSAction.SendSMS:
					{
						var url = string.Format("{0}/z_mdsmssend.aspx", channel.ServiceEntry.TrimEnd('/'));
						var content = string.Format("sn={0}&pwd={1}&mobile={2}&content={3}&ext=&rrid=&stime=", channel.UserName, Libs.Utils.CommonUtility.GetMd5Str32(string.Format("{0}{1}", channel.UserName, channel.Password)).ToUpper(), requestInfo.SerializeMobiles(), System.Web.HttpUtility.UrlEncode(requestInfo.Content, System.Text.Encoding.GetEncoding("GB2312")));
						var data = System.Text.Encoding.GetEncoding("GB2312").GetBytes(content);

						webRequest = (HttpWebRequest)WebRequest.Create(url);
						webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
						webRequest.Method = "POST";
						webRequest.ContentType = "application/x-www-form-urlencoded";
						webRequest.UserAgent = ConfigStore.CommonSettings.WebRequest_UserAgent;
						webRequest.Referer = url;
						webRequest.ContentLength = data.Length;

						using (var reqStream = webRequest.GetRequestStream())
						{
							reqStream.Write(data, 0, data.Length);
						}
					}
					break;
				case SMSAction.GetBalance:
					{
						//var url = string.Format("{0}/z_balance.aspx?sn={1}&pwd={2}", channel.ServiceEntry.TrimEnd('/'), channel.UserName, channel.Password);

						//webRequest = (HttpWebRequest)WebRequest.Create(url);
						//webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
						//webRequest.Method = "POST";
						//webRequest.ContentType = "application/x-www-form-urlencoded";
						//webRequest.UserAgent = ConfigStore.CommonSettings.WebRequest_UserAgent;
						//webRequest.Referer = url;
					}
					break;
			}

			return webRequest;
		}

		[Obsolete]
        HttpWebRequest CreateSMSServiceRequest_Channel_ooxx(SMSAction action, SMSServiceChannel channel, SMSMessageSendRequest requestInfo)
        {
            string url = string.Format("{0}", channel.ServiceEntry);
            string phonenum = requestInfo.Mobiles.FirstOrDefault();
            string content = string.Format("uid={0}&pwd={1}&mobile={2}&content={3}", channel.UserName, MD5(channel.Password), phonenum, requestInfo.Content);
            byte[] bData = Encoding.GetEncoding("GBK").GetBytes(content);
            System.Net.HttpWebRequest hwRequest;

            string strResult = string.Empty;

            hwRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            hwRequest.Timeout = 5000;
            hwRequest.Method = "POST";
            hwRequest.ContentType = "application/x-www-form-urlencoded";
            hwRequest.ContentLength = bData.Length;

            System.IO.Stream smWrite = hwRequest.GetRequestStream();
            smWrite.Write(bData, 0, bData.Length);
            smWrite.Close();

            return hwRequest;
        }

        private string MD5(string pwd)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.Default.GetBytes(pwd);
            byte[] md5data = md5.ComputeHash(data);
            md5.Clear();
            string str = "";
            for (int i = 0; i < md5data.Length; i++)
            {
                str += md5data[i].ToString("x").PadLeft(2, '0');

            }
            return str;
        }
        #endregion
    }
}
