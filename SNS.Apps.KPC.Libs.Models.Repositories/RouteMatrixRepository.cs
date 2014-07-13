using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;

namespace SNS.Apps.KPC.Libs.Models.Repositories
{
	public sealed class RouteMatrixRepository : Base.RepositoryBase<RouteMatrixRepository>
	{
		#region "Fields"
		static object _lock_Running = new object();
		static volatile bool _isRunning = false;
		#endregion

		#region "Constructs"
		private RouteMatrixRepository() { }
		#endregion

		#region "Public Methods"

		#region "Methods: Publish"
		public Guid Publish(RouteCreateRequest requestInfo)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var routeInstance = default(Route);
				var userInstance = default(User);
				var urInstance = default(UserRoute);
				var utInstance = default(UserTrack);

				using (var ctx_transaction = ctx.Database.BeginTransaction())
				{
					#region "Table: Route"
					var rID = requestInfo.Route.RouteGUID.ToString();
					var entityRoute = (requestInfo.Route.RouteGUID.CompareTo(Guid.Empty) == 0) ? (null) : (ctx.tbl_Route.FirstOrDefault(p => string.Compare(p.RouteGUID, rID, StringComparison.InvariantCultureIgnoreCase) == 0));
					var isnew = (entityRoute == null);

					if (isnew)
					{
						entityRoute = ctx.tbl_Route.Create();
					}

					#region "Route: Fields"
					Libs.Utils.CommonUtility.CopyTo(requestInfo.Route, entityRoute);

					entityRoute.RouteGUID = (isnew) ? (Guid.NewGuid().ToString()) : (rID);
					entityRoute.CreateDate = entityRoute.UpdateDate = DateTime.Now;
					entityRoute.From_Longitude = requestInfo.Route.From_Point.Longitude;
					entityRoute.From_Latitude = requestInfo.Route.From_Point.Latitude;
					entityRoute.To_Longitude = requestInfo.Route.To_Point.Longitude;
					entityRoute.To_Latitude = requestInfo.Route.To_Point.Latitude;

					entityRoute.Distance = Point.CalcDistance(requestInfo.Route.From_Point, requestInfo.Route.To_Point);
					entityRoute.RouteType = (int)requestInfo.Route.RouteType;

					// 同城
					//if (string.Compare(requestInfo.Route.From_Province, requestInfo.Route.To_Province, StringComparison.InvariantCultureIgnoreCase) == 0 &&
					//	string.Compare(requestInfo.Route.From_City, requestInfo.Route.To_City, StringComparison.InvariantCultureIgnoreCase) == 0)
					//{
					//	entityRoute.RouteType = (int)RouteType.Citywide_ShortDistance;
					//}
					//// 省内
					//else if (string.Compare(requestInfo.Route.From_Province, requestInfo.Route.To_Province, StringComparison.InvariantCultureIgnoreCase) == 0)
					//{
					//	entityRoute.RouteType = (int)RouteType.Intercity_ShortDistance;
					//	entityRoute.IsLongTerm = requestInfo.Route.IsLongTerm;
					//}
					//// 国内
					//else
					//{
					//	entityRoute.RouteType = (int)RouteType.Intercity_LongDistance;
					//	entityRoute.IsLongTerm = requestInfo.Route.IsLongTerm;
					//}

					//entityRoute.RepeatType = (int)requestInfo.Route.RepeatType;
					//entityRoute.RepeatCondition = requestInfo.Route.RepeatCondition;
					entityRoute.Status = (int)RouteStatus.Available;
					
					#endregion

					if (isnew)
					{
						ctx.tbl_Route.Add(entityRoute);
					}

					ctx.SaveChanges();

					routeInstance = new Route(entityRoute);
					#endregion

					#region "Table: User"
					userInstance = UserRepository.Instance.Get(requestInfo.User.ID);

					if (userInstance.UserRole != requestInfo.UserRole)
					{
						var entityUser = ctx.tbl_User.FirstOrDefault(p => p.ID == userInstance.ID);

						if (entityUser != null)
						{
							entityUser.UserRole = (int)requestInfo.UserRole;

							ctx.SaveChanges();

							userInstance = new User(entityUser);
						}
					}
					#endregion

					#region "Table: UserRoute"
					if (isnew)
					{
						var entityUR = ctx.tbl_User_Route.Create();

						#region "UserRoute: Fields"
						entityUR.UserID = userInstance.ID;
						entityUR.RouteID = routeInstance.ID;
						entityUR.UserRole = (int)requestInfo.UserRole;
						#endregion

						ctx.tbl_User_Route.Add(entityUR);
						ctx.SaveChanges();
					}

					urInstance = new UserRoute
					{
						User = UserRepository.Instance.Get(requestInfo.User.ID),
						Route = routeInstance,
						UserRole = requestInfo.UserRole
					};
					#endregion

					#region "Table: UserTrack"
					var entityUT = ctx.tbl_User_Track.FirstOrDefault(p => p.UserID.HasValue && p.UserID.Value == userInstance.ID);
					var isnew2 = (entityUT == null);

					#region "UserTrack: Fields"
					if (isnew2)
					{
						entityUT = ctx.tbl_User_Track.Create();
						entityUT.CreateDate = entityUT.UpdateDate = DateTime.Now;
					}
					else
					{
						entityUT.UpdateDate = DateTime.Now;
					}

					entityUT.UserID = urInstance.User.ID;
					entityUT.Longitude = routeInstance.From_Longitude;
					entityUT.Latitude = routeInstance.From_Latitude;
					#endregion

					if (isnew2)
					{
						ctx.tbl_User_Track.Add(entityUT);
					}

					ctx.SaveChanges();

					utInstance = new UserTrack(entityUT);
					#endregion

					ctx_transaction.Commit();
				}

				#region "Set to MemCache"
				// MemCache: Route
				RouteRepository.Instance.RefreshMem(routeInstance);
				/* End */

				// MemCache: User
				UserRepository.Instance.RefreshMem(userInstance);
				/* End */

				// MemCache: UserRoute
				UserRouteRepository.Instance.SetMem<UserRoute>(urInstance, CNSTR_MEMCACHEKEY_USERROUTE_ROUTEID, urInstance.Route.ID);
				/* End */

				// MemCache: UserTrack
				UserTrackRepository.Instance.SetMem(utInstance, MemCacheExpireMode.Absolute, TimeSpan.FromMinutes(5), CNSTR_MEMCACHEKEY_USERTRACK_USERID, utInstance.UserID);
				/* End */
				#endregion

				#region "Set to RouteMatrix"
				lock (_lock_Running)
				{
					SetMem(GetMem<RouteMatrix>().Add(urInstance), MemCacheExpireMode.Sliding, TimeSpan.FromMinutes(ConfigStore.CommonSettings.MemCache_ExpiresIn));
				}
				#endregion

				return routeInstance.RouteGUID;
			}
		}
		#endregion

		#region "Methods: Search"
		public IEnumerable<RouteSearchResult> Search(RouteSearchFilter filter, int? page, int? count)
		{
			page = page ?? 0;
			count = (!count.HasValue || count.Value == 0) ? (10) : (count.Value);

			var results = GetMem<IEnumerable<RouteSearchResult>>(() => 
			{
				var searchResults = ExecuteSearch(filter);

				if (searchResults != null)
				{
					return searchResults.Skip(page.Value * count.Value).Take(count.Value);
				}

				return null;
			}, MemCacheExpireMode.Absolute, TimeSpan.FromMinutes(5), filter.GetHashCode(), page, count);

			return (results != null ? results.ToArray() : new RouteSearchResult[] { });
		} 

		IEnumerable<RouteSearchResult> ExecuteSearch(RouteSearchFilter filter)
		{
			var results = GetMem<IEnumerable<RouteSearchResult>>(
				() =>
				{
					var matrixs = GetMem<RouteMatrix>();

					if (matrixs != null && matrixs.Items.Count() > 0)
					{
						var searchUR = new RouteMatrixItem(filter);
						var otherURs = matrixs.Items.Where(p => p.Route.IsAvailable &&
																(filter.UserRole == null || !filter.UserRole.HasValue || p.UserRole == filter.UserRole.Value) &&
																((filter.ExcludeSelf == null || !filter.ExcludeSelf.HasValue || !filter.ExcludeSelf.Value) || (filter.ExcludeSelf != null && filter.ExcludeSelf.HasValue && filter.ExcludeSelf.Value && p.UserID != searchUR.UserID)) &&
																(searchUR.Route.RouteType == RouteType.All || p.Route.RouteType == searchUR.Route.RouteType || p.Route.IsCitywide == searchUR.Route.IsCitywide || p.Route.IsIntercity == searchUR.Route.IsIntercity) &&
																(string.Compare(p.Route.From_Province, searchUR.Route.From_Province, StringComparison.InvariantCultureIgnoreCase) == 0) &&
																(string.Compare(p.Route.From_City, searchUR.Route.From_City, StringComparison.InvariantCultureIgnoreCase) == 0)
														   );

						if (otherURs != null)
						{
							var lst = new List<RouteSearchResult>();

							foreach( var oItem in otherURs)
							{
								var rate = CaculateMatchRate(searchUR, oItem, filter);

								if (rate.Rate_Distance >= ConfigStore.RouteMatrixServiceSettings.RouteMatrix_MatchRate)
								{
									var rs = new RouteSearchResult(UserRouteRepository.Instance.GetByRouteID(oItem.Route.ID));

									//rs.Sort_DateTime = rate.Sort_DateTime.Value;
									rs.Rate_DateTime = rate.Rate_DateTime.Value;
									rs.Rate_Distance = rate.Rate_Distance.Value;

									lst.Add(rs);
								}
							}

							if (lst.Count > 0)
							{
								//return lst.OrderByDescending(p => p.Sort_DateTime).ThenByDescending(p => p.Rate_Distance).ToArray();
								return lst.OrderBy(p => p.Rate_DateTime).ThenByDescending(p => p.Rate_Distance).ToArray();
							}
						}
					}

					return null;
				}, MemCacheExpireMode.Absolute, TimeSpan.FromMinutes(5), filter.GetHashCode());

			return results;
		}
		#endregion

		#region "Methods: Maintain"
		public void Maintain()
		{
			if (!_isRunning)
			{
				_isRunning = true;

				try
				{
					// Refresh MemCache
					var matrixs = GetMem<RouteMatrix>();

					if (matrixs == null || matrixs.Items.Count() == 0)
					{
						matrixs = Initial();
					}

					// TODO: 移除过期线路（RouteType == Temp）

					// Set to MemCache
					SetMem(matrixs, MemCacheExpireMode.Sliding, TimeSpan.FromMinutes(ConfigStore.CommonSettings.MemCache_ExpiresIn));
				}
				finally
				{
					_isRunning = false;
				}
			}
		} 
		#endregion

		#endregion

		#region "Private Methods"

		#region "Private Methods: (Initial)"
		RouteMatrix Initial()
		{
			try
			{
				if (ConfigStore.CommonSettings.Trace_Mode)
				{
					DBLogger.Instance.Info("Start to run initializing Route Matrix....");
				}

				var t1 = DateTime.Now;
				var matrixs = LoadFromDB_Routes();
				
				//// 预加载到 MemCache
				//UserRouteRepository.Instance.LoadByRouteIDs(matrixs.MemSet.Select(p => p.Route.ID));

				if (ConfigStore.CommonSettings.Trace_Mode)
				{
					DBLogger.Instance.InfoFormat(
						"--Completed to run initializing Route Matrix in {1} seconds, Total Available Routes: {0}",
						matrixs.Items.Count(),
						TimeSpan.FromTicks(DateTime.Now.Ticks - t1.Ticks).TotalSeconds
					);
				}

				return matrixs;
			}
			catch (Exception ex)
			{
				DBLogger.Instance.FatalFormat("Fail to initialize RouteMatrix ...\r\nException: {0}", ex.ToString());
			}

			return null;
		}

		RouteMatrix LoadFromDB_Routes()
		{
			var t1 = DateTime.Now;

			try
			{
				using (var ctx = new DataStores.KPCDataModels())
				{
					var results = ctx.sp_Load_Available_Routes();

					if (results != null)
					{
						return new RouteMatrix(results);
					}

					return null;
				}
			}
			finally
			{
				if (ConfigStore.CommonSettings.Trace_Mode)
				{
					DBLogger.Instance.InfoFormat("--Completed to execute 'LoadFromDB_Routes' in {0} seconds!", TimeSpan.FromTicks(DateTime.Now.Ticks - t1.Ticks).TotalSeconds);
				}
			}
		}
		#endregion

		#region "Private Methods: (Maintain)"
		void Maintain_Routes(IList<RouteMatrix> matrixs, IEnumerable<UserRoute> userRoutes)
		{
			//var lstRouteIDs = new List<long>();
			//var lstNewInstances = new List<UserRoute>();

			//#region "检索失效项，或创建新重复实例"
			//foreach (var ur in userRoutes)
			//{
			//	if (!ur.Route.IsValid4Match)
			//	{
			//		// 已失效的实例
			//		lstRouteIDs.Add(ur.Route.ID);

			//		// Route with Repeat
			//		if (ur.Route.IsValid4Repeat)
			//		{
			//			var newRoute = Route.CreateNextRepeatInstance(ur.Route);

			//			if (newRoute != null)
			//			{
			//				lstNewInstances.Add(new UserRoute { Route = newRoute, User = ur.User, UserRole = ur.UserRole });
			//			}
			//		}
			//	}
			//}
			//#endregion

			//#region "Cleanup in MemCache"
			//lstRouteIDs.ForEach(p =>
			//	{
			//		var ur = userRoutes.FirstOrDefault(s => s.Route.ID == p);

			//		// Route
			//		RouteRepository.Instance.RemoveMem("RouteID", p);
			//		RouteRepository.Instance.RemoveMem("RouteGUID", ur.Route.RouteGUID);
			//		/* End */

			//		// UserRoute
			//		UserRouteRepository.Instance.RemoveMem("RouteID", p);
			//		UserRouteRepository.Instance.RemoveMem("RouteGUID", ur.Route.RouteGUID);
			//		/* End */

			//		// UserRoute belong to User
			//		var urs = UserRouteRepository.Instance.GetByUserID(ur.User.ID);

			//		if (urs != null)
			//		{
			//			urs = urs.Where(s => s.Route.ID != p);

			//			if (urs != null && urs.Count() > 0)
			//			{
			//				UserRouteRepository.Instance.SetMem(urs, "UserID", ur.User.ID);
			//				UserRouteRepository.Instance.SetMem(urs, "UserGUID", ur.User.UserGUID);
			//				UserRouteRepository.Instance.SetMem(urs, "OpenID", ur.User.OpenID);
			//			}
			//			else
			//			{
			//				UserRouteRepository.Instance.RemoveMem("UserID", ur.User.ID);
			//				UserRouteRepository.Instance.RemoveMem("UserGUID", ur.User.UserGUID);
			//				UserRouteRepository.Instance.RemoveMem("OpenID", ur.User.OpenID);
			//			}
			//		}
			//		/* End */

			//		// RouteMatrix
			//		var matrix = matrixs.FirstOrDefault(s => s.RouteID == p);

			//		if (matrix != null)
			//		{
			//			#region "清除被对方引用的 RouteMatrixItem(s)"
			//			matrix.Matrixs.ForEach(s =>
			//							{
			//								var om = matrixs.FirstOrDefault(m => m.RouteID == s.RouteID);

			//								if (om != null && om.Matrixs != null && om.Matrixs.Count > 0)
			//								{
			//									var omItem = om.Matrixs.FirstOrDefault(m => m.RouteID == p);

			//									if (omItem != null)
			//									{
			//										om.Matrixs.Remove(omItem);
			//									}
			//								}
			//							});
			//			#endregion

			//			// 清除自身在 RouteMatrix 中的实例
			//			matrixs.Remove(matrix);
			//		}
			//		/* End */
			//	});
			//#endregion

			//#region "Create new instances for Route with Repeat"
			//lstNewInstances.ForEach(p =>
			//{
			//	Publish(new RouteCreateRequest(p.User, p.Route, p.UserRole));
			//});
			//#endregion

			//if (lstRouteIDs != null && lstRouteIDs.Count > 0)
			//{
			//	ThreadPool.QueueUserWorkItem(new WaitCallback(Sync2DB_ExpireRoute), new { RouteIDs = lstRouteIDs.ToArray() });
			//}
		}
		#endregion

		#region "Private Methods: (CaculateMatchRate)"
		RouteMatchRate CaculateMatchRate(RouteMatrixItem uItem, RouteMatrixItem oItem, RouteSearchFilter searchFilter = null)
		{
			var rate = default(RouteMatchRate);

			if (uItem.Route.RouteType == RouteType.All)
			{
				rate = CaculateMatchRate_Citywide(uItem.Route, oItem.Route, searchFilter);

				if (rate.Rate_Distance < ConfigStore.RouteMatrixServiceSettings.RouteMatrix_MatchRate)
				{
					rate = CaculateMatchRate_Intercity(uItem.Route, oItem.Route, searchFilter);
				}
			}
			else
			{
				rate = (uItem.Route.IsCitywide) ? CaculateMatchRate_Citywide(uItem.Route, oItem.Route, searchFilter) : CaculateMatchRate_Intercity(uItem.Route, oItem.Route, searchFilter);
			}

			return rate;
		}

		RouteMatchRate CaculateMatchRate_Citywide(Route uItem, Route oItem, RouteSearchFilter searchFilter = null)
		{
			var rate = new RouteMatchRate { Rate_DateTime = 0.0, Rate_Distance = 0.0 };

			#region "Rate: 时间"
			if (searchFilter != null)
			{
				#region "搜索"
				if ((searchFilter.DateFilter.Range & (int)SearchDateRange.EveryDay) != 0)
				{
					#region "Match Rate in Day"
					if ((searchFilter.DateFilter.Range & (int)SearchDateRange.EntireDay) != 0)
					{
						rate.Rate_DateTime = 100.0;
					}
					else if ((searchFilter.DateFilter.Range & (int)SearchDateRange.InMorning) != 0)
					{
						rate.Rate_DateTime = ((oItem.StartDate == null || !oItem.StartDate.HasValue) || (oItem.StartDate != null && oItem.StartDate.HasValue && oItem.StartDate.Value.Hour < 12)) ? (100.0) : (0.0);
					}
					else
					{
						rate.Rate_DateTime = ((oItem.StartDate == null || !oItem.StartDate.HasValue) || (oItem.StartDate != null && oItem.StartDate.HasValue && oItem.StartDate.Value.Hour >= 12)) ? (100.0) : (0.0);
					} 

					if (rate.Rate_DateTime > 0)
					{
						if (((int)oItem.RouteType & (int)SearchDateRange.Workday) != 0 ||
							((int)oItem.RouteType & (int)SearchDateRange.Weekend) != 0 ||
							((int)oItem.RouteType & (int)SearchDateRange.EveryDay) != 0)
						{
							rate.Rate_DateTime = 31;
						}
						else
						{
							rate.Rate_DateTime = DateTime.Now.Ticks - oItem.StartDate.Value.Ticks;

							if (rate.Rate_DateTime.Value < 0)
							{
								rate.Rate_DateTime = TimeSpan.FromTicks((long)Math.Abs(rate.Rate_DateTime.Value)).TotalDays;
							}
							else if (rate.Rate_DateTime > 0)
							{
								rate.Rate_DateTime = TimeSpan.FromTicks((long)rate.Rate_DateTime.Value).TotalDays;

								if (rate.Rate_DateTime.Value > 0 && rate.Rate_DateTime.Value <= 30)
								{
									rate.Rate_DateTime += 30;
								}
								else if (rate.Rate_DateTime.Value > 30)
								{
									rate.Rate_DateTime += 365;
								}
							}
						}
						
						//rate.Sort_DateTime = (oItem.StartDate == null || !oItem.StartDate.HasValue) ? (DateTime.Now) : (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, oItem.StartDate.Value.Hour, oItem.StartDate.Value.Minute, 0));
					}
					#endregion
				}
				else if ((searchFilter.DateFilter.Range & (int)SearchDateRange.Workday) != 0)
				{
					if (((int)oItem.RouteType & (int)SearchDateRange.Workday) != 0 ||
						((int)oItem.RouteType & (int)SearchDateRange.EveryDay) != 0 ||
						(((int)oItem.RouteType & (int)SearchDateRange.Temp) != 0 && oItem.StartDate != null && oItem.StartDate.HasValue && oItem.StartDate.Value.DayOfWeek != DayOfWeek.Saturday && oItem.StartDate.Value.DayOfWeek != DayOfWeek.Sunday))
					{
						#region "Match Rate in Day"
						if ((searchFilter.DateFilter.Range & (int)SearchDateRange.EntireDay) != 0)
						{
							rate.Rate_DateTime = 100.0;
						}
						else if ((searchFilter.DateFilter.Range & (int)SearchDateRange.InMorning) != 0)
						{
							rate.Rate_DateTime = ((oItem.StartDate == null || !oItem.StartDate.HasValue) || (oItem.StartDate.HasValue && oItem.StartDate.Value.Hour < 12)) ? (100.0) : (0.0);
						}
						else
						{
							rate.Rate_DateTime = ((oItem.StartDate == null || !oItem.StartDate.HasValue) || (oItem.StartDate.HasValue && oItem.StartDate.Value.Hour >= 12)) ? (100.0) : (0.0);
						} 

						if (rate.Rate_DateTime > 0)
						{
							if (((int)oItem.RouteType & (int)SearchDateRange.Workday) != 0 ||
								((int)oItem.RouteType & (int)SearchDateRange.Weekend) != 0 ||
								((int)oItem.RouteType & (int)SearchDateRange.EveryDay) != 0)
							{
								rate.Rate_DateTime = 31;
							}
							else
							{
								rate.Rate_DateTime = DateTime.Now.Ticks - oItem.StartDate.Value.Ticks;

								if (rate.Rate_DateTime.Value < 0)
								{
									rate.Rate_DateTime = TimeSpan.FromTicks((long)Math.Abs(rate.Rate_DateTime.Value)).TotalDays;
								}
								else if (rate.Rate_DateTime > 0)
								{
									rate.Rate_DateTime = TimeSpan.FromTicks((long)rate.Rate_DateTime.Value).TotalDays;

									if (rate.Rate_DateTime.Value > 0 && rate.Rate_DateTime.Value <= 30)
									{
										rate.Rate_DateTime += 30;
									}
									else if (rate.Rate_DateTime.Value > 30)
									{
										rate.Rate_DateTime += 365;
									}
								}
							}

							//rate.Sort_DateTime = (oItem.StartDate == null || !oItem.StartDate.HasValue) ? (DateTime.Now) : (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, oItem.StartDate.Value.Hour, oItem.StartDate.Value.Minute, 0));
						}
						#endregion
					}
				}
				else if ((searchFilter.DateFilter.Range & (int)SearchDateRange.Weekend) != 0)
				{
					if (((int)oItem.RouteType & (int)SearchDateRange.Weekend) != 0 ||
						((int)oItem.RouteType & (int)SearchDateRange.EveryDay) != 0 ||
						(((int)oItem.RouteType & (int)SearchDateRange.Temp) != 0 && oItem.StartDate != null && oItem.StartDate.HasValue && (oItem.StartDate.Value.DayOfWeek == DayOfWeek.Saturday || oItem.StartDate.Value.DayOfWeek == DayOfWeek.Sunday)))
					{
						#region "Match Rate in Day"
						if ((searchFilter.DateFilter.Range & (int)SearchDateRange.EntireDay) != 0)
						{
							rate.Rate_DateTime = 100.0;
						}
						else if ((searchFilter.DateFilter.Range & (int)SearchDateRange.InMorning) != 0)
						{
							rate.Rate_DateTime = ((oItem.StartDate == null || !oItem.StartDate.HasValue) || (oItem.StartDate.HasValue && oItem.StartDate.Value.Hour < 12)) ? (100.0) : (0.0);
						}
						else
						{
							rate.Rate_DateTime = ((oItem.StartDate == null || !oItem.StartDate.HasValue) || (oItem.StartDate.HasValue && oItem.StartDate.Value.Hour >= 12)) ? (100.0) : (0.0);
						}

						if (rate.Rate_DateTime > 0)
						{
							if (((int)oItem.RouteType & (int)SearchDateRange.Workday) != 0 ||
								((int)oItem.RouteType & (int)SearchDateRange.Weekend) != 0 ||
								((int)oItem.RouteType & (int)SearchDateRange.EveryDay) != 0)
							{
								rate.Rate_DateTime = 31;
							}
							else
							{
								rate.Rate_DateTime = DateTime.Now.Ticks - oItem.StartDate.Value.Ticks;

								if (rate.Rate_DateTime.Value < 0)
								{
									rate.Rate_DateTime = TimeSpan.FromTicks((long)Math.Abs(rate.Rate_DateTime.Value)).TotalDays;
								}
								else if (rate.Rate_DateTime > 0)
								{
									rate.Rate_DateTime = TimeSpan.FromTicks((long)rate.Rate_DateTime.Value).TotalDays;

									if (rate.Rate_DateTime.Value > 0 && rate.Rate_DateTime.Value <= 30)
									{
										rate.Rate_DateTime += 30;
									}
									else if (rate.Rate_DateTime.Value > 30)
									{
										rate.Rate_DateTime += 365;
									}
								}
							}

							//rate.Sort_DateTime = (oItem.StartDate == null || !oItem.StartDate.HasValue) ? (DateTime.Now) : (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, oItem.StartDate.Value.Hour, oItem.StartDate.Value.Minute, 0));
						}
						#endregion
					}
				}
				else if (searchFilter.DateFilter.Date != null && searchFilter.DateFilter.Date.HasValue)
				{
					#region "Match Rate in Day"
					if ((searchFilter.DateFilter.Range & (int)SearchDateRange.EntireDay) != 0)
					{
						rate.Rate_DateTime = (
												
							(oItem.StartDate == null || !oItem.StartDate.HasValue) ||
							(
								((int)oItem.RouteType & (int)SearchDateRange.Temp) == 0 && (
																								((searchFilter.DateFilter.Date.Value.DayOfWeek == DayOfWeek.Saturday || searchFilter.DateFilter.Date.Value.DayOfWeek == DayOfWeek.Sunday) && ((int)oItem.RouteType & (int)SearchDateRange.Weekend) != 0) ||
																								((searchFilter.DateFilter.Date.Value.DayOfWeek != DayOfWeek.Saturday && searchFilter.DateFilter.Date.Value.DayOfWeek != DayOfWeek.Sunday) && ((int)oItem.RouteType & (int)SearchDateRange.Workday) != 0) ||
																								(((int)oItem.RouteType & (int)SearchDateRange.EveryDay) != 0)
																						   )
							) ||
							(
								((int)oItem.RouteType & (int)SearchDateRange.Temp) != 0 && oItem.StartDate.Value.Date.Equals(searchFilter.DateFilter.Date.Value.Date)
							)
						) ? (100.00) : (0.00);
					}
					else if ((searchFilter.DateFilter.Range & (int)SearchDateRange.InMorning) != 0)
					{
						rate.Rate_DateTime = (

							(oItem.StartDate == null || !oItem.StartDate.HasValue) ||
							(
								((int)oItem.RouteType & (int)SearchDateRange.Temp) == 0 && (
																								((searchFilter.DateFilter.Date.Value.DayOfWeek == DayOfWeek.Saturday || searchFilter.DateFilter.Date.Value.DayOfWeek == DayOfWeek.Sunday) && ((int)oItem.RouteType & (int)SearchDateRange.Weekend) != 0 && oItem.StartDate.Value.Hour < 12) ||
																								((searchFilter.DateFilter.Date.Value.DayOfWeek != DayOfWeek.Saturday && searchFilter.DateFilter.Date.Value.DayOfWeek != DayOfWeek.Sunday) && ((int)oItem.RouteType & (int)SearchDateRange.Workday) != 0 && oItem.StartDate.Value.Hour < 12) ||
																								(((int)oItem.RouteType & (int)SearchDateRange.EveryDay) != 0 && oItem.StartDate.Value.Hour < 12)
																						   )
							) ||
							(
								((int)oItem.RouteType & (int)SearchDateRange.Temp) != 0 && oItem.StartDate.Value.Date.Equals(searchFilter.DateFilter.Date.Value.Date) && oItem.StartDate.Value.Hour < 12
							)
						) ? (100.00) : (0.00);
					}
					else
					{
						rate.Rate_DateTime = (

							(oItem.StartDate == null || !oItem.StartDate.HasValue) ||
							(
								((int)oItem.RouteType & (int)SearchDateRange.Temp) == 0 && (
																								((searchFilter.DateFilter.Date.Value.DayOfWeek == DayOfWeek.Saturday || searchFilter.DateFilter.Date.Value.DayOfWeek == DayOfWeek.Sunday) && ((int)oItem.RouteType & (int)SearchDateRange.Weekend) != 0 && oItem.StartDate.Value.Hour >= 12) ||
																								((searchFilter.DateFilter.Date.Value.DayOfWeek != DayOfWeek.Saturday && searchFilter.DateFilter.Date.Value.DayOfWeek != DayOfWeek.Sunday) && ((int)oItem.RouteType & (int)SearchDateRange.Workday) != 0 && oItem.StartDate.Value.Hour >= 12) ||
																								(((int)oItem.RouteType & (int)SearchDateRange.EveryDay) != 0 && oItem.StartDate.Value.Hour >= 12)
																						   )
							) ||
							(
								((int)oItem.RouteType & (int)SearchDateRange.Temp) != 0 && oItem.StartDate.Value.Date.Equals(searchFilter.DateFilter.Date.Value.Date) && oItem.StartDate.Value.Hour >= 12
							)
						) ? (100.00) : (0.00);
					}

					if (rate.Rate_DateTime > 0)
					{
						if (((int)oItem.RouteType & (int)SearchDateRange.Workday) != 0 ||
							((int)oItem.RouteType & (int)SearchDateRange.Weekend) != 0 ||
							((int)oItem.RouteType & (int)SearchDateRange.EveryDay) != 0)
						{
							rate.Rate_DateTime = 31;
						}
						else
						{
							rate.Rate_DateTime = DateTime.Now.Ticks - oItem.StartDate.Value.Ticks;

							if (rate.Rate_DateTime.Value < 0)
							{
								rate.Rate_DateTime = TimeSpan.FromTicks((long)Math.Abs(rate.Rate_DateTime.Value)).TotalDays;
							}
							else if (rate.Rate_DateTime > 0)
							{
								rate.Rate_DateTime = TimeSpan.FromTicks((long)rate.Rate_DateTime.Value).TotalDays;

								if (rate.Rate_DateTime.Value > 0 && rate.Rate_DateTime.Value <= 30)
								{
									rate.Rate_DateTime += 30;
								}
								else if (rate.Rate_DateTime.Value > 30)
								{
									rate.Rate_DateTime += 365;
								}
							}
						}

						//rate.Sort_DateTime = (oItem.StartDate == null || !oItem.StartDate.HasValue) ? (DateTime.Now) : (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, oItem.StartDate.Value.Hour, oItem.StartDate.Value.Minute, 0));
					}
					#endregion
				}
				#endregion
			}
			else
			{
				#region "匹配"
				var hs = (((int)oItem.RouteType & (int)SearchDateRange.Temp) != 0) ? (uItem.StartDate.HasValue && oItem.StartDate.HasValue ? TimeSpan.FromTicks(Math.Abs(uItem.StartDate.Value.Ticks - oItem.StartDate.Value.Ticks)).TotalHours : -1) : (uItem.StartDate.HasValue && oItem.StartDate.HasValue ? Math.Abs(uItem.StartDate.Value.Hour - oItem.StartDate.Value.Hour) : -1);

				rate.Rate_DateTime = (hs != -1) ? ((1 - hs / 24) * 100) : (0.0);
				#endregion
			} 
			#endregion

			if (rate.Rate_DateTime.Value <= 0)
			{
				return rate;
			}

			#region "Rate: 距离"
			#region "搜索"
			#region "匹配条件：同城"
			if (searchFilter != null && string.IsNullOrEmpty(uItem.From_Location) && string.IsNullOrEmpty(uItem.To_Location))
			{
				rate.Rate_Distance = 100.0;
			}
			#endregion
			#region "匹配条件：出发区/目的区 一致"
			else if (searchFilter != null && string.IsNullOrEmpty(uItem.From_Location))
			{
				// 省，市
				rate.Rate_Distance = 35;

				// 县区
				if (string.Compare(uItem.To_District, oItem.To_District, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					rate.Rate_Distance += 25.0;
				}

				// 坐标
				var dist = Point.CalcDistance(uItem.To_Point, oItem.To_Point);

				if (dist <= ConfigStore.RouteMatrixServiceSettings.RouteMatrix_MatchDist_Citywide)
				{
					rate.Rate_Distance += (1 - dist / ConfigStore.RouteMatrixServiceSettings.RouteMatrix_MatchDist_Citywide) * 20;
				}
			}
			else if (searchFilter != null && string.IsNullOrEmpty(uItem.To_Location))
			{
				// 省，市
				rate.Rate_Distance = 35;

				// 县区
				if (string.Compare(uItem.From_District, oItem.From_District, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					rate.Rate_Distance += 25.0;
				}

				// 坐标
				var dist = Point.CalcDistance(uItem.From_Point, oItem.From_Point);

				if (dist <= ConfigStore.RouteMatrixServiceSettings.RouteMatrix_MatchDist_Citywide)
				{
					rate.Rate_Distance += (1 - dist / ConfigStore.RouteMatrixServiceSettings.RouteMatrix_MatchDist_Citywide) * 20;
				}
			}
			#endregion
			#endregion
			else
			{
				#region "匹配"
				if (string.Compare(uItem.From_District, uItem.To_District, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					// 区内拼车

					#region "县/区"
					if (string.Compare(uItem.From_District, oItem.From_District, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						rate.Rate_Distance += 5.0;
					}

					if (string.Compare(uItem.To_District, oItem.To_District, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						rate.Rate_Distance += 5.0;
					}
					#endregion

					#region "距离：起点/终点"
					var dist1 = Point.CalcDistance(uItem.From_Point, oItem.From_Point);
					var dist2 = Point.CalcDistance(uItem.To_Point, oItem.To_Point);

					if (dist1 <= ConfigStore.RouteMatrixServiceSettings.RouteMatrix_MatchDist_Citywide)
					{
						rate.Rate_Distance += (1 - dist1 / ConfigStore.RouteMatrixServiceSettings.RouteMatrix_MatchDist_Citywide) * 45;
					}

					if (dist2 <= ConfigStore.RouteMatrixServiceSettings.RouteMatrix_MatchDist_Citywide)
					{
						rate.Rate_Distance += (1 - dist2 / ConfigStore.RouteMatrixServiceSettings.RouteMatrix_MatchDist_Citywide) * 45;
					}
					#endregion
				}
				else
				{
					// 跨区拼车

					#region "省，市"
					rate.Rate_Distance += 10.0;
					#endregion

					#region "县/区"
					if (string.Compare(uItem.From_District, oItem.From_District, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						rate.Rate_Distance += 25.0;
					}

					if (string.Compare(uItem.To_District, oItem.To_District, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						rate.Rate_Distance += 25.0;
					}
					#endregion

					#region "坐标：起点/终点"
					var dist1 = Point.CalcDistance(uItem.From_Point, oItem.From_Point);
					var dist2 = Point.CalcDistance(uItem.To_Point, oItem.To_Point);

					if (dist1 <= ConfigStore.RouteMatrixServiceSettings.RouteMatrix_MatchDist_Citywide)
					{
						rate.Rate_Distance += (1 - dist1 / ConfigStore.RouteMatrixServiceSettings.RouteMatrix_MatchDist_Citywide) * 20;
					}

					if (dist2 <= ConfigStore.RouteMatrixServiceSettings.RouteMatrix_MatchDist_Citywide)
					{
						rate.Rate_Distance += (1 - dist2 / ConfigStore.RouteMatrixServiceSettings.RouteMatrix_MatchDist_Citywide) * 20;
					}
					#endregion
				}
				#endregion
			} 
			#endregion

			return rate;
		}

		RouteMatchRate CaculateMatchRate_Intercity(Route uItem, Route oItem, RouteSearchFilter searchFilter = null)
		{
			var rate = new RouteMatchRate { Rate_DateTime = 0.0, Rate_Distance = 0.0 };

			#region "Rate: 时间"
			if (searchFilter != null)
			{
				#region "搜索"
				if ((searchFilter.DateFilter.Range & (int)SearchDateRange.EveryDay) != 0)
				{
					rate.Rate_DateTime = 100.0;

					if (((int)oItem.RouteType & (int)SearchDateRange.Workday) != 0 ||
						((int)oItem.RouteType & (int)SearchDateRange.Weekend) != 0 ||
						((int)oItem.RouteType & (int)SearchDateRange.EveryDay) != 0)
					{
						rate.Rate_DateTime = 31;
					}
					else
					{
						rate.Rate_DateTime = DateTime.Now.Ticks - oItem.StartDate.Value.Ticks;

						if (rate.Rate_DateTime.Value < 0)
						{
							rate.Rate_DateTime = TimeSpan.FromTicks((long)Math.Abs(rate.Rate_DateTime.Value)).TotalDays;
						}
						else if (rate.Rate_DateTime > 0)
						{
							rate.Rate_DateTime = TimeSpan.FromTicks((long)rate.Rate_DateTime.Value).TotalDays;

							if (rate.Rate_DateTime.Value > 0 && rate.Rate_DateTime.Value <= 30)
							{
								rate.Rate_DateTime += 30;
							}
							else if (rate.Rate_DateTime.Value > 30)
							{
								rate.Rate_DateTime += 365;
							}
						}
					}

					//rate.Sort_DateTime = (oItem.StartDate == null || !oItem.StartDate.HasValue) ? (DateTime.Now) : (oItem.StartDate.Value);
				}
				else if ((searchFilter.DateFilter.Range & (int)SearchDateRange.Workday) != 0)
				{
					if (((int)oItem.RouteType & (int)SearchDateRange.Workday) != 0 ||
						((int)oItem.RouteType & (int)SearchDateRange.EveryDay) != 0 ||
						(((int)oItem.RouteType & (int)SearchDateRange.Temp) != 0 && oItem.StartDate != null && oItem.StartDate.HasValue && oItem.StartDate.Value.DayOfWeek != DayOfWeek.Saturday && oItem.StartDate.Value.DayOfWeek != DayOfWeek.Sunday))
					{
						rate.Rate_DateTime = ((oItem.StartDate == null || !oItem.StartDate.HasValue) || (oItem.StartDate.Value.DayOfWeek != DayOfWeek.Saturday && oItem.StartDate.Value.DayOfWeek != DayOfWeek.Sunday)) ? (100.0) : (0.00);

						if (rate.Rate_DateTime > 0)
						{
							if (((int)oItem.RouteType & (int)SearchDateRange.Workday) != 0 ||
								((int)oItem.RouteType & (int)SearchDateRange.Weekend) != 0 ||
								((int)oItem.RouteType & (int)SearchDateRange.EveryDay) != 0)
							{
								rate.Rate_DateTime = 31;
							}
							else
							{
								rate.Rate_DateTime = DateTime.Now.Ticks - oItem.StartDate.Value.Ticks;

								if (rate.Rate_DateTime.Value < 0)
								{
									rate.Rate_DateTime = TimeSpan.FromTicks((long)Math.Abs(rate.Rate_DateTime.Value)).TotalDays;
								}
								else if (rate.Rate_DateTime > 0)
								{
									rate.Rate_DateTime = TimeSpan.FromTicks((long)rate.Rate_DateTime.Value).TotalDays;

									if (rate.Rate_DateTime.Value > 0 && rate.Rate_DateTime.Value <= 30)
									{
										rate.Rate_DateTime += 30;
									}
									else if (rate.Rate_DateTime.Value > 30)
									{
										rate.Rate_DateTime += 365;
									}
								}
							}

							//rate.Sort_DateTime = (oItem.StartDate == null || !oItem.StartDate.HasValue) ? (DateTime.Now) : (oItem.StartDate.Value);
						}
					}
				}
				else if ((searchFilter.DateFilter.Range & (int)SearchDateRange.Weekend) != 0)
				{
					if (((int)oItem.RouteType & (int)SearchDateRange.Weekend) != 0 ||
						((int)oItem.RouteType & (int)SearchDateRange.EveryDay) != 0 ||
						(((int)oItem.RouteType & (int)SearchDateRange.Temp) != 0 && oItem.StartDate != null && oItem.StartDate.HasValue && (oItem.StartDate.Value.DayOfWeek == DayOfWeek.Saturday || oItem.StartDate.Value.DayOfWeek == DayOfWeek.Sunday)))
					{
						rate.Rate_DateTime = ((oItem.StartDate == null || !oItem.StartDate.HasValue) || (oItem.StartDate.Value.DayOfWeek == DayOfWeek.Saturday || oItem.StartDate.Value.DayOfWeek == DayOfWeek.Sunday)) ? (100.0) : (0.00);

						if (rate.Rate_DateTime > 0)
						{
							if (((int)oItem.RouteType & (int)SearchDateRange.Workday) != 0 ||
								((int)oItem.RouteType & (int)SearchDateRange.Weekend) != 0 ||
								((int)oItem.RouteType & (int)SearchDateRange.EveryDay) != 0)
							{
								rate.Rate_DateTime = 31;
							}
							else
							{
								rate.Rate_DateTime = DateTime.Now.Ticks - oItem.StartDate.Value.Ticks;

								if (rate.Rate_DateTime.Value < 0)
								{
									rate.Rate_DateTime = TimeSpan.FromTicks((long)Math.Abs(rate.Rate_DateTime.Value)).TotalDays;
								}
								else if (rate.Rate_DateTime > 0)
								{
									rate.Rate_DateTime = TimeSpan.FromTicks((long)rate.Rate_DateTime.Value).TotalDays;

									if (rate.Rate_DateTime.Value > 0 && rate.Rate_DateTime.Value <= 30)
									{
										rate.Rate_DateTime += 30;
									}
									else if (rate.Rate_DateTime.Value > 30)
									{
										rate.Rate_DateTime += 365;
									}
								}
							}

							//rate.Sort_DateTime = (oItem.StartDate == null || !oItem.StartDate.HasValue) ? (DateTime.Now) : (oItem.StartDate.Value);
						}
					}
				}
				else if (searchFilter.DateFilter.Date != null && searchFilter.DateFilter.Date.HasValue)
				{
					rate.Rate_DateTime = (
						(oItem.StartDate == null || !oItem.StartDate.HasValue) || 
						(
							((int)oItem.RouteType & (int)SearchDateRange.Temp) == 0 && (
																							((searchFilter.DateFilter.Date.Value.DayOfWeek == DayOfWeek.Saturday || searchFilter.DateFilter.Date.Value.DayOfWeek == DayOfWeek.Sunday) && ((int)oItem.RouteType & (int)SearchDateRange.Weekend) != 0) ||
																							((searchFilter.DateFilter.Date.Value.DayOfWeek != DayOfWeek.Saturday && searchFilter.DateFilter.Date.Value.DayOfWeek != DayOfWeek.Sunday) && ((int)oItem.RouteType & (int)SearchDateRange.Workday) != 0) ||
																							(((int)oItem.RouteType & (int)SearchDateRange.EveryDay) != 0)
																					   )
						) ||
						(
							((int)oItem.RouteType & (int)SearchDateRange.Temp) != 0 && searchFilter.DateFilter.Date.Value.Date.Equals(oItem.StartDate.Value.Date)
						)
					) ? (100.0) : (0.00);

					if (rate.Rate_DateTime > 0)
					{
						if (((int)oItem.RouteType & (int)SearchDateRange.Workday) != 0 ||
							((int)oItem.RouteType & (int)SearchDateRange.Weekend) != 0 ||
							((int)oItem.RouteType & (int)SearchDateRange.EveryDay) != 0)
						{
							rate.Rate_DateTime = 31;
						}
						else
						{
							rate.Rate_DateTime = DateTime.Now.Ticks - oItem.StartDate.Value.Ticks;

							if (rate.Rate_DateTime.Value < 0)
							{
								rate.Rate_DateTime = TimeSpan.FromTicks((long)Math.Abs(rate.Rate_DateTime.Value)).TotalDays;
							}
							else if (rate.Rate_DateTime > 0)
							{
								rate.Rate_DateTime = TimeSpan.FromTicks((long)rate.Rate_DateTime.Value).TotalDays;

								if (rate.Rate_DateTime.Value > 0 && rate.Rate_DateTime.Value <= 30)
								{
									rate.Rate_DateTime += 30;
								}
								else if (rate.Rate_DateTime.Value > 30)
								{
									rate.Rate_DateTime += 365;
								}
							}
						}

						//rate.Sort_DateTime = (oItem.StartDate == null || !oItem.StartDate.HasValue) ? (DateTime.Now) : (oItem.StartDate.Value);
					}
				}
				#endregion
			}
			else
			{
				#region "匹配"
				var ds = (((int)oItem.RouteType & (int)SearchDateRange.Temp) != 0) ? (uItem.StartDate.HasValue && oItem.StartDate.HasValue ? TimeSpan.FromTicks(Math.Abs(uItem.StartDate.Value.Ticks - oItem.StartDate.Value.Ticks)).TotalDays : -1) : (0.0);

				rate.Rate_DateTime = (ds != -1) ? ((1 - ds / ConfigStore.RouteMatrixServiceSettings.RouteMatrix_ExpiresInDays_Intercity) * 100) : (0.0);
				#endregion
			} 
			#endregion

			if (rate.Rate_DateTime.Value <= 0)
			{
				return rate;
			}

			#region "Rate: 地址"
			#region "省"
			if (string.Compare(uItem.From_Province, oItem.From_Province, StringComparison.InvariantCultureIgnoreCase) == 0 &&
				(string.Compare(uItem.To_Province, oItem.To_Province, StringComparison.InvariantCultureIgnoreCase) == 0 || string.IsNullOrEmpty(uItem.To_Province)))
			{
				rate.Rate_Distance += 20;
			}
			else
			{
				return rate;
			}
			#endregion

			#region "市"
			if (string.Compare(uItem.From_City, oItem.From_City, StringComparison.InvariantCultureIgnoreCase) == 0 &&
				(string.Compare(uItem.To_City, oItem.To_City, StringComparison.InvariantCultureIgnoreCase) == 0 || string.IsNullOrEmpty(uItem.To_City)))
			{
				rate.Rate_Distance += 40;
			}
			else
			{
				return rate;
			}
			#endregion

			#region "县/区"
			if (searchFilter != null && (string.IsNullOrEmpty(uItem.From_Location) || string.IsNullOrEmpty(uItem.To_Location)))
			{
				rate.Rate_Distance += (string.IsNullOrEmpty(uItem.From_Location) && string.IsNullOrEmpty(uItem.To_Location)) ? (10.0) : (5.0);
			}

			if (string.Compare(uItem.From_District, oItem.From_District, StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				rate.Rate_Distance += 5.0;
			}

			if (string.Compare(uItem.To_District, oItem.To_District, StringComparison.InvariantCultureIgnoreCase) == 0 || string.IsNullOrEmpty(uItem.To_District))
			{
				rate.Rate_Distance += 5.0;
			}
			#endregion

			#region "坐标：起点/终点"
			if (searchFilter != null && string.IsNullOrEmpty(uItem.To_Location) && string.IsNullOrEmpty(uItem.To_Location))
			{
				rate.Rate_Distance += 30.0;
			}
			else if (searchFilter != null && string.IsNullOrEmpty(uItem.From_Location))
			{
				rate.Rate_Distance += 15.0;

				var dist = Point.CalcDistance(uItem.To_Point, oItem.To_Point);
				var distTarget = Point.CalcDistance(oItem.From_Point, oItem.To_Point);
				var distRedundate = (distTarget == -1) ? (-1) : (distTarget * ConfigStore.RouteMatrixServiceSettings.RouteMatrix_MatchPerc_Intercity);

				if (dist > 0 && dist <= distRedundate && dist <= ConfigStore.RouteMatrixServiceSettings.RouteMatrix_MatchDist_Intercity)
				{
					rate.Rate_Distance += (1 - dist / distRedundate) * 15;
				}
			}
			else if (searchFilter != null && string.IsNullOrEmpty(uItem.To_Location))
			{
				rate.Rate_Distance += 15.0;

				var dist = Point.CalcDistance(uItem.From_Point, oItem.From_Point);
				var distTarget = Point.CalcDistance(oItem.From_Point, oItem.To_Point);
				var distRedundate = (distTarget == -1) ? (-1) : (distTarget * ConfigStore.RouteMatrixServiceSettings.RouteMatrix_MatchPerc_Intercity);

				if (dist > 0 && dist <= distRedundate && dist <= ConfigStore.RouteMatrixServiceSettings.RouteMatrix_MatchDist_Intercity)
				{
					rate.Rate_Distance += (1 - dist / distRedundate) * 15;
				}
			}
			else
			{
				var dist1 = Point.CalcDistance(uItem.From_Point, oItem.From_Point);
				var dist2 = Point.CalcDistance(uItem.To_Point, oItem.To_Point);
				var distTarget = Point.CalcDistance(oItem.From_Point, oItem.To_Point);
				var distRedundate = (distTarget == -1) ? (-1) : (distTarget * ConfigStore.RouteMatrixServiceSettings.RouteMatrix_MatchPerc_Intercity);

				if (dist1 > 0 && dist1 <= distRedundate && dist1 <= ConfigStore.RouteMatrixServiceSettings.RouteMatrix_MatchDist_Intercity)
				{
					rate.Rate_Distance += (1 - dist1 / distRedundate) * 15;
				}

				if (dist2 > 0 && dist2 <= distRedundate && dist2 <= ConfigStore.RouteMatrixServiceSettings.RouteMatrix_MatchDist_Intercity)
				{
					rate.Rate_Distance += (1 - dist2 / distRedundate) * 15;
				}
			}
			#endregion
			#endregion

			return rate;
		}
		#endregion

		#region "Private Methods: (Sync2DB)"
		void Sync2DB_ExpireRoute(object state)
		{
			var routeIDs = Libs.Utils.CommonUtility.GetPropertyValue<object, long[]>(state, "RouteIDs");
			var sbRouteIDs = new StringBuilder();

			try
			{
				if (routeIDs != null && routeIDs.Length > 0)
				{
					foreach (var rID in routeIDs)
					{
						sbRouteIDs.AppendFormat("{0},", rID);
					}

					using (var ctx = new DataStores.KPCDataModels())
					{
						ctx.sp_Sync_ExpireRoute(sbRouteIDs.ToString().TrimEnd(','));
					}
				}
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Queue Work Item: [{0}]\r\nParameters: [{1}]\r\nException: {2}",
					"void Sync2DB_ExpireRoute(object state)",
					string.Format("RouteIDs: {0}", sbRouteIDs.ToString().TrimEnd(',')),
					ex.Message);
			}
		}

		//void Sync2DB_MatchRoute(object state)
		//{
		//	var rm = state as RouteMatch;	
		//	var sb = new StringBuilder();

		//	try
		//	{
		//		if (rm != null)
		//		{
		//			foreach (var matrix in rm.Items)
		//			{
		//				sb.AppendFormat("{0},{1},{2}|", matrix.RouteID, matrix.Rate_DateTime, matrix.Rate_Distance);
		//			}

		//			using (var ctx = new DataStores.KPCDataModels())
		//			{
		//				ctx.sp_Sync_MatrixRoute(rm.RouteID, sb.ToString().TrimEnd('|'));
		//			}
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		DBLogger.Instance.ErrorFormat(
		//			"Queue Work Item: [{0}]\r\nParameters: [{1}]\r\nException: {2}",
		//			"void Sync2DB_MatrixRoute(object state)",
		//			string.Format("State: [{0}]", (rm != null) ? (rm.ToString()) : (string.Empty)),
		//			ex.Message);
		//	}
		//}
		#endregion

		#endregion
	}
}
