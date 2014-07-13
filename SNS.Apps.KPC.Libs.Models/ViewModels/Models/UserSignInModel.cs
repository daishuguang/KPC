using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime;
using System.Runtime.Serialization;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public class UserSignInModel
	{
		[Required(ErrorMessage = "亲，手机号是必填的哦！")]
		[MaxLength(11, ErrorMessage = "亲，不是有效的手机号哦！")]
		[DataMember]
		public string Mobile { get; set; }

		[Required(ErrorMessage = "亲，您忘了输密码了吧！")]
		[MaxLength(12, ErrorMessage = "亲，有效的密码是 6 ~ 12 位数字或字符哦！")]
		[MinLength(6, ErrorMessage = "亲，有效的密码是 6 ~ 12 位数字或字符哦！")]
		[DataMember]
		public string Password { get; set; }

		[Required(ErrorMessage = "亲，您忘了输验证码了吧！")]
		[MaxLength(4, ErrorMessage = "亲，有效的验证码是 4 位数字或字符哦！")]
		[MinLength(4, ErrorMessage = "亲，有效的验证码是 4 位数字或字符哦！")]
		[DataMember]
		public string ValidationCode { get; set; }
	}
}
