using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Models.DataStores;
using SNS.Apps.KPC.Libs.IServices;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;

namespace SNS.Apps.TaskConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			//var clientWechat = SNS.Apps.KPC.Libs.Utils.WCFServiceClientUtility.CreateServiceChanel<SNS.Apps.KPC.Libs.IServices.IWeChatService>("wechatService");
			//var clientSMS = SNS.Apps.KPC.Libs.Utils.WCFServiceClientUtility.CreateServiceChanel<SNS.Apps.KPC.Libs.IServices.ISMSService>("smsService");

			try
			{
				//Console.WriteLine(clientSMS.GetBlance());

				//ExternalDataImportor.ImportPhones();

				//var code = QRCodeGenerator.Generate(34);

				//Console.WriteLine(code);


				//var webRequest = (HttpWebRequest)System.Net.WebRequest.Create("https://api.weixin.qq.com/sns/oauth2/access_token?appid=wxd946f6039772215d&secret=296c3e599e7574c5694c639d78d3ea33&code=01f0112003e1c701f7abcc415677ecaf&grant_type=authorization_code");

				//webRequest.Method = "GET";
				//webRequest.UserAgent = "";

				//var response = webRequest.GetResponse();


				//SMSMessenger.GetBlance();

				//ExternalDataImportor.GenerateCode();
				//ExternalDataImportor.ParseContent();
				//ExternalDataImportor.Import_Users();
				//ExternalDataImportor.Import_Phones();
				//ExternalDataImportor.Import_Routes();

				//PortraitsConvertor.Convert();
				//UserProfileValidator.Validate();

                //var a = "abc";
                //var b = "def";
                //var c = "abc";

                //Console.WriteLine(a.GetHashCode());
                //Console.WriteLine(b.GetHashCode());
                //Console.WriteLine(c.GetHashCode());


				//var msg = new KPC.Libs.Models.WeChat.WeChat_MsgText();
				//var msg = KPC.Libs.Models.WeChat.WeChat_MsgBase<SNS.Apps.KPC.Libs.Models.WeChat.WeChat_MsgText_Content>.CreateInstance();

				//msg.ToUser = "111";
				//msg.Content = new KPC.Libs.Models.WeChat.WeChat_MsgText_Content();
				//msg.Content.Text = "abc";

				//Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(msg));

				//clientSMS.SendSMS(new KPC.Libs.Models.SMS.SMSMessageSendRequest { Channel = 0, Mobiles = new string[] { "13917952577" }, Content = "欢迎使用快拼车，您的验证码是“1111”。【快拼车】" });
				//clientSMS.SendSMS(new KPC.Libs.Models.SMS.SMSMessageSendRequest { Channel = 1, Mobiles = new string[] { "13917952577" }, Content = "您的验证码是：2222。请不要把验证码泄露给其他人。" });
				//clientSMS.SendSMS(new KPC.Libs.Models.SMS.SMSMessageSendRequest { Channel = 2, Mobiles = new string[] { "13524692229" }, Content = "您的验证码是：cccc。请不要把验证码泄露给其他人。" });

				//System.Threading.Thread th1 = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(M1));
				//System.Threading.Thread th2 = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(M2));
				//System.Threading.Thread th3 = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(M3));

				//th1.Start(clientSMS);
				//th2.Start(clientSMS);
				//th3.Start(clientSMS);

				//UserProfileValidator.Validate();

				//MigrateFolio();
                MetLisfeService.doRequestRequest request = new MetLisfeService.doRequestRequest();

				//Console.WriteLine(System.Text.Encoding.Default.GetString(Convert.FromBase64String("dGVsZWNvbWFkbWluMzExNDczNDEA")));

				//DBLogger.Instance.Info("abc");
				DBLogger.Instance.Info("1");
				DBLogger.Instance.Debug("2");
				DBLogger.Instance.Error("3");
				DBLogger.Instance.Fatal("4");
				DBLogger.Instance.Warn("5");
				DBLogger.Instance.Log(KPC.Libs.Utils.Logger.Base.LogLevel.Debug, "6");

				Console.WriteLine("DONE!");
				Console.ReadKey();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			finally
			{
				//SNS.Apps.KPC.Libs.Utils.WCFServiceClientUtility.CloseServiceChannel(clientWechat);
				//SNS.Apps.KPC.Libs.Utils.WCFServiceClientUtility.CloseServiceChannel(clientSMS);
			}
		}

		static void MigrateFolio()
		{
			var dicDatas = new Dictionary<long, string>();
			var conn = new SqlConnection("data source=.;initial catalog=SNS.Apps.KPC.V2;integrated security=True;");

			try
			{
				var cmd = new SqlCommand("select * from dbo.tbl_User_order order by ID", conn);
				var ada = new SqlDataAdapter(cmd);
				var ds = new DataSet();

				ada.Fill(ds);

				if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
				{
					var count = 0;
					var dt = DateTime.MinValue;
					var rand = new Random();

					foreach (DataRow row in ds.Tables[0].Rows)
					{
						var date = Convert.ToDateTime(row["CreateDate"]);

						if (date.Date.CompareTo(dt.Date) != 0)
						{
							dt = date;
							count = 1;
						}
						else
						{
							count++;
						}
						
						var folio = string.Format("{0}{1}", date.ToString("yyyyMMdd"), rand.Next(100000, 999999));

						dicDatas.Add(Convert.ToInt64(row["ID"]), folio);
					}

					if (dicDatas != null && dicDatas.Count > 0)
					{
						var sb = new StringBuilder();

						foreach(KeyValuePair<long, string> kv in dicDatas)
						{
							sb.AppendLine(string.Format("update dbo.tbl_user_order set Folio = '{0}' where ID = {1}", kv.Value, kv.Key));
						}

						conn.Open();

						cmd = new SqlCommand(sb.ToString(), conn);
						cmd.ExecuteNonQuery();
					}
				}
			}
			finally
			{
				if (conn != null && conn.State != ConnectionState.Closed)
				{
					conn.Close();
				}
			}
		}

		static void CleanMemCache()
		{
			SNS.Apps.KPC.Libs.IServices.IConsoleService client = null;

			try
			{
				client = SNS.Apps.KPC.Libs.Utils.WCFServiceClientUtility.CreateServiceChanel<SNS.Apps.KPC.Libs.IServices.IConsoleService>("consoleService");

				client.CleanMemCache();

				Console.WriteLine("Clean All MemCache Data. Done!");
			}
			finally
			{
				SNS.Apps.KPC.Libs.Utils.WCFServiceClientUtility.CloseServiceChannel(client);
			}
		}
	}
}
