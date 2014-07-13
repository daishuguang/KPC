using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Models.DataStores;
using SNS.Apps.KPC.Libs.IServices;
using SNS.Apps.KPC.Libs.Utils;

namespace SNS.Apps.TaskConsole
{
	class UserProfileValidator
	{
		#region "Fields"
		Dictionary<long, string[]> _dicData = new Dictionary<long, string[]>();

		const string CNSTR_PORTRAITS_FOLDER = @"C:\Users\Alex\Documents\My Projects\SNS.Apps.KPC\_Publish\Portraits_Images";
		#endregion

		#region "Public Methods"
		public static void Validate()
		{
			var clientW = WCFServiceClientUtility.CreateServiceChanel<IWeChatService>("wechatService");
			var clientR = WCFServiceClientUtility.CreateServiceChanel<IRepositoryService>("repositoryService");

			try
			{
				var users = LoadUserProfileData();

				if (users != null && users.Count() > 0)
				{
					Console.WriteLine("Total: {0}", users.Count());

					var no = 0;

					foreach (var user in users)
					{
						Console.WriteLine("No.{0:D4}", (++no));

						try
						{
							var userInfo = clientW.GetUserInfo(user.OpenID);

							if (userInfo != null)
							{
								clientR.UpdateProfile(user.ID, user.OpenID, userInfo);

								Console.WriteLine("DONE!");
							}
							else
							{
								Console.WriteLine("SKIPED!");
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
						finally
						{
							Console.WriteLine();
							Console.WriteLine();
							Console.WriteLine();

							System.Threading.Thread.Sleep(1000);
						}
					}
				}

				Console.WriteLine("DONE!");
			}
			finally
			{
				WCFServiceClientUtility.CloseServiceChannel(clientW);
				WCFServiceClientUtility.CloseServiceChannel(clientR);
			}
		} 

		public static void Validate2()
		{
			var clientR = WCFServiceClientUtility.CreateServiceChanel<IRepositoryService>("repositoryService");

			try
			{
				var users = LoadUserProfileData();

				if (users != null && users.Count() > 0)
				{
					Console.WriteLine("Total: {0}", users.Count());

					var no = 0;

					foreach (var user in users)
					{
						Console.WriteLine("No.{0:D4}", (++no));

						try
						{
							//var userInfo = clientW.GetUserInfo(user.OpenID);

							//if (userInfo != null)
							//{
							//	clientR.UpdateProfile(user.ID, user.OpenID, userInfo);

							//	Console.WriteLine("DONE!");
							//}
							//else
							//{
							//	Console.WriteLine("SKIPED!");
							//}


							var imgPath = user.PortraitsUrl;
							var imgName = imgPath.Substring(imgPath.LastIndexOf('\\') + 1);

							imgName = imgName.Substring(0, imgName.IndexOf('.'));


						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
						finally
						{
							Console.WriteLine();
							Console.WriteLine();
							Console.WriteLine();

							System.Threading.Thread.Sleep(1000);
						}
					}
				}
			}
			finally
			{
				WCFServiceClientUtility.CloseServiceChannel(clientR);
			}
		}
		#endregion

		#region "Private Methods"
		static IEnumerable<User> LoadUserProfileData()
		{
			var lst = new List<User>();
			var conn = new SqlConnection("data source=.;initial catalog=SNS.Apps.KPC.V2;integrated security=True;");

			try
			{
				var cmd = new SqlCommand("select top 1000 * from dbo.tbl_User where (Status >= 0) and (OpenID is not null) and (NickName is null or PortraitsUrl is null) order by CreateDate DESC", conn);
				var ada = new SqlDataAdapter(cmd);
				var ds = new DataSet();

				ada.Fill(ds);

				if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in ds.Tables[0].Rows)
					{
						var user = new User
						{
							ID = Convert.ToInt64(row["ID"]),
							OpenID = Convert.ToString(row["OpenID"]),
							UserGUID = Guid.Parse(Convert.ToString(row["UserGUID"]))
						};

						lst.Add(user);
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

			return lst.ToArray();
		}

		static IEnumerable<User> LoadUserProfileData2()
		{
			var lst = new List<User>();
			var conn = new SqlConnection("data source=.;initial catalog=SNS.Apps.KPC.V2;integrated security=True;");

			try
			{
				var cmd = new SqlCommand("select * from dbo.tbl_User where (Status >= 0) and (OpenID is not null) and (PortraitsUrl is not null) order by CreateDate DESC", conn);
				var ada = new SqlDataAdapter(cmd);
				var ds = new DataSet();

				ada.Fill(ds);

				if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in ds.Tables[0].Rows)
					{
						var user = new User
						{
							ID = Convert.ToInt64(row["ID"]),
							OpenID = Convert.ToString(row["OpenID"]),
							UserGUID = Guid.Parse(Convert.ToString(row["UserGUID"])),
							PortraitsUrl = Convert.ToString(row["PortraitsUrl"]),
							PortraitsThumbUrl = Convert.ToString(row["PortraitsThumbUrl"])
						};

						lst.Add(user);
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

			return lst.ToArray();
		}
		#endregion
	}
}
