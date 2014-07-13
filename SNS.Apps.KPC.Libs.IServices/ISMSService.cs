using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Models.SMS;

namespace SNS.Apps.KPC.Libs.IServices
{
	[ServiceContract(Namespace = "api.kuaipinche.com")]
	public interface ISMSService
	{
		[OperationContract]
		int GetBlance(int channel = 0);

		[OperationContract]
		SMSMessageSendResult SendSMS_VerifyCode(SMSMessageSendRequest requestInfo);

		[OperationContract]
		SMSMessageSendResult SendSMS_Notification(SMSMessageSendRequest requestInfo);

		//[OperationContract]
		//SMSMessageReceiveResult ReceiveSMS();
	}
}
