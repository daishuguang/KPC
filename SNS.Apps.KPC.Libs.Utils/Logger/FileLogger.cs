using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Utils.Logger
{
	public sealed class FileLogger : Base.Logger<FileLogger> 
	{
		#region "Fields"
		const string CNSTR_LOGGER_NAME = "SNS_Apps_FileLogger"; 
		#endregion

		#region "Constructs"
		public FileLogger() : base(CNSTR_LOGGER_NAME) { } 
		#endregion
	}
}
