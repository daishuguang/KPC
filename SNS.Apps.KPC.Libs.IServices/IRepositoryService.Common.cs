using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

using SNS.Apps.KPC.Libs.Models;

namespace SNS.Apps.KPC.Libs.IServices
{
	[ServiceContract(Namespace = "api.kuaipinche.com")]
	public partial interface IRepositoryService
	{
		#region "Methods: PhoneVerify"
		[OperationContract]
		bool VerifyMobile(string mobile, string verifyCode);

		[OperationContract]
		string SetVerifyCode(string mobile, int channel); 
		#endregion
	}
}
