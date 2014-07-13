using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public class InsuranceSubmitModel
	{
		#region "Properties"
		[Required(ErrorMessage = "请输入您真实的姓名！")]
		[Display(Name = "姓名")]
		[DataMember]
		public string Name { get; set; }

		[Required(ErrorMessage = "请选择您的性别！")]
		[Display(Name = "性别")]
		[DataMember]
		public Nullable<bool> Gender { get; set; }

		[Display(Name = "身份证号")]
		[DataMember]
		public string IdentityNo { get; set; }

		[Required(ErrorMessage = "请选择您真实的生日！")]
		[Display(Name = "生日")]
		[DataMember]
		public Nullable<DateTime> Birthday { get; set; }

		[Display(Name = "手机号")]
		[DataMember]
		public string Mobile { get; set; }

		[Display(Name = "省")]
		[DataMember]
		public string Province { get; set; }

		[Required(ErrorMessage = "请输入您当前居住的城市！")]
		[Display(Name = "市")]
		[DataMember]
		public string City { get; set; }

		[Display(Name = "县/区")]
		[DataMember]
		public string District { get; set; }

		[Display(Name = "地址")]
		[DataMember]
		public string Location { get; set; }
		#endregion

		#region "Methods"
		public override string ToString()
		{
			return string.Format("Name: {0}, Birthday: {1}, Mobile: {2}, Province: {3}, City: {4}, District: {5}, Location: {6}", this.Name, (this.Birthday != null && this.Birthday.HasValue) ? (this.Birthday.Value.ToString("yyyy-MM-dd")) : (string.Empty), this.Mobile, this.Province, this.City, this.District, this.Location);
		}
		#endregion
	}
}
