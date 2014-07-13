using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models.DataStores_Log
{
	public sealed class KPCLogModels : DataModels_Log
	{
		#region "Constructs"
		public KPCLogModels()
			: base()
		{
			this.Database.CommandTimeout = 300;
		} 
		#endregion
	}
}
