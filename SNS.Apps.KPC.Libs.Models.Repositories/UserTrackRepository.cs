using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Utils;

namespace SNS.Apps.KPC.Libs.Models.Repositories
{
	public sealed class UserTrackRepository : Base.RepositoryBase<UserTrackRepository>
	{
		#region "Constructs"
		private UserTrackRepository() { this.RepositoryKeys = new string[] { CNSTR_MEMCACHEKEY_USERTRACK_USERID }; }
		#endregion

		#region "Methods: GetUserTrack"
		public UserTrack GetUserTrack(long userID, bool inForce = false)
		{
			var instance = default(UserTrack);

			if (inForce)
			{
				instance = GetUserTrack_FromDB(userID);

				SetMem(instance, MemCacheExpireMode.Absolute, TimeSpan.FromMinutes(5), CNSTR_MEMCACHEKEY_USERTRACK_USERID, userID);
			}
			else
			{
				instance = GetMem(() => { return GetUserTrack_FromDB(userID); }, MemCacheExpireMode.Absolute, TimeSpan.FromMinutes(5), CNSTR_MEMCACHEKEY_USERTRACK_USERID, userID);
			}

			return instance;
		}

		public UserTrack GetUserTrack(Guid userID, bool inForce = false)
		{
			var userInstance = UserRepository.Instance.Get(userID, inForce);

			return ((userInstance != null) ? (GetUserTrack(userInstance.ID, inForce)) : (null));
		}

		public UserTrack GetUserTrack(string openID, bool inForce = false)
		{
			var userInstance = UserRepository.Instance.Get(openID, inForce);

			return ((userInstance != null) ? (GetUserTrack(userInstance.ID, inForce)) : (null));
		}

		UserTrack GetUserTrack_FromDB(long userID)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var result = ctx.tbl_User_Track.FirstOrDefault(p => p.UserID.HasValue && p.UserID.Value == userID);

				return (result != null) ? (new UserTrack(result)) : (null);
			}
		}
		#endregion

		#region "Methods: SetUserTrack"
		public void SetUserTrack(long userID, Point position)
		{
			var result = SetUserTrack2DB(userID, position);

			if (result != null)
			{
				// Set to MemCache
				SetUserTrack2Mem(result);
			}
		}

		public void SetUserTrack(Guid userID, Point position)
		{
			var userInstance = UserRepository.Instance.Get(userID);

			if (userInstance != null)
			{
				SetUserTrack(userInstance.ID, position);
			}
		}

		public void SetUserTrack(string openID, Point position)
		{
			var userInstance = UserRepository.Instance.Get(openID);

			if (userInstance != null)
			{
				SetUserTrack(userInstance.ID, position);
			}
		}

		private void SetUserTrack2Mem(UserTrack utInstance)
		{
			#region "UserTrack: Current User"
			SetMem(utInstance, MemCacheExpireMode.Absolute, TimeSpan.FromMinutes(5), CNSTR_MEMCACHEKEY_USERTRACK_USERID, utInstance.UserID);
			#endregion

			#region "UserTrack: All"
			var lstUTs = LoadUserAround(null);
			var ut = (lstUTs != null) ? (lstUTs.SingleOrDefault(p => p.UserID == utInstance.UserID)) : (null);

			if (ut != null)
			{
				lstUTs.Remove(ut);
				lstUTs.Add(utInstance);

				SetMem(lstUTs, "All");
			}
			#endregion

			#region "UserTrack: Driver"
			lstUTs = LoadUserAround(UserRole.Driver);
			ut = (lstUTs != null) ? (lstUTs.FirstOrDefault(p => p.UserID == utInstance.UserID)) : (null);

			if (ut != null)
			{
				lstUTs.Remove(ut);
			}

			SetMem(lstUTs, UserRole.Driver);
			#endregion

			#region "UserTrack: Passenger"
			lstUTs = LoadUserAround(UserRole.Passenger);
			ut = (lstUTs != null) ? (lstUTs.FirstOrDefault(p => p.UserID == utInstance.UserID)) : (null);

			if (ut != null)
			{
				lstUTs.Remove(ut);
			}

			SetMem(lstUTs, UserRole.Passenger);
			#endregion

			// 清除当前 User 的分页缓存
			RemoveMemGreedy(string.Format("{0}_{1}", CNSTR_MEMCACHEKEY_USERTRACK_USERID, utInstance.UserID));
		}

		private UserTrack SetUserTrack2DB(long userID, Point position)
		{
			if (position.IsAvailable)
			{
				using (var ctx = new DataStores.KPCDataModels())
				{
					#region "UserTrack: DB"
					var results = ctx.tbl_User_Track.Where(p => p.UserID.HasValue && p.UserID.Value == userID);
					var result = (results != null && results.Count() > 0) ? (results.FirstOrDefault()) : (null);
					var isnew = (result == null);

					if (isnew)
					{
						result = ctx.tbl_User_Track.Create();

						result.CreateDate = result.UpdateDate = DateTime.Now;
					}
					else
					{
						if (results.Count() > 1)
						{
							#region "删除多余的数据"
							foreach (var itm in results.Except(new DataStores.tbl_User_Track[] { result }))
							{
								ctx.tbl_User_Track.Remove(itm);
							}
							#endregion
						}

						result.UpdateDate = DateTime.Now;
					}

					result.UserID = userID;
					result.Longitude = position.Longitude;
					result.Latitude = position.Latitude;

					if (isnew)
					{
						ctx.tbl_User_Track.Add(result);
					}

					ctx.SaveChanges();
					#endregion

					return new UserTrack(result);
				}
			}
			else
			{
				return null;
			}
		}
		#endregion

		#region "Methods: LoadUserAround"
		public IList<UserTrack> LoadUserAround(UserRole? userRole)
		{
			#region "All Available UserTracks"
			return GetMem(() =>
			{ 
				using (var ctx = new DataStores.KPCDataModels())
				{
					var lst = new List<UserTrack>();
					var results = ctx.sp_Load_Available_UserTracks(userRole.HasValue ? (int)userRole.Value : -1);

					if (results != null)
					{
						foreach (var result in results)
						{
							lst.Add(new UserTrack(result));
						}
					}

					return lst;
				}
			}, MemCacheExpireMode.Absolute, TimeSpan.FromMinutes(5), ((userRole != null && userRole.HasValue) ? (userRole.ToString()) : ("All")));
			#endregion
		}

		public IEnumerable<UserTrackResult> LoadUserAroundList(UserTrackRequest.UserTrackRequestFilter filter, int page = 0, int count = 10)
		{
			var userInstance = UserRepository.Instance.Get(filter.UserID);
			var resultDatas = default(IEnumerable<UserTrackResult>);

			if (userInstance != null)
			{
				resultDatas = GetMem(() =>
				{
					page = (page < 0) ? (0) : (page);
					count = (count <= 0) ? (10) : (count);

					var results = UserTrackRepository.Instance.LoadUserAround(filter.UserRole);
					var uts = new List<UserTrack>();

					if (results != null)
					{
						#region "Filter Data"
						var ut = UserTrackRepository.Instance.GetUserTrack(filter.UserID, true);

						if (ut != null)
						{
							var datas = results.Where(p => p.UserID.HasValue && p.UserID.Value != ut.UserID.Value)
											   .Select(p => new { UserID = p.UserID, Position = p.Position, Distance = Point.CalcDistance(p.Position.Value, ut.Position.Value) })
											   .OrderBy(p => p.Distance);

							foreach (var data in datas.Skip(page * count).Take(count))
							{
								uts.Add(new UserTrack { UserID = data.UserID, Longitude = data.Position.Value.Longitude, Latitude = data.Position.Value.Latitude, Distance = data.Distance });
							}
						}
						#endregion
					}

					if (uts != null && uts.Count > 0)
					{
						var lstUTs = new List<UserTrackResult>();

						#region "Generate Results"
						foreach (var ut in uts)
						{
							var urs = UserRouteRepository.Instance.LoadByUserID(ut.UserID.Value, 0, 1);
							var user = (ut.UserID.HasValue) ? (UserRepository.Instance.Get(ut.UserID.Value)) : (null);
							var route = default(Route);

							if (urs != null && urs.Count() > 0)
							{
								route = urs.SingleOrDefault().Route;
							}

							lstUTs.Add(new UserTrackResult
							{
								User = user,
								Route = route,
								Position = ut.Position,
								Distance = ut.Distance
							});
						}
						#endregion

						return lstUTs.ToArray();
					}

					return null; 
				}, MemCacheExpireMode.Absolute, TimeSpan.FromMinutes(5), CNSTR_MEMCACHEKEY_USERTRACK_USERID, userInstance.ID, ((filter.UserRole != null && filter.UserRole.HasValue) ? (filter.UserRole.Value.ToString()) : ("All")), page, count);
			}

			return (resultDatas != null) ? (resultDatas) : (new UserTrackResult[] { });
		}

		public IEnumerable<UserTrackResult> LoadUserAroundMap(UserTrackRequest.UserTrackRequestFilter filter)
		{
			var userInstance = UserRepository.Instance.Get(filter.UserID);
			var resultDatas = default(IEnumerable<UserTrackResult>);

			if (userInstance != null)
			{
				resultDatas = GetMem(() =>
				{
					var results = UserTrackRepository.Instance.LoadUserAround(filter.UserRole);
					var uts = new List<UserTrack>();

					if (results != null)
					{
						#region "Filter Data"
						var ut = UserTrackRepository.Instance.GetUserTrack(filter.UserID, true);

						if (ut != null)
						{
							var datas = results.Where(p => p.UserID != ut.UserID && Range.IsInRange(filter.Range.Value, p.Position.Value))
											   .Select(p => new { UserID = p.UserID, Position = p.Position, Distance = Point.CalcDistance(p.Position.Value, ut.Position.Value) });

							foreach (var data in datas)
							{
								uts.Add(new UserTrack { UserID = data.UserID, Longitude = data.Position.Value.Longitude, Latitude = data.Position.Value.Latitude, Distance = data.Distance });
							}
						}
						#endregion
					}

					if (uts != null && uts.Count > 0)
					{
						var lstUTs = new List<UserTrackResult>();

						#region "Generate Results"
						foreach (var ut in uts.Take(100))
						{
							var urs = UserRouteRepository.Instance.LoadByUserID(ut.UserID.Value, 0, 1);
							var user = (ut.UserID.HasValue) ? (UserRepository.Instance.Get(ut.UserID.Value)) : (null);
							var route = default(Route);

							if (urs != null && urs.Count() > 0)
							{
								route = urs.SingleOrDefault().Route;
							}

							lstUTs.Add(new UserTrackResult
							{
								User = user,
								Route = route,
								Position = ut.Position,
								Distance = ut.Distance
							});
						}
						#endregion

						return lstUTs.ToArray();
					}

					return null;
				}, MemCacheExpireMode.Absolute, TimeSpan.FromMinutes(5), CNSTR_MEMCACHEKEY_USERTRACK_USERID, userInstance.ID, filter.Range.Value.ToSerialize(), ((filter.UserRole != null && filter.UserRole.HasValue) ? (filter.UserRole.Value.ToString()) : ("All")));
			}

			return (resultDatas != null) ? (resultDatas) : (new UserTrackResult[] { });
		}
		#endregion
	}
}
