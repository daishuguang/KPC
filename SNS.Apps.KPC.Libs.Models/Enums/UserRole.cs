using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public enum UserRole
	{
		[Description("乘客")]
		[EnumMember]
		Passenger = 0,

		[Description("车主")]
		[EnumMember]
		Driver = 1
	}
}
