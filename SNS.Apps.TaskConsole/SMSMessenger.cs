using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.TaskConsole
{
	class SMSMessenger
	{
		public static void GetBlance()
		{
			var client = SNS.Apps.KPC.Libs.Utils.WCFServiceClientUtility.CreateServiceChanel<SNS.Apps.KPC.Libs.IServices.ISMSService>("smsService");

			try
			{
				int num = client.GetBlance();

				Console.WriteLine(num);
			}
			finally
			{
				SNS.Apps.KPC.Libs.Utils.WCFServiceClientUtility.CloseServiceChannel(client);
			}
		}

		public static void SendSMSMessage()
		{
			var client = SNS.Apps.KPC.Libs.Utils.WCFServiceClientUtility.CreateServiceChanel<SNS.Apps.KPC.Libs.IServices.ISMSService>("smsService");

			try
			{
				var request = new SNS.Apps.KPC.Libs.Models.SMS.SMSMessageSendRequest { Mobiles = new string[] { "18501708084", "18817889736", "18616543433", "13916069441", "15000263522", "13795406924" }, Content = "Hi" };
				//var result = client.SendSMS(request);

				//Console.WriteLine(result.Success);
			}
			finally
			{
				SNS.Apps.KPC.Libs.Utils.WCFServiceClientUtility.CloseServiceChannel(client);
			}
		}

		//public static void RecieveSMSMessages()
		//{
		//	var client = SNS.Apps.KPC.Libs.Utils.WCFServiceClientUtility.CreateServiceChanel<SNS.Apps.KPC.Libs.IServices.ISMSService>("smsService");

		//	try
		//	{
		//		var result = client.ReceiveSMS();

		//		Console.WriteLine(result.Success);
		//	}
		//	finally
		//	{
		//		SNS.Apps.KPC.Libs.Utils.WCFServiceClientUtility.CloseServiceChannel(client);
		//	}
		//}
	}
}
