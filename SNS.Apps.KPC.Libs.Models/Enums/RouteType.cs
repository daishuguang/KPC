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
	public enum RouteType
	{
		//[Description("同城拼车（短途）")]
		//[EnumMember]
		//Citywide_ShortDistance = 0,

		//[Description("同城拼车（长途）")]
		//[EnumMember]
		//Citywide_LongDistance = 2,

		//[Description("城际拼车（长途）")]
		//[EnumMember]
		//Intercity_LongDistance = 1,

		//[Description("城际拼车（短途）")]
		//[EnumMember]
		//Intercity_ShortDistance = 3,

		//[Description("不限（适用于搜索未输入终点情况）")]
		//[EnumMember]
		//All = -1

		[Description("同城（全部）")]
		[EnumMember]
		Citywide_All = 0x1F,

		[Description("同城（工作日）")]
		[EnumMember]
		Citywide_Workday = 0x11,

		[Description("同城（周末）")]
		[EnumMember]
		Citywide_Weekend = 0x12,

		[Description("同城（临时）")]
		[EnumMember]
		Citywide_Temp = 0x14,

		[Description("同城（每天）")]
		[EnumMember]
		Citywide_EveryDay = 0x18,

		[Description("城际（全部）")]
		[EnumMember]
		Intercity_All = 0x2F,

		[Description("城际（工作日）")]
		[EnumMember]
		Intercity_Workday = 0x21,

		[Description("城际（周末）")]
		[EnumMember]
		Intercity_Weekend = 0x22,

		[Description("城际（临时）")]
		[EnumMember]
		Intercity_Temp = 0x24,

		[Description("城际（每天）")]
		[EnumMember]
		Intercity_EveryDay = 0x28,

		[Description("不限（适用于搜索未输入终点情况）")]
		[EnumMember]
		All = 0xFF,

		[Description("同城（全部）")]
		[EnumMember]
		NoSet = 0
	}
}