using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Models.DataStores;
using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Models.WeChat;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;

namespace SNS.Apps.KPC.Libs.Models.Repositories
{
	public sealed class UserRepository : Base.RepositoryBase<UserRepository>
	{
		#region "Constructs"
		private UserRepository() { this.RepositoryKeys = new string[] { CNSTR_MEMCACHEKEY_USER_USERID, CNSTR_MEMCACHEKEY_USER_USERGUID, CNSTR_MEMCACHEKEY_USER_OPENID, CNSTR_MEMCACHEKEY_USER_MOBILE }; }
		#endregion

		#region "Public Methods"
		#region "Methods: Get"
		public User Get(long id, bool inForce = false)
		{
			var instance = default(User);

			if (inForce)
			{
				instance = Get_FromDB(id);

				RefreshMem(instance);
			}
			else
			{
				instance = GetMem(() =>
				{
					return Get_FromDB(id);
				}, CNSTR_MEMCACHEKEY_USER_USERID, id);
			}

			return instance;
		}

		public User Get(Guid id, bool inForce = false)
		{
			var instance = default(User);

			if (inForce)
			{
				instance = Get_FromDB(id);

				RefreshMem(instance);
			}
			else
			{
				instance = GetMem(() =>
				{
					return Get_FromDB(id);
				}, CNSTR_MEMCACHEKEY_USER_USERGUID, id);
			}

			return instance;
		}

		public User Get(string openID, bool inForce = false)
		{
			var instance = default(User);

			if (inForce)
			{
				instance = Get_FromDB(openID);

				RefreshMem(instance);
			}
			else
			{
				instance = GetMem(() =>
				{
					return Get_FromDB(openID);
				}, CNSTR_MEMCACHEKEY_USER_OPENID, openID);
			}

			return instance;
		}

		public User GetWithMobile(string mobile, bool inForce = false)
		{
			var instance = default(User);

			if (inForce)
			{
				instance = GetWithMobile_FromDB(mobile);

				RefreshMem(instance);
			}
			else
			{
				instance = GetMem(() =>
				{
					return Get_FromDB(mobile);
				}, CNSTR_MEMCACHEKEY_USER_MOBILE, mobile);
			}

			return instance;
		}

		public User GetWithMobile(string mobile, string password)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				dynamic entity;

				// tbl_User
				entity = ctx.tbl_User.FirstOrDefault(p => string.Compare(p.Mobile, mobile, StringComparison.InvariantCultureIgnoreCase) == 0 && string.Compare(p.Password, password) == 0);

				if (entity == null && ConfigStore.CommonSettings.ExternalData_Enabled)
				{
					// tbl_User_External
					entity = ctx.tbl_User_External.FirstOrDefault(p => string.Compare(p.Mobile, mobile, StringComparison.InvariantCultureIgnoreCase) == 0 && string.Compare(p.Password, password) == 0);
				}

				return (entity != null) ? (new User(entity)) : (null);
			}
		}

		public UserPrivy GetPrivy(long id)
		{
			var instance = Get(id);

			if (instance != null)
			{
				return new UserPrivy(instance);
			}

			return null;
		}

		public UserSecurity GetSecurity(long id)
		{
			var instance = Get(id);

			if (instance != null)
			{
				return new UserSecurity(instance);
			}

			return null;
		}

		User Get_FromDB(long id)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				dynamic entity;

				if (ConfigStore.CommonSettings.ExternalData_Enabled && id >= ConfigStore.CommonSettings.ExternalData_StartedID)
				{
					// tbl_User_External
					entity = ctx.tbl_User_External.FirstOrDefault(p => p.ID == id);
				}
				else if (ConfigStore.CommonSettings.ImportData_Enabled && id >= ConfigStore.CommonSettings.ImportData_StartedID)
				{
					// tbl_User_Import
					entity = ctx.tbl_User_Import.FirstOrDefault(p => p.ID == id);
				}
				else
				{
					// tbl_User
					entity = ctx.tbl_User.FirstOrDefault(p => p.ID == id);
				}

				return (entity != null) ? (new User(entity)) : (null);
			}
		}

		User Get_FromDB(Guid id)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var uID = id.ToString();
				dynamic entity;

				// tbl_User
				entity = ctx.tbl_User.FirstOrDefault(p => string.Compare(p.UserGUID, uID, StringComparison.InvariantCultureIgnoreCase) == 0);

				if (entity == null && ConfigStore.CommonSettings.ImportData_Enabled)
				{
					// tbl_User_Import
					entity = ctx.tbl_User_Import.FirstOrDefault(p => string.Compare(p.UserGUID, uID, StringComparison.InvariantCultureIgnoreCase) == 0);
				}

				if (entity == null && ConfigStore.CommonSettings.ExternalData_Enabled)
				{
					// tbl_User_External
					entity = ctx.tbl_User_External.FirstOrDefault(p => string.Compare(p.UserGUID, uID, StringComparison.InvariantCultureIgnoreCase) == 0);
				}

				return (entity != null) ? (new User(entity)) : (null);
			}
		}

		User Get_FromDB(string openID)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var user = ctx.tbl_User.FirstOrDefault(p => string.Compare(p.OpenID, openID) == 0);

				return (user != null) ? (new User(user)) : (null);
			}
		}

		User GetWithMobile_FromDB(string mobile)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				dynamic entity;

				// tbl_User
				entity = ctx.tbl_User.FirstOrDefault(p => string.Compare(p.Mobile, mobile, StringComparison.InvariantCultureIgnoreCase) == 0);

				if (entity == null && ConfigStore.CommonSettings.ImportData_Enabled)
				{
					// tbl_User_Import
					entity = ctx.tbl_User_Import.FirstOrDefault(p => string.Compare(p.Mobile, mobile, StringComparison.InvariantCultureIgnoreCase) == 0);
				}

				if (entity == null && ConfigStore.CommonSettings.ExternalData_Enabled)
				{
					// tbl_User_External
					entity = ctx.tbl_User_External.FirstOrDefault(p => string.Compare(p.Mobile, mobile, StringComparison.InvariantCultureIgnoreCase) == 0);
				}

				return (entity != null) ? (new User(entity)) : (null);
			}
		}
		#endregion

		#region "Methods: ResetPass"
		public bool ResetPass(string mobile, string password)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				dynamic entity;

				// tbl_User
				entity = ctx.tbl_User.FirstOrDefault(p => string.Compare(p.Mobile, mobile) == 0);

				if (entity == null && ConfigStore.CommonSettings.ExternalData_Enabled)
				{
					entity = ctx.tbl_User_External.FirstOrDefault(p => string.Compare(p.Mobile, mobile) == 0);
				}

				if (entity != null && !string.IsNullOrEmpty(entity.Password))
				{
					entity.Password = password;

					ctx.SaveChanges();

					return true;
				}
			}

			return false;
		} 
		#endregion

		#region "Methods: CheckIsFirst"
		public bool CheckIsSubscribeFirst(long userID)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var entity = ctx.tbl_User.SingleOrDefault(p => p.ID == userID);

				return (entity != null && (
												(entity.Status != null && entity.Status.HasValue && entity.Status.Value == (int)UserStatus.Subscribed) &&
												(entity.SubscribeCount != null && entity.SubscribeCount.HasValue && entity.SubscribeCount.Value == 1)
										  )
					   );
			}
		} 

		public bool CheckIsRegisterFirst(long userID)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var entity = ctx.tbl_User.SingleOrDefault(p => p.ID == userID);

				return (entity != null && (
												(entity.Status != null && entity.Status.HasValue && entity.Status.Value == (int)UserStatus.Registered) && 
												(entity.RegisterCount != null && entity.RegisterCount.HasValue && entity.RegisterCount.Value == 1)
										  )
					   );
			}
		}

		public bool CheckIsUnsubscribeFirst(long userID)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var entity = ctx.tbl_User.SingleOrDefault(p => p.ID == userID);

				return (entity != null && (
												(entity.Status != null && entity.Status.HasValue && entity.Status.Value == (int)UserStatus.UnSubscribed) &&
												(entity.UnsubscribeCount != null && entity.UnsubscribeCount.HasValue && entity.UnsubscribeCount.Value == 1)
										  )
				);
			}
		} 
		#endregion

		#region "Methods: 关注 & 取消关注 & 更新"
		public User Subscribe(string openID, WeChat.WeChat_UserInfo userWeChat, int? sceneID)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				using (var ctx_transaction = ctx.Database.BeginTransaction())
				{
					var entity = ctx.tbl_User.FirstOrDefault(p => string.Compare(p.OpenID, openID) == 0);
					var isnew = (entity == null);

					#region "Table: User"

					#region "Fields"
					if (isnew)
					{
						entity = ctx.tbl_User.Create();

						entity.OpenID = openID;
						entity.UserGUID = Guid.NewGuid().ToString();
						entity.CreateDate = entity.UpdateDate = DateTime.Now;
						entity.Status = (int)UserStatus.Subscribed;
					}
					else
					{
						entity.UpdateDate = DateTime.Now;
						entity.Status = (string.IsNullOrEmpty(entity.Mobile)) ? ((int)UserStatus.Subscribed) : ((int)UserStatus.Registered);
					}

					switch (entity.Status.Value)
					{
						case (int)UserStatus.Subscribed:
							entity.SubscribeDate = DateTime.Now;
							entity.SubscribeCount = (entity.SubscribeCount.HasValue) ? (entity.SubscribeCount.Value + 1) : (1);
							break;
						case (int)UserStatus.Registered:
							entity.RegisterDate = DateTime.Now;
							entity.RegisterCount = (entity.RegisterCount.HasValue) ? (entity.RegisterCount.Value + 1) : (1);
							break;
					} 
					#endregion

					#region "Fields: UserProfile from API"
					if (userWeChat != null)
					{
						entity.NickName = userWeChat.NickName;
						entity.Gender = userWeChat.Gender;
						entity.Country = userWeChat.Country;
						entity.Province = userWeChat.Province;
						entity.City = userWeChat.City;
						entity.IsExtended = userWeChat.IsExtended;

						if (userWeChat.IsExtended && (entity.RefID == null || entity.RefID.Value == 0) && !string.IsNullOrEmpty(userWeChat.ExtendChannel) && string.Compare(userWeChat.ExtendChannel, "0") != 0)
						{
							#region "未被推广的用户：加入推广（平台合作）"
							var refItem = ctx.tbl_User_Promotion.FirstOrDefault(p => p.ChannelID.HasValue && p.ChannelID.Value == (long)UserPromotionType.ExtendChannel && string.Compare(p.ChannelCode, userWeChat.ExtendChannel, true) == 0);

							if (refItem != null)
							{
								entity.RefID = refItem.ID;
							}
							#endregion
						}
					} 
					#endregion

					#endregion

					#region "Table: User_Promotion"
					if (sceneID.HasValue && sceneID.Value > 0)
					{
						if ((!entity.RefID.HasValue || entity.RefID.Value == 0) && (!entity.IsExtended.HasValue || !entity.IsExtended.Value))
						{
							#region "未被推广的用户：加入推广，更新代理数据"
							var strSceneID = sceneID.ToString();
							var refItem = ctx.tbl_User_Promotion.FirstOrDefault(p => p.ChannelID.HasValue && p.ChannelID.Value == (long)UserPromotionType.Agent && string.Compare(p.ChannelCode, strSceneID, true) == 0);
							
							if (refItem == null)
							{
								refItem = ctx.tbl_User_Promotion.Create();

								refItem.ChannelID = (long)UserPromotionType.Agent;
								refItem.ChannelCode = sceneID.ToString();
								refItem.CreateDate = refItem.UpdateDate = DateTime.Now;

								ctx.tbl_User_Promotion.Add(refItem);
								ctx.SaveChanges();
							}

							entity.RefID = refItem.ID; 
							#endregion
						}
					}
					#endregion

					if (isnew)
					{
						ctx.tbl_User.Add(entity);
					}

					ctx.SaveChanges();
					ctx_transaction.Commit();

					#region "Set to MemCache"
					var userInstance = new User(entity);

					RefreshMem(userInstance);
					#endregion

					return userInstance;
				}
			}
		}

		public void UnSubscribe(string openID)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var entity = ctx.tbl_User.FirstOrDefault(p => string.Compare(p.OpenID, openID) == 0);

				if (entity != null)
				{
					entity.Status = (int)UserStatus.UnSubscribed;
					entity.UpdateDate = DateTime.Now;
					entity.UnsubscribeDate = DateTime.Now;
					entity.UnsubscribeCount = (entity.UnsubscribeCount.HasValue) ? (entity.UnsubscribeCount.Value + 1) : (1);

					ctx.SaveChanges();

					#region "Set to MemCache"
					var userInstance = new User(entity);

					RefreshMem(userInstance);
					#endregion
				}
			}
		}
		#endregion

		#region "Methods: 注册 & 编辑Profile"
		public User Register(UserRegisterModel registerInfo)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				dynamic entity;
				var entity_inExternal = false;

				if (registerInfo.IsExternal)
				{
					entity = ctx.tbl_User.FirstOrDefault(p => string.Compare(p.Mobile, registerInfo.Mobile, StringComparison.InvariantCultureIgnoreCase) == 0);

					if (entity == null && ConfigStore.CommonSettings.ExternalData_Enabled)
					{
						entity = ctx.tbl_User_External.FirstOrDefault(p => string.Compare(p.Mobile, registerInfo.Mobile, StringComparison.InvariantCultureIgnoreCase) == 0);
						entity_inExternal = (entity != null);
					}
				}
				else
				{
					var uID = registerInfo.UserGUID.ToString();
					 
					entity = ctx.tbl_User.FirstOrDefault(p => string.Compare(p.UserGUID, uID, StringComparison.InvariantCultureIgnoreCase) == 0);
				}

				var isnew = (entity == null);

				#region "Table: User"
				if (isnew)
				{
					if (registerInfo.IsExternal)
					{
						entity = ctx.tbl_User_External.Create();
					}
					else
					{
						entity = ctx.tbl_User.Create();
					}

					entity.CreateDate = entity.UpdateDate = DateTime.Now;
				}
				else
				{
					entity.UpdateDate = DateTime.Now;
				}

				#region "User: Fields"
				if (isnew && registerInfo.IsExternal)
				{
					entity.UserGUID = Guid.NewGuid().ToString();
				}

				entity.NickName = (!string.IsNullOrEmpty(registerInfo.NickName)) ? (registerInfo.NickName) : (entity.NickName);
				entity.Mobile = registerInfo.Mobile;
				entity.WeChatID = registerInfo.WeChatID;
				entity.Password = (registerInfo.IsExternal && !string.IsNullOrEmpty(registerInfo.Password)) ? (registerInfo.Password) : (entity.Password);
				entity.UserRole = (int)registerInfo.UserRole;
				entity.LicencePlateNumber = registerInfo.LicencePlateNumber;
				entity.Status = (int)UserStatus.Registered;

				if (!registerInfo.IsExternal)
				{
					entity.RegisterDate = DateTime.Now;
					entity.RegisterCount = (entity.RegisterCount != null) ? (entity.RegisterCount + 1) : (1);
				}
				#endregion

				#region "User: RefCode"
				if (!registerInfo.IsExternal && !isnew && entity.RefID == null && !string.IsNullOrEmpty(registerInfo.RefCode))
				{
					var refItem = ctx.tbl_User_Promotion.SingleOrDefault(p => p.ChannelID != null && p.ChannelID.HasValue && p.ChannelID.Value == (long)UserPromotionType.User && string.Compare(p.ChannelCode, registerInfo.RefCode, true) == 0);

					if (refItem != null)
					{
						entity.RefID = refItem.ID;
					}
				} 
				#endregion

				if (isnew)
				{
					if (registerInfo.IsExternal)
					{
						ctx.tbl_User_External.Add(entity);
					}
					else
					{
						ctx.tbl_User.Add(entity);
					}
				}

				ctx.SaveChanges();
				#endregion

				#region "Table: User_Import / User_External"
				var userInstance = new User(entity);

				if (registerInfo.IsExternal)
				{
					if (isnew)
					{
						Sync2UserImport(ctx, registerInfo.Mobile, userInstance.ID, registerInfo.IsExternal);
					}
					else if (!entity_inExternal)
					{
						Sync2UserExternal(ctx, registerInfo.Mobile, userInstance.ID, registerInfo.IsExternal);
					}
				}
				else
				{
					if (!isnew)
					{
						Sync2UserImport(ctx, registerInfo.Mobile, userInstance.ID, registerInfo.IsExternal);
						Sync2UserExternal(ctx, registerInfo.Mobile, userInstance.ID, registerInfo.IsExternal);
					}
				}
				#endregion

				#region "Set to MemCache"
				RefreshMem(userInstance);
				#endregion

				return userInstance;
			}
		}

		public User EditProfile(UserEditModel editInfo)
		{
			using (DataModels ctx = new DataModels())
			{
				#region "Table: User"
				var uid = editInfo.UserGUID.ToString();
				var entity = ctx.tbl_User.FirstOrDefault(p => string.Compare(p.UserGUID, uid, StringComparison.InvariantCultureIgnoreCase) == 0);
				var isnew = (entity == null);

				if (isnew)
				{
					entity = ctx.tbl_User.Create();

					entity.CreateDate = entity.UpdateDate = DateTime.Now;
				}
				else
				{
					entity.UpdateDate = DateTime.Now;
				}

				//entity.UserGUID = uid;
				//entity.NickName = editInfo.NickName;
				entity.Mobile = editInfo.Mobile;
				entity.WeChatID = editInfo.WeChatID;
				entity.UserRole = (int)editInfo.UserRole;
				//entity.Status = (int)UserStatus.Registered;
				entity.LicencePlateNumber = editInfo.LicencePlateNumber;

				if (isnew)
				{
					ctx.tbl_User.Add(entity);
				}
				#region "User: RefCode"
				else if ((entity.RefID == null || !entity.RefID.HasValue || entity.RefID.Value == 0) && !string.IsNullOrEmpty(editInfo.RefCode))
				{
					var refItem = ctx.tbl_User_Promotion.SingleOrDefault(p => p.ChannelID != null && p.ChannelID.HasValue && p.ChannelID.Value == (long)UserPromotionType.User && string.Compare(p.ChannelCode, editInfo.RefCode, true) == 0);

					if (refItem != null)
					{
						entity.RefID = refItem.ID;
					}
				} 
				#endregion

				ctx.SaveChanges();
				#endregion

				#region "Set to MemCache"
				var userInstance = new User(entity);

				RefreshMem(userInstance);
				#endregion

				return userInstance;
			}
		} 

		void Sync2UserImport(DataStores.KPCDataModels ctx, string mobile, long userID, bool isExternal)
		{
			var entity_import = ctx.tbl_User_Import.FirstOrDefault(p => (!p.IsImported.HasValue || !p.IsImported.Value) && string.Compare(p.Mobile, mobile, StringComparison.InvariantCultureIgnoreCase) == 0);

			if (entity_import != null)
			{
				using (var ctx_transaction = ctx.Database.BeginTransaction())
				{
					#region "更新外部导入数据状态： IsImported = True"
					entity_import.IsImported = true;
					entity_import.UpdateDate = DateTime.Now;
					#endregion

					#region "更新外部导入数据 User Item 在 UserRoute 中的记录"
					var entity_import_ur = ctx.tbl_User_Route.FirstOrDefault(p => p.UserID == entity_import.ID);

					if (entity_import_ur != null)
					{
						entity_import_ur.UserID = userID;

						// 移除 MemCache 中关联外部导入 User Item 的数据
						UserRouteRepository.Instance.RemoveMem(CNSTR_MEMCACHEKEY_USERROUTE_ROUTEID, entity_import_ur.RouteID);
					}
					#endregion

					#region "更新外部导入数据 User Item 在 UserTrack 中的记录"
					var entity_ut = ctx.tbl_User_Track.FirstOrDefault(p => p.UserID == userID);
					var entity_import_ut = ctx.tbl_User_Track.FirstOrDefault(p => p.UserID == entity_import.ID);
					var utInstance = default(UserTrack);

					if (entity_ut == null && entity_import_ut != null)
					{
						entity_import_ut.UserID = userID;
						utInstance = new UserTrack(entity_import_ut);
					}
					else if (entity_ut != null && entity_import_ut != null && entity_ut.UpdateDate.HasValue && entity_import_ut.UpdateDate.HasValue)
					{
						if (entity_ut.UpdateDate.Value.CompareTo(entity_import_ut.UpdateDate.Value) > 0)
						{
							utInstance = new UserTrack(entity_ut);

							ctx.tbl_User_Track.Remove(entity_import_ut);
						}
						else
						{
							entity_import_ut.UserID = userID;
							utInstance = new UserTrack(entity_import_ut);

							ctx.tbl_User_Track.Remove(entity_ut);
						}
					}

					#region "从 MemCache 中删除导入数据 UserTrack"
					UserTrackRepository.Instance.RemoveMemGreedy(string.Format("{0}_{1}", CNSTR_MEMCACHEKEY_USERTRACK_USERID, entity_import.ID));
					#endregion

					#region "从 MemCache 中更新当前 UserTrack"
					if (utInstance != null)
					{
						UserTrackRepository.Instance.SetUserTrack(utInstance.UserID.Value, utInstance.Position.Value);
					}
					#endregion
					#endregion

					ctx.SaveChanges();
					ctx_transaction.Commit();
				}
			}
		}

		void Sync2UserExternal(DataStores.KPCDataModels ctx, string mobile, long userID, bool isExternal)
		{
			var entity_external = ctx.tbl_User_External.FirstOrDefault(p => string.Compare(p.Mobile, mobile) == 0);
			
			if (entity_external != null)
			{
				using (var ctx_transaction = ctx.Database.BeginTransaction())
				{
					#region "更新外部用户数据 Password 在 User 中的记录"
					if (!string.IsNullOrEmpty(entity_external.Password))
					{
						var entity = ctx.tbl_User.FirstOrDefault(p => p.ID == userID);

						if (entity != null && string.IsNullOrEmpty(entity.Password))
						{
							entity.Password = entity_external.Password;
						}
					} 
					#endregion

					#region "更新外部用户数据 User Item 在 UserRoute 中的记录"
					var entity_external_ur = ctx.tbl_User_Route.FirstOrDefault(p => p.UserID == entity_external.ID);

					if (entity_external_ur != null)
					{
						entity_external_ur.UserID = userID;
						
						// 移除 MemCache 中关联外部导入 User Item 的数据
						UserRouteRepository.Instance.RemoveMem(CNSTR_MEMCACHEKEY_USERROUTE_ROUTEID, entity_external_ur.RouteID);
					}
					#endregion

					#region "更新外部用户数据 User Item 在 UserTrack 中的记录"
					var entity_ut = ctx.tbl_User_Track.FirstOrDefault(p => p.UserID == userID);
					var entity_external_ut = ctx.tbl_User_Track.FirstOrDefault(p => p.UserID == entity_external.ID);
					var utInstance = default(UserTrack);

					if (entity_ut == null && entity_external_ut != null)
					{
						entity_external_ut.UserID = userID;
						utInstance = new UserTrack(entity_external_ut);
					}
					else if (entity_ut != null && entity_external_ut != null && entity_ut.UpdateDate.HasValue && entity_external_ut.UpdateDate.HasValue)
					{
						if (entity_ut.UpdateDate.Value.CompareTo(entity_external_ut.UpdateDate.Value) > 0)
						{
							utInstance = new UserTrack(entity_ut);

							ctx.tbl_User_Track.Remove(entity_external_ut);
						}
						else
						{
							entity_external_ut.UserID = userID;
							utInstance = new UserTrack(entity_external_ut);

							ctx.tbl_User_Track.Remove(entity_ut);
						}
					}

					#region "从 MemCache 中删除导入数据 UserTrack"
					UserTrackRepository.Instance.RemoveMemGreedy(string.Format("{0}_{1}", CNSTR_MEMCACHEKEY_USERTRACK_USERID, entity_external.ID));
					#endregion

					#region "从 MemCache 中更新当前 UserTrack"
					if (utInstance != null)
					{
						UserTrackRepository.Instance.SetUserTrack(utInstance.UserID.Value, utInstance.Position.Value);
					}
					#endregion
					#endregion

					#region "删除外部用户数据"
					ctx.tbl_User_External.Remove(entity_external); 
					#endregion

					ctx.SaveChanges();
					ctx_transaction.Commit();
				}
			}
		}
		#endregion

		#region "Methods: 同步用户信息（WeChat API）"
		public void UpdateProfile(long userID, WeChat_UserInfo userWeChat)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var entity = ctx.tbl_User.FirstOrDefault(p => p.ID == userID);

				if (entity != null)
				{
					#region "Table: User"
					entity.NickName = userWeChat.NickName;
					entity.Country = userWeChat.Country;
					entity.Province = userWeChat.Province;
					entity.City = userWeChat.City;
					entity.Gender = userWeChat.Gender;
					entity.IsExtended = userWeChat.IsExtended;

					if (!string.IsNullOrEmpty(userWeChat.PortraitsUrl) && string.Compare(userWeChat.PortraitsUrl, entity.PortraitsUrl, StringComparison.InvariantCultureIgnoreCase) != 0)
					{
						var imgMonth = DateTime.Now.ToString("yyyyMM");

						entity.PortraitsUrl = string.Format(ConfigStore.CommonSettings.Portraits_Image, imgMonth, entity.UserGUID);
						entity.PortraitsThumbUrl = string.Format(ConfigStore.CommonSettings.Portraits_ImageThumb, imgMonth, entity.UserGUID);

						// Start to download image file
						ThreadPool.QueueUserWorkItem(new WaitCallback(DownloadPortraitsImage), new { PortraitsUrl = userWeChat.PortraitsUrl, UserGUID = entity.UserGUID, ImageFolder = imgMonth });
					}
					else if (string.IsNullOrEmpty(entity.PortraitsUrl))
					{
						//entity.PortraitsUrl = entity.PortraitsThumbUrl = string.Format(ConfigStore.CommonSettings.Portraits_TempIcon, (new Random(1)).Next(ConfigStore.CommonSettings.Portraits_TempIcon_RangeMin, ConfigStore.CommonSettings.Portraits_TempIcon_RangeMax));
						entity.PortraitsUrl = entity.PortraitsThumbUrl = ConfigStore.CommonSettings.Portraits_ImageDefault;
					}

					entity.UpdateDate = DateTime.Now;

					ctx.SaveChanges(); 
					#endregion

					#region "Set to MemCache"
					var userInstance = new User(entity);

					SetMem(userInstance, "OpenID", userInstance.OpenID);
					SetMem(userInstance, "UserID", userInstance.ID);
					SetMem(userInstance, "UserGUID", userInstance.UserGUID);
					#endregion
				}
			}
		}

		async void DownloadPortraitsImage(object state)
		{
			var imgUrl = Libs.Utils.CommonUtility.GetPropertyValue<object, string>(state, "PortraitsUrl");
			var imgName = Libs.Utils.CommonUtility.GetPropertyValue<object, string>(state, "UserGUID");
			var imgMonth = Libs.Utils.CommonUtility.GetPropertyValue<object, string>(state, "ImageFolder");
			var imgPath = Path.Combine(ConfigStore.CommonSettings.Portraits_Folder, imgMonth);

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
				}
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat("Fail to download portraits image from URL: {0}, for user: {2}\r\nException: {3}", imgUrl, imgName, ex.ToString());
			}
		}
		#endregion
		#endregion
	}
}
