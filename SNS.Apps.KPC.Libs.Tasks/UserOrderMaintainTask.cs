using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Models.Repositories;
using SNS.Apps.KPC.Libs.Utils;

namespace SNS.Apps.KPC.Libs.Tasks
{
	public sealed class UserOrderMaintainTask : Interfaces.TaskBase
	{
		#region "Constructs"
		public UserOrderMaintainTask(string name, bool isEnabled, DateTime? executeDate, DateTime? executeTime)
			: base(name, isEnabled, executeDate, executeTime)
		{ } 
		#endregion

		public override bool IsMatchCondition
		{
			get
			{
				if (this.ExecuteTime != null && this.ExecuteTime.HasValue)
				{
					if (DateTime.Now.Hour != this.ExecuteTime.Value.Hour)
					{
						return false;
					}

					if (Math.Abs(DateTime.Now.Minute - this.ExecuteTime.Value.Minute) < Configurations.ConfigStore.TaskServiceSettings.TaskService_Interval)
					{
						return true;
					}
				}

				return false;
			}
		}

		public override void Execute(params object[] states)
		{
			UserOrderRepository.Instance.Sync_Status();
		}
	}
}
