using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Models.DataStores;
using SNS.Apps.KPC.Libs.IServices;
using SNS.Apps.KPC.Libs.Utils;

namespace SNS.Apps.TaskConsole
{
	class UserDataImportor
	{
		public static void ImportUserData(string dataFile)
		{
			var client = default(IWeChatService);

			try
			{
				if (string.IsNullOrEmpty(dataFile) || !File.Exists(dataFile))
				{
					Console.WriteLine("Please select the correct data file!");
					return;
				}

				var lstOpenIDs = new List<string>();

				using (var stream = File.OpenText(dataFile))
				{
					while (!stream.EndOfStream)
					{
						lstOpenIDs.Add(stream.ReadLine().Trim());
					}
				}

				var accessors = LoadAllAccessors(lstOpenIDs.ToArray());

				if (accessors != null)
				{
					Console.WriteLine("Total: {0}", accessors.Count());

					client = WCFServiceClientUtility.CreateServiceChanel<IWeChatService>("wechatService");

					var i = 0;

					#region "Start to retrieve UserInfo from WeChat Server & Update to local"
					foreach (var accessor in accessors)
					{
						Console.WriteLine("No.{0}", (++i));
						Console.WriteLine("Start to retrieve user data for '{0}'...", accessor.OpenID);

						try
						{
							var userInfo = client.GetUserInfo(accessor.OpenID);

							if (userInfo != null)
							{
								Console.WriteLine("Start to update user data for '{0}'", accessor.NickName);

								UpdateUserData(accessor, userInfo);

								Console.WriteLine("Done!");
							}
							else
							{
								Console.WriteLine("Skiped!");
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Fail to update for user '{0}'. Exception: {1}", accessor.OpenID, ex.Message);
						}

						Console.WriteLine();
						Console.WriteLine();

						System.Threading.Thread.Sleep(500);
					} 
					#endregion

					Console.WriteLine("Completed!");
				}
				else
				{
					Console.WriteLine("Total: 0");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			finally
			{
				WCFServiceClientUtility.CloseServiceChannel(client);
			}
		}


		#region "Private Methods"
		static IEnumerable<Accessor> LoadAllAccessors(IEnumerable<string> openIDs)
		{
			using (var ctx = new DataModels())
			{
				var lst = new List<Accessor>();
				var results = (openIDs != null && openIDs.Count() > 0) ? (ctx.tbl_Accessor.Where(p => openIDs.Contains(p.OpenID))) : (ctx.tbl_Accessor.Where(p => p.Status.HasValue && p.Status.Value != -1 && !string.IsNullOrEmpty(p.OpenID)));

				if (results != null)
				{
					foreach (var entity in results)
					{
						if (string.IsNullOrEmpty(entity.NickName) || string.IsNullOrEmpty(entity.PortraitsUrl) || string.IsNullOrEmpty(entity.PortraitsThumbUrl))
						{
							lst.Add(new Accessor(entity));
						}
					}
				}

				return lst.ToArray();
			}
		}

		static void UpdateUserData(Accessor accessor, SNS.Apps.KPC.Libs.Models.WeChat.WeChat_UserInfo userInfo)
		{
			using (var ctx = new DataModels())
			{
				var entity = ctx.tbl_Accessor.FirstOrDefault(p => p.ID == accessor.ID);

				if (entity != null)
				{
					entity.NickName = userInfo.NickName;
					entity.Gender = userInfo.Gender;
					entity.Country = userInfo.Country;
					entity.Province = userInfo.Province;
					entity.City = userInfo.City;

					if (!string.IsNullOrEmpty(entity.UserGUID))
					{
						var entityUser = ctx.tbl_User.FirstOrDefault(p => string.Compare(p.UserGUID, entity.UserGUID, StringComparison.InvariantCultureIgnoreCase) == 0);

						if (entityUser != null)
						{
							entityUser.NickName = userInfo.NickName;
							entityUser.Gender = userInfo.Gender;
						}
					}

					if (!string.IsNullOrEmpty(userInfo.PortraitsUrl))
					{
						ThreadPool.QueueUserWorkItem(new WaitCallback(DownloadUserProfileData), new { PortraitsUrl = userInfo.PortraitsUrl, PortraitsFolder = SNS.Apps.KPC.Libs.Configurations.ConfigStore.CommonSettings.Portraits_Folder, UserGUID = accessor.UserGUID.ToString() });
					}

					ctx.SaveChanges();
				}
			}
		}

		static async void DownloadUserProfileData(object state)
		{
			var imgUrl = SNS.Apps.KPC.Libs.Utils.CommonUtility.GetPropertyValue<object, string>(state, "PortraitsUrl");
			var imgName = SNS.Apps.KPC.Libs.Utils.CommonUtility.GetPropertyValue<object, string>(state, "UserGUID");
			var imgMonth = DateTime.Now.ToString("yyyyMM");
			var imgPath = Path.Combine(SNS.Apps.KPC.Libs.Utils.CommonUtility.GetPropertyValue<object, string>(state, "PortraitsFolder"), imgMonth);

			try
			{
				if (!Directory.Exists(imgPath))
				{
					Directory.CreateDirectory(imgPath);
					Directory.CreateDirectory(Path.Combine(imgPath, "thumbs"));
				}

				var client = new WebClient();

				client.UseDefaultCredentials = true;
				client.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

				var imgData = await client.DownloadDataTaskAsync(imgUrl);

				if (imgData != null && imgData.Length > 0)
				{
					#region "Download Image File"
					using (var ms = new MemoryStream())
					{
						ms.Write(imgData, 0, imgData.Length);

						// 原图
						var img = System.Drawing.Image.FromStream(ms);
						var imgFile = string.Format("{0}/{1}.jpeg", imgPath.TrimEnd(' ', '/'), imgName);

						if (File.Exists(imgFile))
						{
							File.Delete(imgFile);
						}

						img.Save(imgFile, System.Drawing.Imaging.ImageFormat.Jpeg);

						// 缩略图
						var imgThumb = img.GetThumbnailImage(70, 70, new System.Drawing.Image.GetThumbnailImageAbort(() => { return false; }), IntPtr.Zero);
						var imgThumbFile = string.Format("{0}/thumbs/{1}.jpeg", imgPath.TrimEnd(' ', '/'), imgName);

						if (File.Exists(imgThumbFile))
						{
							File.Delete(imgThumbFile);
						}

						imgThumb.Save(imgThumbFile, System.Drawing.Imaging.ImageFormat.Jpeg);
					}
					#endregion

					#region "Save to Accessor & User"
					using (var ctx = new SNS.Apps.KPC.Libs.Models.DataStores.DataModels())
					{
						var entity_accessor = ctx.tbl_Accessor.FirstOrDefault(p => string.Compare(p.UserGUID, imgName, StringComparison.InvariantCultureIgnoreCase) == 0);

						if (entity_accessor == null)
						{
							return;
						}

						entity_accessor.PortraitsUrl = string.Format("/Content/portraits/{0}/{1}.jpeg", imgMonth, entity_accessor.UserGUID);
						entity_accessor.PortraitsThumbUrl = string.Format("/Content/portraits/{0}/thumbs/{1}.jpeg", imgMonth, entity_accessor.UserGUID);
						entity_accessor.UpdateDate = DateTime.Now;

						// User
						#region "Table: User"
						var entity_user = ctx.tbl_User.FirstOrDefault(p => string.Compare(p.UserGUID, imgName, StringComparison.InvariantCultureIgnoreCase) == 0);

						if (entity_user != null)
						{
							entity_user.PortraitsUrl = string.Format("/Content/portraits/{0}/{1}.jpeg", imgMonth, entity_accessor.UserGUID);
							entity_user.PortraitsThumbUrl = string.Format("/Content/portraits/{0}/thumbs/{1}.jpeg", imgMonth, entity_accessor.UserGUID);
							entity_user.UpdateDate = DateTime.Now;
						}
						#endregion

						ctx.SaveChanges();
					}
					#endregion
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Fail to download portraits image from URL: {0}, for user: {2}\r\nException: {3}", imgUrl, imgName, ex.ToString());
			}
		} 
		#endregion
	}
}
