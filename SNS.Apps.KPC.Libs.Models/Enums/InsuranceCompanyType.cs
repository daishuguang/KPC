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
	public enum InsuranceCompanyType
	{
		[EnumMember]
		NoSet = 0,

		[EnumMember]
		MetLife = 1
	}
}
