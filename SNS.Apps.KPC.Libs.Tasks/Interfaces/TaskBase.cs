using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Utils;

namespace SNS.Apps.KPC.Libs.Tasks.Interfaces
{
	public abstract class TaskBase : ITask
	{
		#region "Constructs"
		public TaskBase(string name, bool isEnabled, DateTime? executeDate, DateTime? executeTime)
		{
			this.Name = name;
			this.IsEnabled = isEnabled;
			this.ExecuteDate = executeDate;
			this.ExecuteTime = executeTime;
		}
		#endregion

		#region "Properties"
		public bool IsEnabled { get; private set; }

		public string Name { get; private set; }

		public DateTime? ExecuteDate { get; private set; }

		public DateTime? ExecuteTime { get; private set; }

		public virtual bool IsMatchCondition { get { return true; } }
		#endregion

		#region "Interfaces: ITask"
		public abstract void Execute(params object[] states);
		#endregion
	}
}
