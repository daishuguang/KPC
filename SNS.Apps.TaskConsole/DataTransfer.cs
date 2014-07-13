using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;

namespace SNS.Apps.TaskConsole
{
	class DataTransfer
	{
		public static void TransData_UserRoute()
		{
			var conn_bak = default(SqlConnection);
			var conn_cur = default(SqlConnection);

			try
			{
				conn_bak = new SqlConnection("data source=.;initial catalog=SNS.Apps.KPC.V2.BAK;integrated security=True;");
				conn_cur = new SqlConnection("data source=.;initial catalog=SNS.Apps.KPC.V2;integrated security=True;");

				var cmd = new SqlCommand(@"
					select tab1.UserID, tab1.UserGUID, tab1.UserRole, tab1.RouteID, tabR.RouteGUID
					from
						(
							select tabU.ID as UserID, tabU.UserGUID, tabUR.UserRole, tabUR.RouteID
							from (
									(select ID, UserGUID from [dbo].[tbl_User]) tabU inner join [dbo].[tbl_User_Route] tabUR on tabUR.UserID = tabU.ID
								  ) 
						) tab1 inner join  (select ID, RouteGUID from [dbo].[tbl_Route]) tabR on tabR.ID = tab1.RouteID", conn_bak);
				var ada = new SqlDataAdapter(cmd);
				var ds_ur = new DataSet();

				ada.Fill(ds_ur);

				if (ds_ur != null && ds_ur.Tables.Count > 0 && ds_ur.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in ds_ur.Tables[0].Rows)
					{
						var userGUID = Convert.ToString(row["UserGUID"]);
						var routeGUID = Convert.ToString(row["RouteGUID"]);
						var userRole = Convert.ToInt32(row["UserRole"]);

						try
						{
							var cmd_curr = new SqlCommand(string.Format(@"
							insert into [dbo].[tbl_User_Route] (UserID, UserRole, RouteID) 
							values (
										(select UserID from  [dbo].[tbl_User] where UserGUID = '{0}'),
										{1},
										(select RouteID from  [dbo].[tbl_Route] where RouteGUID = '{2}')
								   )
							", userGUID, userRole, routeGUID), conn_cur);

							cmd_curr.ExecuteNonQuery();
						}
						catch (Exception ex)
						{
							DBLogger.Instance.ErrorFormat(
								"Fail to insert UserRoute Item: UserGUID: {0}, UserRole: {1}, RouteGUID: {2}\r\nException: {3}",
								userGUID,
								userRole,
								routeGUID,
								ex.ToString()
							);
						}
					}
				}
			}
			finally
			{
				if (conn_bak != null && conn_bak.State != ConnectionState.Closed)
				{
					conn_bak.Close();
				}

				if (conn_cur != null && conn_cur.State != ConnectionState.Closed)
				{
					conn_cur.Close();
				}
			}
		}

		public static void TransData_UserTrack()
		{
			var conn_bak = default(SqlConnection);
			var conn_cur = default(SqlConnection);

			try
			{
				conn_bak = new SqlConnection("data source=.;initial catalog=SNS.Apps.KPC.V2.BAK;integrated security=True;");
				conn_cur = new SqlConnection("data source=.;initial catalog=SNS.Apps.KPC.V2;integrated security=True;");

				var cmd = new SqlCommand(@"
					select tab1.UserID, tab1.UserGUID, tab1.Longitude, tab1.Latitude, tab1.CreateDate, tab1.UpdateDate, tab1.RouteID, tabR.RouteGUID
					from (
							select tabU.ID as UserID, tabU.UserGUID, tabUT.RouteID, tabUT.Longitude, tabUT.Latitude, tabUT.CreateDate, tabUT.UpdateDate
							from (
									(select ID, UserGUID from [dbo].[tbl_User]) tabU inner join [dbo].[tbl_User_Track] tabUT on tabUT.UserID = tabU.ID
								 )
						 ) tab1 left join (select ID, RouteGUID from [dbo].[tbl_Route]) tabR on tabR.ID = tab1.RouteID", conn_bak);
				var ada = new SqlDataAdapter(cmd);
				var ds_ur = new DataSet();

				ada.Fill(ds_ur);

				if (ds_ur != null && ds_ur.Tables.Count > 0 && ds_ur.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in ds_ur.Tables[0].Rows)
					{
						var userGUID = Convert.ToString(row["UserGUID"]);
						var routeGUID = Convert.ToString(row["RouteGUID"]);

						try
						{
							var cmd_curr = default(SqlCommand);

							if (string.IsNullOrEmpty(routeGUID))
							{
								cmd_curr = new SqlCommand(string.Format(@"
									insert into [dbo].[tbl_User_Track] (UserID, RouteID, Longitude, Latitude, CreateDate, UpdateDate) 
									values (
												(select UserID from  [dbo].[tbl_User] where UserGUID = '{0}'),
												null,
												{1},
												{2},
												{3},
												{4}
										   )
									", userGUID, Convert.ToDouble(row["Longitude"]), Convert.ToDouble(row["Latitude"]), Convert.ToDateTime(row["CreateDate"]), Convert.ToDateTime(row["UpdateDate"])), conn_cur);
							}
							else
							{
								cmd_curr = new SqlCommand(string.Format(@"
									insert into [dbo].[tbl_User_Track] (UserID, RouteID, Longitude, Latitude, CreateDate, UpdateDate) 
									values (
												(select UserID from  [dbo].[tbl_User] where UserGUID = '{0}'),
												(select RouteID from  [dbo].[tbl_Route] where RouteGUID = '{1}'),
												{2},
												{3},
												{4},
												{5}
										   )
									", userGUID, routeGUID, Convert.ToDouble(row["Longitude"]), Convert.ToDouble(row["Latitude"]), Convert.ToDateTime(row["CreateDate"]), Convert.ToDateTime(row["UpdateDate"])), conn_cur);
							}

							cmd_curr.ExecuteNonQuery();
						}
						catch (Exception ex)
						{
							DBLogger.Instance.ErrorFormat(
								"Fail to insert UserRoute Item: UserGUID: {0}, RouteGUID: {1}\r\nException: {2}",
								userGUID,
								routeGUID,
								ex.ToString()
							);
						}
					}
				}
			}
			finally
			{
				if (conn_bak != null && conn_bak.State != ConnectionState.Closed)
				{
					conn_bak.Close();
				}

				if (conn_cur != null && conn_cur.State != ConnectionState.Closed)
				{
					conn_cur.Close();
				}
			}
		}
	}
}
