using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Models.DataStores;
using SNS.Apps.KPC.Libs.Utils;

namespace SNS.Apps.TaskConsole
{
	class RouteDataImportor
	{
		public static void ImportData()
		{
			try
			{
				Console.WriteLine("Start to verify all Data for Route Table");

				using (var ctx = new DataModels())
				{
					var results = ctx.tbl_Route;

					Console.WriteLine("Total: {0}", results.Count());

					var i = 0;

					foreach (var entity in results)
					{
						try
						{
							Console.WriteLine("No. {0}", (++i));

							var isChanged = false;

							if (string.IsNullOrEmpty(entity.From_Province) || string.IsNullOrEmpty(entity.From_City) || string.IsNullOrEmpty(entity.From_District) ||
								string.IsNullOrEmpty(entity.To_Province) || string.IsNullOrEmpty(entity.To_City) || string.IsNullOrEmpty(entity.To_District))
							{
								isChanged = SetGeoData(entity);
							}

							if ((!string.IsNullOrEmpty(entity.From_Province) && !string.IsNullOrEmpty(entity.To_Province) && string.Compare(entity.From_Province, entity.To_Province, StringComparison.InvariantCultureIgnoreCase) == 0) &&
							(!string.IsNullOrEmpty(entity.From_City) && !string.IsNullOrEmpty(entity.To_City) && string.Compare(entity.From_City, entity.To_City, StringComparison.InvariantCultureIgnoreCase) == 0))
							{
								if (!entity.RouteType.HasValue || entity.RouteType.Value != (int)RouteType.Citywide)
								{
									isChanged = true;

									entity.RouteType = (int)RouteType.Citywide;

									Console.WriteLine("Set 'RouteType' to {0}", RouteType.Citywide);
								}
							}
							else
							{
								if (!entity.RouteType.HasValue || entity.RouteType.Value != (int)RouteType.Intercity)
								{
									isChanged = true;

									entity.RouteType = (int)RouteType.Intercity;

									Console.WriteLine("Set 'RouteType' to {0}", RouteType.Intercity);
								}
							}

							if (isChanged)
							{
								ctx.SaveChanges();

								Console.WriteLine("Done!");
							}
							else
							{
								Console.WriteLine("Skiped!");
							}

							
							System.Threading.Thread.Sleep(100);

							Console.WriteLine();
							Console.WriteLine();
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.ToString());
						}
					}

					Console.WriteLine("Completed!");
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		static bool SetGeoData(tbl_Route entity)
		{
			var isChanged = false;

			if (entity.From_Longitude.HasValue && entity.From_Latitude.HasValue)
			{
				Console.WriteLine("Invoking Baidu API for reverse Start Point: [{0}, {1}]", entity.From_Longitude, entity.From_Latitude);

				var location = Location.Reverse(new Point { Longitude = entity.From_Longitude, Latitude = entity.From_Latitude });

				if (location != null)
				{
					isChanged = true;

					entity.From_Province = location.Province;
					entity.From_City = location.City;
					entity.From_District = location.District;
				}

				Console.WriteLine("Done.");
			}

			if (entity.To_Longitude.HasValue && entity.To_Latitude.HasValue)
			{
				Console.WriteLine("Invoking Baidu API for reverse End Point: [{0}, {1}]", entity.To_Longitude, entity.To_Latitude);

				var location = Location.Reverse(new Point { Longitude = entity.To_Longitude, Latitude = entity.To_Latitude });

				if (location != null)
				{
					isChanged = true;

					entity.To_Province = location.Province;
					entity.To_City = location.City;
					entity.To_District = location.District;
				}

				Console.WriteLine("Done.");
			}

			return isChanged;
		}
	}
}
