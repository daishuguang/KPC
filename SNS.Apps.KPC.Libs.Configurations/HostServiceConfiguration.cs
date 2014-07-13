using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Configurations
{
	public class HostServiceConfiguration : ConfigurationSection
	{
		[ConfigurationProperty("", IsDefaultCollection = true, DefaultValue = null)]
		public HostServiceElements Services { get { return ((HostServiceElements)this[""]); } set { this[""] = value; } }
	}

	[ConfigurationCollection(typeof(HostServiceElement), AddItemName = "add", CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	public class HostServiceElements : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new HostServiceElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return (element as HostServiceElement).Name;
		}
	}

	public class HostServiceElement : ConfigurationElement
	{
		#region "Properties"
		[ConfigurationProperty("name", IsKey = true, IsRequired = true)]
		public string Name { get { return Convert.ToString(this["name"]); } set { this["name"] = value; } }

		[ConfigurationProperty("type", IsRequired = true)]
		public string Type { get { return Convert.ToString(this["type"]); } set { this["type"] = value; } }
		#endregion
	}
}
