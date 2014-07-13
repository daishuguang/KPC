using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Net;
using System.Text;
using System.Web;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Utils;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public struct Point
	{
		#region "Fields"
		const double CNNUM_EARTH_RADIUS = 6378.137;
		#endregion

		#region "Properties"
		[DataMember]
		public double? Longitude { get; set; }

		[DataMember]
		public double? Latitude { get; set; }
		#endregion

		#region "Properties"
		[IgnoreDataMember]
		public bool IsAvailable { get { return (this.Longitude.HasValue && this.Longitude.Value != 0 && this.Latitude.HasValue && this.Latitude.Value != 0); } } 
		#endregion

		#region "Methods"
		public override string ToString()
		{
			return string.Format("[{0}, {1}]", this.Longitude ?? 0.0000, this.Latitude ?? 0.0000);
		}

		public string ToSerialize()
		{
			return string.Format("{0}_{1}", (this.Longitude.HasValue) ? (string.Format("{0:N6}", this.Longitude.Value)) : ("0.000000"), (this.Latitude.HasValue) ? (string.Format("{0:N6}", this.Latitude.Value)) : ("0.000000"));
		}
		#endregion

		#region "Static Methods"
		/// <summary>
		/// 坐标系统转换
		/// </summary>
		/// <param name="fromCoordType"></param>
		/// <param name="toCoordType"></param>
		/// <param name="fromPoint"></param>
		/// <returns></returns>
		public static Point Convert(Point fromPoint, PointCoordType fromCoordType, PointCoordType toCoordType)
		{
			if (fromPoint.Longitude != 0 && fromPoint.Latitude != 0)
			{
				var request = WebRequest.CreateHttp(string.Format("http://api.map.baidu.com/ag/coord/convert?from={0}&to={1}&x={2}&y={3}", (int)fromCoordType, (int)toCoordType, fromPoint.Longitude, fromPoint.Latitude));

				using (var sr = new StreamReader(request.GetResponse().GetResponseStream()))
				{
					var content = sr.ReadToEnd();
					var result = Newtonsoft.Json.JsonConvert.DeserializeObject<PointResponse>(content);

					if (result.Status == 0)
					{
						return new Point
						{
							Longitude = System.Convert.ToDouble(System.Text.ASCIIEncoding.ASCII.GetString(System.Convert.FromBase64String(result.Longitude))),
							Latitude = System.Convert.ToDouble(System.Text.ASCIIEncoding.ASCII.GetString(System.Convert.FromBase64String(result.Latitude)))
						};
					}
				}
			}

			return new Point { Longitude = 0.0, Latitude = 0.0 };
		}

		/// <summary>
		/// 计算球面两点距离
		/// </summary>
		/// <param name="point1"></param>
		/// <param name="point2"></param>
		/// <returns></returns>
		public static double CalcDistance(Point? point1, Point? point2)
		{
			if (
					(point1 == null || point2 == null) ||
					(!point1.HasValue || !point2.HasValue) ||
					(!point1.Value.IsAvailable || !point2.Value.IsAvailable)
			   )
			{
				return -1;
			}

			var radLat1 = Rad(point1.Value.Latitude.Value);
			var radLat2 = Rad(point2.Value.Latitude.Value);
			var a = radLat1 - radLat2;
			var b = Rad(point1.Value.Longitude.Value) - Rad(point2.Value.Longitude.Value);
			var s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));

			s = s * CNNUM_EARTH_RADIUS;
			s = Round(s * 10000) / 10000;
			
			return s;
		}
		#endregion

		#region "Static Methods"
		static double Rad(double d)
		{
			return d * Math.PI / 180.0;
		}

		static double Round(double d)
		{
			return Math.Floor(d + 0.5);
		}   
		#endregion

		#region "Static Methods"
		public static bool operator ==(Point p1, Point p2)
		{
			return (p1.Longitude.HasValue && p2.Longitude.HasValue && p1.Longitude.Value == p2.Longitude.Value && p1.Latitude.Value == p2.Latitude.Value);
		}

		public static bool operator !=(Point p1, Point p2)
		{
			return !(p1 == p2);
		} 
		#endregion

		#region "Internal Class"
		[Serializable]
		internal class PointResponse
		{
			[Newtonsoft.Json.JsonProperty("error")]
			public int Status { get; set; }

			[Newtonsoft.Json.JsonProperty("x")]
			public string Longitude { get; set; }

			[Newtonsoft.Json.JsonProperty("y")]
			public string Latitude { get; set; }
		} 
		#endregion
	}

	public enum PointCoordType
	{
		GPS_wgs8411 = 0,
		GoogleMap_gcj0211 = 2,
		BaiduMap_bd0911 = 4
	}
}
