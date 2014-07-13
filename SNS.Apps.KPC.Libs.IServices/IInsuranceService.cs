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
	public interface IInsuranceService
	{
		#region "Methods: Create"
		[OperationContract]
		InsuranceSubmitResult Create(InsuranceSubmitRequest requestInfo);
		#endregion
	}
}
