using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Tasks
{
    public interface ITask
    {
		bool IsMatchCondition { get; }

		bool IsEnabled { get; }

		string Name { get; }

		void Execute(params object[] states);
    }
}
