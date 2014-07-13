using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public struct Range
	{
		#region "Properties"
		[DataMember]
		public Point BL { get; set; }

		[DataMember]
		public Point TR { get; set; }
		#endregion

		#region "Methods"
		public override string ToString()
		{
			return string.Format("{0} ~ {1}", this.BL, this.TR);
		}

		public string ToSerialize()
		{
			return string.Format("{0}_{1}", this.BL.ToSerialize(), this.TR.ToSerialize());
		}
		#endregion

		#region "Static Methods"
		public static bool IsInRange(Range range, Point point)
		{
			return (range.BL.Longitude <= point.Longitude && point.Longitude <= range.TR.Longitude) && (range.BL.Latitude <= point.Latitude && point.Latitude <= range.TR.Latitude);
		}
		#endregion
	}
}
