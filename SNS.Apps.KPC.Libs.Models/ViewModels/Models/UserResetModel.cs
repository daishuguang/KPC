using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime;
using System.Runtime.Serialization;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public class UserResetModel
	{
		#region "Properties"
		[DataMember]
		public string Mobile { get; set; }

		[DataMember]
		public string Password { get; set; }

		[DataMember]
		public string ConfirmPassword { get; set; }

		[DataMember]
		public string VerifyCode { get; set; }
		#endregion
	}
}
