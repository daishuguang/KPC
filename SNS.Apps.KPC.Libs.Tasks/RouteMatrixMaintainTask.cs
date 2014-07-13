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
	public sealed class RouteMatrixMaintainTask : Interfaces.TaskBase
	{
		#region "Constructs"
		public RouteMatrixMaintainTask(string name, bool isEnabled, DateTime? executeDate, DateTime? executeTime)
			: base(name, isEnabled, executeDate, executeTime)
		{ } 
		#endregion

		public override void Execute(params object[] states)
		{
			RouteMatrixRepository.Instance.Maintain();
		}
	}
}
