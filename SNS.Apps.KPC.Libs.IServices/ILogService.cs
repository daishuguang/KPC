using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Models;

namespace SNS.Apps.KPC.Libs.IServices
{
	[ServiceContract(Namespace = "api.kuaipinche.com")]
	public interface ILogService
	{
		#region "Methods: WXMessage"
		[OperationContract(Name = "GetLatestWXMsgByUserID")]
		WXMessage GetLatestWXMsg(long userID, bool inForce = false);

		[OperationContract(Name = "GetLatestWXMsgByOpenID")]
		WXMessage GetLatestWXMsg(string openID, bool inForce = false);

		[OperationContract]
		void ArchiveWXMsg(WXMessage msg);
		#endregion
	}
}
