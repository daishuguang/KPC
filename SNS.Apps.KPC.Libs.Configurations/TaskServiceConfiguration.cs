using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Configurations
{
	public sealed class TaskServiceConfiguration : ConfigurationSection
	{
		[ConfigurationProperty("", IsDefaultCollection = true, DefaultValue = null)]
		public TaskServiceElements Tasks { get { return ((TaskServiceElements)this[""]); } set { this[""] = value; } }
	}

	[ConfigurationCollection(typeof(TaskServiceElement), AddItemName = "add", CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	public sealed class TaskServiceElements : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new TaskServiceElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return (element as TaskServiceElement).Name;
		}
	}

	public sealed class TaskServiceElement : ConfigurationElement
	{
		#region "Properties"
		[ConfigurationProperty("name", IsKey = true, IsRequired = true)]
		public string Name { get { return Convert.ToString(this["name"]); } set { this["name"] = value; } }

		[ConfigurationProperty("isEnabled", DefaultValue = true)]
		public bool IsEnabled { get { return (this["isEnabled"] != null) ? (Convert.ToBoolean(this["isEnabled"])) : (true); } set { this["isEnabled"] = value; } }

		[ConfigurationProperty("executeDate", DefaultValue = null)]
		public DateTime? ExecuteDate { get { return (this["executeDate"] != null) ? (((DateTime)this["executeDate"]).Date) : ((DateTime?)null); } set { this["executeDate"] = value; } }

		[ConfigurationProperty("executeTime", DefaultValue = null)]
		public DateTime? ExecuteTime { get { return (this["executeTime"] != null) ? ((DateTime)this["executeTime"]) : ((DateTime?)null); } set { this["executeTime"] = value; } }

		[ConfigurationProperty("type", IsRequired = true)]
		public string Type { get { return Convert.ToString(this["type"]); } set { this["type"] = value; } }
		#endregion
	}
}
