using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;

namespace SNS.Apps.KPC.Libs.Models
{
	public class Location
	{
		#region "Constructs"
		public Location(LocationReverseResponse response)
		{
			this.Province = response.Result.AddressComponent.Province;
			this.City = response.Result.AddressComponent.City;
			this.District = response.Result.AddressComponent.District;
			this.Street = response.Result.AddressComponent.Street;
			this.StreetNumber = response.Result.AddressComponent.StreetNumber;
		} 
		#endregion

		#region "Properties"
		public string Province { get; set; }

		public string City { get; set; }

		public string District { get; set; }

		public string Street { get; set; }

		public string StreetNumber { get; set; }
		#endregion

		#region "Static Methods"
		/// <summary>
		/// 
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public static Location Reverse(Point point)
		{
			if (point.Longitude != 0 && point.Latitude != 0)
			{
				try
				{
					//var webRequest = HttpWebRequest.Create(string.Format(ConfigStore.MapAPISettings.Baidu_Location_Reverse_URI, ConfigStore.MapAPISettings.MapAPIKey_Server, point.Latitude, point.Longitude));
					var webRequest = HttpWebRequest.Create(string.Format("http://api.map.baidu.com/geocoder/v2/?ak={0}&location={1},{2}&output=json&pois=0", "16f0929daf0ec892389af1e437224d9f", point.Latitude, point.Longitude));

					using (var sr = new StreamReader(webRequest.GetResponse().GetResponseStream()))
					{
						var content = sr.ReadToEnd();

						if (!string.IsNullOrEmpty(content))
						{
							var result = Newtonsoft.Json.JsonConvert.DeserializeObject<LocationReverseResponse>(content);

							if (result != null && result.Status == 0 && result.Result != null && result.Result.AddressComponent != null)
							{
								return new Location(result);
							}
						}
					}
				}
				catch (Exception ex)
				{
					DBLogger.Instance.ErrorFormat(
						"Method: [{0}]\r\nParameters: Point: [{1}]\r\nException: {2}",
						"public static Location Reverse(Point point)",
						string.Format("Lng: {0}, Lat: {1}", point.Longitude, point.Latitude),
						ex.ToString()
					);
				}
			}

			return null;
		} 

		public static Point Geocoding(string location)
		{
			if (!string.IsNullOrEmpty(location))
			{
				try
				{
					//var webRequest = HttpWebRequest.Create(string.Format(ConfigStore.MapAPISettings.Baidu_Location_Geocoding_URI, ConfigStore.MapAPISettings.MapAPIKey_Server, location));
					var webRequest = HttpWebRequest.Create(string.Format("http://api.map.baidu.com/geocoder/v2/?ak={0}&address={1}&output=json", "16f0929daf0ec892389af1e437224d9f", location));

					using (var sr = new StreamReader(webRequest.GetResponse().GetResponseStream()))
					{
						var content = sr.ReadToEnd();

						if (!string.IsNullOrEmpty(content))
						{
							var result = Newtonsoft.Json.JsonConvert.DeserializeObject<LocationGeocodingResponse>(content);

							if (result != null && result.Status == 0 && result.Result != null && result.Result.Location != null)
							{
								return new Point { Longitude = result.Result.Location.Logitude, Latitude = result.Result.Location.Latitude };
							}
						}
					}
				}
				catch (Exception ex)
				{
					DBLogger.Instance.ErrorFormat(
						"Method: [{0}]\r\nParameters: [Location: {1}]\r\nException: {2}",
						"public static Point Geocoding(string location)",
						location,
						ex.ToString()
					);
				}
			}

			return default(Point);
		}
		#endregion
	}

	#region "Models: LocationReverseResponse"
	[Serializable]
	public class LocationReverseResponse
	{
		#region "Properties"
		[Newtonsoft.Json.JsonProperty(PropertyName = "status")]
		public int Status { get; set; }

		[Newtonsoft.Json.JsonProperty(PropertyName = "result")]
		public LocationReverseResponse_Result Result { get; set; } 
		#endregion
	}

	[Serializable]
	public class LocationReverseResponse_Result
	{
		#region "Properties"
		[Newtonsoft.Json.JsonProperty(PropertyName = "formatted_address", Required = Newtonsoft.Json.Required.AllowNull)]
		public string Formatted_Address { get; set; }

		[Newtonsoft.Json.JsonProperty(PropertyName = "business", Required = Newtonsoft.Json.Required.AllowNull)]
		public string Business { get; set; }

		[Newtonsoft.Json.JsonProperty(PropertyName = "addressComponent", Required = Newtonsoft.Json.Required.AllowNull)]
		public LocationReverseResponse_Result_AddressComponent AddressComponent { get; set; } 
		#endregion
	}

	[Serializable]
	public class LocationReverseResponse_Result_AddressComponent
	{
		#region "Properties"
		[Newtonsoft.Json.JsonProperty(PropertyName = "province")]
		public string Province { get; set; }

		[Newtonsoft.Json.JsonProperty(PropertyName = "city")]
		public string City { get; set; }

		[Newtonsoft.Json.JsonProperty(PropertyName = "district")]
		public string District { get; set; }

		[Newtonsoft.Json.JsonProperty(PropertyName = "street")]
		public string Street { get; set; }

		[Newtonsoft.Json.JsonProperty(PropertyName = "street_number")]
		public string StreetNumber { get; set; } 
		#endregion
	} 
	#endregion

	#region "Models: LocationGeocodingResponse"
	[Serializable]
	public class LocationGeocodingResponse
	{
		#region "Properties"
		[Newtonsoft.Json.JsonProperty(PropertyName = "status")]
		public int Status { get; set; }

		[Newtonsoft.Json.JsonProperty(PropertyName = "result")]
		public LocationGeocodingResponse_Result Result { get; set; }
		#endregion
	}

	[Serializable]
	public class LocationGeocodingResponse_Result
	{
		#region "Properties"
		[Newtonsoft.Json.JsonProperty(PropertyName = "location")]
		public LocationGeocodingResponse_Result_Location Location { get; set; }

		[Newtonsoft.Json.JsonProperty(PropertyName = "precise")]
		public int Precise { get; set; }

		[Newtonsoft.Json.JsonProperty(PropertyName = "confidence")]
		public int Confidence { get; set; }

		[Newtonsoft.Json.JsonProperty(PropertyName = "level")]
		public string Level { get; set; }
		#endregion
	}

	[Serializable]
	public class LocationGeocodingResponse_Result_Location
	{
		[Newtonsoft.Json.JsonProperty(PropertyName = "lng")]
		public Nullable<float> Logitude { get; set; }

		[Newtonsoft.Json.JsonProperty(PropertyName = "lat")]
		public Nullable<float> Latitude { get; set; }
	}
	#endregion
}
