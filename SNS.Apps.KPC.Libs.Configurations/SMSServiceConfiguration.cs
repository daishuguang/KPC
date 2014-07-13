using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Configurations
{
	public class SMSServiceConfiguration : ConfigurationSection
	{
		[ConfigurationProperty("", IsDefaultCollection = true, DefaultValue = null)]
		public SMSServiceChannels Channels { get { return ((SMSServiceChannels)this[""]); } set { this[""] = value; } }

		//[ConfigurationProperty("default", DefaultValue = null)]
		//public SMSServiceChannel Default 
		//{ 
		//	get 
		//	{
		//		var defChannel = Convert.ToString(this["default"]);

		//		if (!string.IsNullOrEmpty(defChannel))
		//		{
		//			foreach(var channel in this.Channels)
		//			{
		//				if (((SMSServiceChannel)channel).Channel.ToString() == defChannel)
		//				{
		//					return (SMSServiceChannel)channel;
		//				}
		//			}
		//		}

		//		return null;
		//	} 
		//	set
		//	{
		//		this["default"] = this.Default.Channel;
		//	}
		//}
	}

	[ConfigurationCollection(typeof(SMSServiceChannel), AddItemName = "add", CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	public sealed class SMSServiceChannels : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new SMSServiceChannel();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return (element as SMSServiceChannel).Channel;
		}
	}

	public sealed class SMSServiceChannel : ConfigurationElement
	{
		#region "Properties"
		[ConfigurationProperty("channel", IsRequired = true, IsKey = true)]
		public int Channel { get { return Convert.ToInt32(this["channel"]); } set { this["channel"] = value; } }

		[ConfigurationProperty("name", IsRequired = true)]
		public string Name { get { return Convert.ToString(this["name"]); } set { this["name"] = value; } }

		[ConfigurationProperty("isEnabled", DefaultValue = true)]
		public bool IsEnabled { get { return (this["isEnabled"] != null) ? (Convert.ToBoolean(this["isEnabled"])) : (true); } set { this["isEnabled"] = value; } }

		[ConfigurationProperty("serviceEntry", IsRequired = true)]
		public string ServiceEntry { get { return Convert.ToString(this["serviceEntry"]); } set { this["serviceEntry"] = value; } }

		[ConfigurationProperty("userName", IsRequired = true)]
		public string UserName { get { return Convert.ToString(this["userName"]); } set { this["userName"] = value; } }

		[ConfigurationProperty("password", IsRequired = true)]
		public string Password { get { return Convert.ToString(this["password"]); } set { this["password"] = value; } }
		#endregion
	}
}
