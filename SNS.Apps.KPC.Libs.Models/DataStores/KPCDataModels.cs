using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models.DataStores
{
	public sealed class KPCDataModels : DataModels
	{
		#region "Constructs"
		public KPCDataModels()
			: base()
		{
			this.Database.CommandTimeout = 300;
		} 
		#endregion
	}
}
