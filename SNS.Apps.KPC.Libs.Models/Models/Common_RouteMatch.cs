using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Utils;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public class RouteMatch
	{
		#region "Fields"
		List<RouteMatchItem> _lst = new List<RouteMatchItem>();
		#endregion

		#region "Constructs"
		public RouteMatch() { }
		#endregion

		#region "Properties"
		[DataMember]
		public long RouteID { get; set; }

		[DataMember]
		public IEnumerable<RouteMatchItem> Items { get { return _lst.ToArray(); } }
		#endregion

		#region "Methods"
		public void Add(RouteMatchItem matchItem)
		{
			var item = this.Items.SingleOrDefault(p => p.RouteID == matchItem.RouteID);

			if (item != null)
			{
				this._lst.Remove(item);
			}

			this._lst.Add(matchItem);
		} 

		public void Add(long routeID, RouteMatchRate matchRate)
		{
			Add(new RouteMatchItem(routeID, matchRate));
		}
		#endregion

		#region "Methods"
		public override string ToString()
		{
			var sb = new StringBuilder();

			foreach(var item in this.Items)
			{
				sb.AppendFormat("{0},", item.RouteID);
			}

			return string.Format("RouteID: {0}, Match: [{1}]", this.RouteID, sb.ToString().TrimEnd(','));
		} 
		#endregion
	}

	[DataContract]
	public class RouteMatchItem
	{
		#region "Constructs"
		public RouteMatchItem() { }

		public RouteMatchItem(long routeID, RouteMatchRate matchRate) 
		{
			this.RouteID = routeID;
			this.Rate_DateTime = matchRate.Rate_DateTime.HasValue ? matchRate.Rate_DateTime.Value : 0.0;
			this.Rate_Distance = matchRate.Rate_Distance.HasValue ? matchRate.Rate_Distance.Value : 0.0;
		}
		#endregion

		#region "Properties"
		[DataMember]
		public long RouteID { get; set; }

		[DataMember]
		public double Rate_Distance { get; set; }

		[DataMember]
		public double Rate_DateTime { get; set; }

		[DataMember]
		public bool IsInformed { get; set; }

		[DataMember]
		public int InformCount { get; set; }

		[DataMember]
		public Nullable<DateTime> InformDate { get; set; }
		#endregion
	}
}
