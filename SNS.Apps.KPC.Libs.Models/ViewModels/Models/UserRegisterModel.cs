using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime;
using System.Runtime.Serialization;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public class UserRegisterModel
	{
		#region "Properties"
		[DataMember]
		public Guid UserGUID { get; set; }

		[Required(ErrorMessage = "方便的话，选一个独一无二的“昵称”吧！")]
		[Display(Name = "昵称")]
		[DataMember]
		public string NickName { get; set; }

		[DataMember]
		public string PortraitsUrl { get; set; }

		[DataMember]
		public string PortraitsThumbUrl { get; set; }

		[Display(Name = "手机号")]
		[Required(ErrorMessage = "请输入“手机号”，方便您和其他拼友联系！")]
		[DataType(DataType.PhoneNumber)]
		[MaxLength(11, ErrorMessage = "手机号太长了吧，亲！")]
		[RegularExpression(@"^\d{11}$", ErrorMessage = "手机号输入有误吧，亲！")]
		[DataMember]
		public string Mobile { get; set; }

		[Display(Name = "QQ号")]
		[DataMember]
		public string QQ { get; set; }

		[Display(Name = "微信号")]
		[DataMember]
		public string WeChatID { get; set; }

		[Display(Name = "角色")]
		[Required(ErrorMessage = "请选择您的“角色”")]
		[DataMember]
		public UserRole UserRole { get; set; }

		[Display(Name = "车牌号")]
		[MaxLength(10, ErrorMessage = "貌似没有这么长的车牌号吧")]
		[DataMember]
		public string LicencePlateNumber { get; set; }

		[DataMember]
		public string Password { get; set; }

		#region "推荐码"
		[DataMember]
		public Nullable<long> RefID { get; set; }

		[Display(Name = "推荐码")]
		[RegularExpression(@"^\d{6}$", ErrorMessage = "您输入的推荐码不正确！")]
		[DataMember]
		public string RefCode { get; set; }  
		#endregion
		#endregion

		#region "Properties"
		[DataMember]
		public bool IsExternal { get; set; }

		//[DataMember]
		//public bool IsExtended { get; set; } 
		#endregion
	}
}
