using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Utils.Logger
{
	public sealed class DBLogger : Base.Logger<DBLogger>
	{
		#region "Fields"
		const string CNSTR_LOGGER_NAME = "SNS_Apps_DBLogger";
		#endregion

		#region "Constructs"
		public DBLogger() : base(CNSTR_LOGGER_NAME) { }
		#endregion
	}
}
