using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Threading.Tasks;

namespace SNS.Apps.TaskConsole
{
	class ExternalDataImportor
	{
		public static void GenerateCode()
		{
			var conn = default(SqlConnection);

			try
			{
				conn = new SqlConnection("data source=.;initial catalog=ExternalData;integrated security=True;");

				var ada = new SqlDataAdapter(@"select ID from tbl_Phones where Code is null", conn);
				var ds = new DataSet();

				ada.Fill(ds);

				if (ds.Tables != null && ds.Tables[0].Rows.Count > 0)
				{
					var sbCmds = new StringBuilder();
					var rand = new Random(1);

					foreach (DataRow row in ds.Tables[0].Rows)
					{
						sbCmds.AppendLine(string.Format("update dbo.tbl_Phones set code = '{0}' where ID = '{1}';", rand.Next(1000, 10000), Convert.ToInt64(row["ID"])));
					}

					var cmd = new SqlCommand(sbCmds.ToString(), conn);

					conn.Open();
					cmd.ExecuteNonQuery();

					Console.WriteLine("DONE!");
				}
			}
			finally
			{
				if (conn != null && conn.State != ConnectionState.Closed)
				{
					conn.Close();
				}
			}
		}

		public static void ParseContent()
		{
			var conn = default(SqlConnection);

			try
			{
				conn = new SqlConnection("data source=.;initial catalog=ExternalData;integrated security=True;");

				var ada = new SqlDataAdapter(@"
					select	tabA.*, tabB.UserGUID
					from (
							(select top 5000 * from [dbo].[tbl_Contents] where (IsParsed is null or IsParsed = 0) and (IsDisabled is null or IsDisabled = 0)) tabA inner join (select Mobile, UserGUID from [dbo].[tbl_Phones]) tabB on tabB.Mobile = tabA.Mobile
						 )", conn);
				var ds = new DataSet();

				ada.Fill(ds);

				if (ds.Tables != null && ds.Tables[0].Rows.Count > 0)
				{
					conn.Open();

					var no = 0;

					foreach (DataRow row in ds.Tables[0].Rows)
					{
						Console.WriteLine("No.{0:D4}", (++no));

						var from_point = Geocoding(Convert.ToString(row["From_Location"]));
						var from_location = (from_point.Longitude != 0 && from_point.Latitude != 0) ? (Reverse(from_point)) : (null);

						var to_point = Geocoding(Convert.ToString(row["To_Location"]));
						var to_location = (to_point.Longitude != 0 && to_point.Latitude != 0) ? (Reverse(to_point)) : (null);

						if (from_location != null && to_location != null)
						{
							#region "Success to parse content data"
							Console.WriteLine("Start to Update to DB ...");

							try
							{
								var role = (row["IsDriver"] != null && row["IsDriver"] != DBNull.Value && !string.IsNullOrEmpty(Convert.ToString(row["IsDriver"]))) ? (1) : (0);
								var distance = SNS.Apps.KPC.Libs.Models.Point.CalcDistance(from_point, to_point);
								var charge = (row["Charge"] != null && row["Charge"] != DBNull.Value) ? (Convert.ToDecimal(row["Charge"])) : (0);

								var cmd = new SqlCommand(
									"INSERT INTO tbl_Routes(UserGUID, UserRole, RouteType, From_Location, From_Longitude, From_Latitude, From_Province, From_City, From_District, To_Location, To_Longitude, To_Latitude, To_Province, To_City, To_District, Distance, Charge) " +
									"VALUES	(@UserGUID, @UserRole, @RouteType, @From_Location, @From_Longitude, @From_Latitude, @From_Province, @From_City, @From_District, @To_Location, @To_Longitude, @To_Latitude, @To_Province, @To_City, @To_District, @Distance, @Charge)",
									conn);

								cmd.Parameters.AddWithValue("@UserGUID", row["UserGUID"]);
								cmd.Parameters.AddWithValue("@UserRole", role);
								cmd.Parameters.AddWithValue("@RouteType", (string.Compare(from_location.Province, to_location.Province, StringComparison.InvariantCultureIgnoreCase) != 0 || string.Compare(from_location.City, to_location.City, StringComparison.InvariantCultureIgnoreCase) != 0) ? (1) : (0));
								cmd.Parameters.AddWithValue("@From_Location", row["From_Location"]);
								cmd.Parameters.AddWithValue("@From_Longitude", from_point.Longitude);
								cmd.Parameters.AddWithValue("@From_Latitude", from_point.Latitude);
								cmd.Parameters.AddWithValue("@From_Province", from_location.Province);
								cmd.Parameters.AddWithValue("@From_City", from_location.City);
								cmd.Parameters.AddWithValue("@From_District", from_location.District);
								cmd.Parameters.AddWithValue("@To_Location", row["To_Location"]);
								cmd.Parameters.AddWithValue("@To_Longitude", to_point.Longitude);
								cmd.Parameters.AddWithValue("@To_Latitude", to_point.Latitude);
								cmd.Parameters.AddWithValue("@To_Province", to_location.Province);
								cmd.Parameters.AddWithValue("@To_City", to_location.City);
								cmd.Parameters.AddWithValue("@To_District", to_location.District);
								cmd.Parameters.AddWithValue("@Distance", distance);
								cmd.Parameters.AddWithValue("@Charge", charge);

								cmd.ExecuteNonQuery();

								cmd = new SqlCommand(string.Format("update dbo.tbl_Contents set IsParsed = 1 where ID = {0}", Convert.ToInt64(row["ID"])), conn);

								cmd.ExecuteNonQuery();

								Console.WriteLine("Finish to update 2 DB!");
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.Message);
							}
							#endregion
						}
						else
						{
							var cmd = new SqlCommand(string.Format("update dbo.tbl_Contents set IsDisabled = 1 where ID = {0}", Convert.ToInt64(row["ID"])), conn);

							cmd.ExecuteNonQuery();

							Console.WriteLine("Skiped!");
						}

						Console.WriteLine();
						Console.WriteLine();
					}

					Console.WriteLine("DONE!");
				}
			}
			finally
			{
				if (conn != null && conn.State != ConnectionState.Closed)
				{
					conn.Close();
				}
			}
		}

		public static void Import_Users()
		{
			var conn = default(SqlConnection);

			try
			{
				conn = new SqlConnection("data source=.;initial catalog=ExternalData;integrated security=True;");

				var ada = new SqlDataAdapter(@"select * from [ExternalData].[dbo].[tbl_Phones] where not exists (select [SNS.Apps.KPC.V2].[dbo].[tbl_User].Mobile from [SNS.Apps.KPC.V2].[dbo].[tbl_User] where [SNS.Apps.KPC.V2].[dbo].[tbl_User].Mobile = [ExternalData].[dbo].[tbl_Phones].Mobile)", conn);
				var ds = new DataSet();

				ada.Fill(ds);

				if (ds.Tables != null && ds.Tables[0].Rows.Count > 0)
				{
					var conn_kpc = new SqlConnection("data source=.;initial catalog=SNS.Apps.KPC.V2;integrated security=True;");

					try
					{
						conn_kpc.Open();

						var no = 0;
						var rand = new Random();
						var cmd = default(SqlCommand);
						var lstPhones4Imported = new List<string>();

						Console.WriteLine("Start to import User data ...");

						#region "Import User to Table: tbl_User"
						foreach (DataRow row in ds.Tables[0].Rows)
						{
							try
							{
								Console.WriteLine("No.{0:D4}", (++no));

								cmd = new SqlCommand(@"
											insert into [dbo].[tbl_User] (UserGUID, NickName, Mobile, UserRole, PortraitsUrl, PortraitsThumbUrl, CreateDate, UpdateDate, Status, IsExternal)
											values(@UserGUID, @NickName, @Mobile, @UserRole, @PortraitsUrl, @PortraitsThumbUrl, @CreateDate, @UpdateDate, 0, 1)", conn_kpc);

								cmd.Parameters.Add(new SqlParameter("@UserGUID", row["UserGUID"]));
								cmd.Parameters.Add(new SqlParameter("@NickName", row["NickName"]));
								cmd.Parameters.Add(new SqlParameter("@Mobile", row["Mobile"]));
								cmd.Parameters.Add(new SqlParameter("@UserRole", row["UserRole"]));
								cmd.Parameters.Add(new SqlParameter("@PortraitsUrl", string.Format("/content/portraits/temp/icon_{0:D4}.jpg", rand.Next(1, 431))));
								cmd.Parameters.Add(new SqlParameter("@PortraitsThumbUrl", string.Format("/content/portraits/temp/icon_{0:D4}.jpg", rand.Next(1, 431))));
								cmd.Parameters.Add(new SqlParameter("@CreateDate", DateTime.Now));
								cmd.Parameters.Add(new SqlParameter("@UpdateDate", DateTime.Now));

								cmd.ExecuteNonQuery();

								lstPhones4Imported.Add(Convert.ToString(row["Mobile"]));

								Console.WriteLine("Done!");
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.Message);
							}
							finally
							{
								Console.WriteLine();
								Console.WriteLine();
							}
						}
						#endregion

						Console.WriteLine("Finish to import User data");
						//Console.WriteLine("Start to sync status to phones table ...");

						#region "Update Status to Table: tbl_Phones"
						//conn.Open();

						//no = 0;

						//lstPhones4Imported.ForEach(p =>
						//{
						//	Console.WriteLine("No.{0:D4}", (++no));

						//	cmd = new SqlCommand(string.Format("update [dbo].[tbl_Phones] set IsImported = 1 where Mobile = '{0}'", p), conn);
						//	cmd.ExecuteNonQuery();
						//});
						#endregion

						Console.WriteLine("Completed!");
					}
					finally
					{
						if (conn_kpc != null && conn_kpc.State != ConnectionState.Closed)
						{
							conn_kpc.Close();
						}
					}
				}
			}
			finally
			{
				if (conn != null && conn.State != ConnectionState.Closed)
				{
					conn.Close();
				}
			}
		}

		public static void Import_Phones()
		{
			var conn = default(SqlConnection);

			try
			{
				conn = new SqlConnection("data source=.;initial catalog=ExternalData;integrated security=True;");

				var ada = new SqlDataAdapter(@"select Mobile, Code from [dbo].[tbl_Phones] where not exists (select Phonenum from [SNS.Apps.KPC.V2].dbo.tbl_Phone_Verify where Phonenum = Mobile)", conn);
				var ds = new DataSet();

				ada.Fill(ds);

				if (ds.Tables != null && ds.Tables[0].Rows.Count > 0)
				{
					var conn_kpc = new SqlConnection("data source=.;initial catalog=SNS.Apps.KPC.V2;integrated security=True;");

					try
					{
						conn_kpc.Open();

						Console.WriteLine("Total: ", ds.Tables[0].Rows.Count);
						Console.WriteLine("Start to import Phone data ...");

						#region "Import Mobile to Table: tbl_Phone_Verify"
						var no = 0;

						foreach (DataRow row in ds.Tables[0].Rows)
						{
							try
							{
								Console.WriteLine("No.{0:D4}", (++no));

								var cmd = new SqlCommand(string.Format("insert into dbo.tbl_Phone_Verify (Phonenum, Code, Count, CreateDate) values('{0}', '{1}', 1, GETDATE())", row["Mobile"], row["Code"]), conn_kpc);

								cmd.ExecuteNonQuery();

								Console.WriteLine("Done!");
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.Message);
							}
							finally
							{
								Console.WriteLine();
								Console.WriteLine();
							}
						}
						#endregion

						Console.WriteLine("Completed");
					}
					finally
					{
						if (conn_kpc != null && conn_kpc.State != ConnectionState.Closed)
						{
							conn_kpc.Close();
						}
					}
				}
			}
			finally
			{
				if (conn != null && conn.State != ConnectionState.Closed)
				{
					conn.Close();
				}
			}
		}

		public static void Import_Routes()
		{
			var conn = default(SqlConnection);

			try
			{
				conn = new SqlConnection("data source=.;initial catalog=ExternalData;integrated security=True;");

				var ada = new SqlDataAdapter(@"select * from [dbo].[tbl_Routes] where IsImported is null or IsImported = 0", conn);
				var ds = new DataSet();

				ada.Fill(ds);

				if (ds.Tables != null && ds.Tables[0].Rows.Count > 0)
				{
					var conn_kpc = new SqlConnection("data source=.;initial catalog=SNS.Apps.KPC.V2;integrated security=True;");

					try
					{
						conn_kpc.Open();

						Console.WriteLine("Total: ", ds.Tables[0].Rows.Count);
						Console.WriteLine("Start to import Route data ...");

						#region "Import Route to Table: tbl_Route"
						var no = 0;

						foreach (DataRow row in ds.Tables[0].Rows)
						{
							try
							{
								Console.WriteLine("No.{0:D4}", (++no));

								using (TransactionScope scope = new TransactionScope())
								{
									var cmd = new SqlCommand(string.Format("select ID from dbo.tbl_User where UserGUID = '{0}'", Convert.ToString(row["UserGUID"])), conn_kpc);
									var userID = Convert.ToInt64(cmd.ExecuteScalar());

									if (userID == 0)
									{
										Console.WriteLine("Skiped!");
										continue;
									}

									#region "Table: tbl_Route"
									cmd = new SqlCommand(@"
									insert into dbo.tbl_Route (RouteGUID, RouteType, From_Province, From_City, From_District, From_Location, From_Longitude, From_Latitude, To_Province, To_City, To_District, To_Location, To_Longitude, To_Latitude, StartDate, RepeatType, Distance, Charge, Status, CreateDate, UpdateDate) 
									values (LOWER(NEWID()), @RouteType, @From_Province, @From_City, @From_District, @From_Location, @From_Longitude, @From_Latitude, @To_Province, @To_City, @To_District, @To_Location, @To_Longitude, @To_Latitude, @StartDate, 0, @Distance, @Charge, 0, GETDATE(), GETDATE());
									select @@identity;", conn_kpc);

									cmd.Parameters.Add(new SqlParameter("@RouteType", row["RouteType"]));
									cmd.Parameters.Add(new SqlParameter("@From_Province", row["From_Province"]));
									cmd.Parameters.Add(new SqlParameter("@From_City", row["From_City"]));
									cmd.Parameters.Add(new SqlParameter("@From_District", row["From_District"]));
									cmd.Parameters.Add(new SqlParameter("@From_Location", row["From_Location"]));
									cmd.Parameters.Add(new SqlParameter("@From_Longitude", row["From_Longitude"]));
									cmd.Parameters.Add(new SqlParameter("@From_Latitude", row["From_Latitude"]));
									cmd.Parameters.Add(new SqlParameter("@To_Province", row["To_Province"]));
									cmd.Parameters.Add(new SqlParameter("@To_City", row["To_City"]));
									cmd.Parameters.Add(new SqlParameter("@To_District", row["To_District"]));
									cmd.Parameters.Add(new SqlParameter("@To_Location", row["To_Location"]));
									cmd.Parameters.Add(new SqlParameter("@To_Longitude", row["To_Longitude"]));
									cmd.Parameters.Add(new SqlParameter("@To_Latitude", row["To_Latitude"]));
									cmd.Parameters.Add(new SqlParameter("@Distance", row["Distance"]));
									cmd.Parameters.Add(new SqlParameter("@Charge", row["Charge"]));
									cmd.Parameters.Add(new SqlParameter("@StartDate", GetStartDate(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)));

									var routeID = Convert.ToInt64(cmd.ExecuteScalar());
									#endregion

									#region "Table: tbl_User_Route"
									cmd = new SqlCommand("insert into dbo.tbl_User_Route (UserID, RouteID, UserRole) values (@UserID, @RouteID, @UserRole);", conn_kpc);

									cmd.Parameters.Add(new SqlParameter("@UserID", userID));
									cmd.Parameters.Add(new SqlParameter("@RouteID", routeID));
									cmd.Parameters.Add(new SqlParameter("@UserRole", row["UserRole"]));

									cmd.ExecuteNonQuery();
									#endregion

									#region "Table: tbl_User_Track"
									cmd = new SqlCommand("sp_Set_UserTrack", conn_kpc);

									cmd.CommandType = CommandType.StoredProcedure;
									cmd.Parameters.Add(new SqlParameter("@userID", userID));
									cmd.Parameters.Add(new SqlParameter("@routeID", routeID));
									cmd.Parameters.Add(new SqlParameter("@lng", row["From_Longitude"]));
									cmd.Parameters.Add(new SqlParameter("@lat", row["From_Latitude"]));

									cmd.ExecuteNonQuery();
									#endregion

									scope.Complete();
								}

								Console.WriteLine("Done!");
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.Message);
							}
							finally
							{
								Console.WriteLine();
								Console.WriteLine();
							}
						}
						#endregion

						Console.WriteLine("Completed");
					}
					finally
					{
						if (conn_kpc != null && conn_kpc.State != ConnectionState.Closed)
						{
							conn_kpc.Close();
						}
					}
				}
			}
			finally
			{
				if (conn != null && conn.State != ConnectionState.Closed)
				{
					conn.Close();
				}
			}
		}




		#region "Private Methods"
		static SNS.Apps.KPC.Libs.Models.Point Geocoding(string location)
		{
			Console.WriteLine("Start to get Gecoding: {0}", location);

			return SNS.Apps.KPC.Libs.Models.Location.Geocoding(location);
		}

		static SNS.Apps.KPC.Libs.Models.Location Reverse(SNS.Apps.KPC.Libs.Models.Point point)
		{
			Console.WriteLine("Start to get Revers: {0}", point.ToString());

			return SNS.Apps.KPC.Libs.Models.Location.Reverse(point);
		}

		static DateTime GetStartDate(int year, int mon, int day)
		{
			var dt = new DateTime(year, mon, day, 6, 0, 0);
			var rand = new Random(1);

			dt = dt.AddMinutes(rand.Next(0, 60 * 20));

			return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour,  (int)Math.Floor((decimal)dt.Minute / 10) * 10 , 0);
		}
		#endregion
	}
}