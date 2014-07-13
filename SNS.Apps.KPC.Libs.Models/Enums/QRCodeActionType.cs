using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public enum QRCodeActionType
	{
		[EnumMember]
		QR_SCENE = 0,

		[EnumMember]
		QR_LIMIT_SCENE = 1
	}
}
