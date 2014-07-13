using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Models.DataStores;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.IServices;

namespace SNS.Apps.KPC.Open.Controllers
{
	#region "Class: SearchParam"
	public class SearchParam
	{
		public string from_location { get; set; }
		public string from_province { get; set; }
		public string from_city { get; set; }
		public string from_district { get; set; }
		public string to_location { get; set; }
		public string to_province { get; set; }
		public string to_city { get; set; }
		public string to_district { get; set; }
		public string userrole { get; set; }
		public string ptype { get; set; }
		public string pcon { get; set; }
		public string pcon2 { get; set; }
		public string pdate { get; set; }
		public double from_lat { get; set; }
		public double from_lng { get; set; }
		public double to_lat { get; set; }
		public double to_lng { get; set; }
	}
	#endregion

	public class RouteController : Base.BaseController
	{
		#region "Action: City"
		[HttpGet]
		public ActionResult City()
		{
			return View();
		}
		#endregion

		#region "Action: Search"
		[HttpGet]
		public ActionResult Search()
		{
			#region "标记外部访问用户来源（平台合作）"
			var channel = (!string.IsNullOrEmpty(Request.QueryString["Channel"])) ? (Request.QueryString["Channel"].Trim()) : (null);
			
			if (!string.IsNullOrEmpty(channel) && Regex.IsMatch(channel, @"^\d{6}$"))
			{
				var clientR = CreateServiceClient<IRepositoryService>();

				try
				{
					var agent = clientR.GetPromoter(UserPromotionType.ExtendChannel, channel);

					if (agent != null)
					{
						this.ExtendChannel = channel;
					}
					else
					{
						this.ExtendChannel = "0";
					}
				}
				finally
				{
					CloseServiceClient(clientR);
				}
			}
			else
			{
				this.ExtendChannel = "0";
			}
			#endregion

			return View();
		}

		[HttpGet]
		public ActionResult SearchResult()
		{
			var client = CreateServiceClient<IRouteMatrixService>();

			try
			{
				var requestinfo = (RouteSearchRequest)Session["searchfilter"];
				var routesearchResult = client.Search(requestinfo);

				routesearchResult = (routesearchResult != null ? routesearchResult : new RouteSearchResult[] { });
				ViewBag.HasResults = true;
				
				if (routesearchResult.Count() == 0)
				{
					ViewBag.HasResults = false;
				}

				ViewBag.SearchFilter = (SearchParam)Session["searchparam"];
				
				return View(routesearchResult);
			}
			finally
			{
				CloseServiceClient(client);
			}
		}

		[HttpPost]
		public ActionResult SearchResultPost()
		{
			string from_location = Request.Form["from_location"],
				from_province = Request.Form["from_province"],
				from_city = Request.Form["from_city"],
				from_district = Request.Form["from_district"],
				to_location = Request.Form["to_location"],
				to_province = Request.Form["to_province"],
				to_city = Request.Form["to_city"],
				to_district = Request.Form["to_district"],
				userrole = Request.Form["userrole"],
				ptype = Request.Form["ptype"],
				pcon,
				pcon2,
				pdate = null;

			if (ptype == "sxb")
			{
				pcon = Request.Form["daysection"];
				pcon2 = Request.Form["pdate"];
				pdate = (pcon == ((int)SearchDateRange.Temp).ToString()) ? (Request.Form["StartDate_Date"]) : (null);
			}
			else
			{
				pcon = Request.Form["daysection"];
				pcon2 = null;
				pdate = (pcon == ((int)SearchDateRange.Temp).ToString()) ? (Request.Form["StartDate_Date"]) : (null);
			}

			int pagenum = 0;
			double from_lat = String.IsNullOrEmpty(Request.Form["from_lat"]) ? 0.0 : Convert.ToDouble(Request.Form["from_lat"]),
				from_lng = String.IsNullOrEmpty(Request.Form["from_lng"]) ? 0.0 : Convert.ToDouble(Request.Form["from_lng"]),
				to_lat = String.IsNullOrEmpty(Request.Form["to_lat"]) ? 0.0 : Convert.ToDouble(Request.Form["to_lat"]),
				to_lng = String.IsNullOrEmpty(Request.Form["to_lng"]) ? 0.0 : Convert.ToDouble(Request.Form["to_lng"]);

			Session["searchparam"] = new SearchParam
			{

				from_location = from_location,
				from_province = from_province,
				from_city = from_city,
				from_district = from_district,
				to_location = to_location,
				to_province = to_province,
				to_city = to_city,
				to_district = to_district,
				userrole = userrole,
				ptype = ptype,
				pcon = pcon,
				pcon2 = pcon2,
				pdate = pdate,
				from_lat = from_lat,
				from_lng = from_lng,
				to_lat = to_lat,
				to_lng = to_lng
			};

			var role = default(Nullable<UserRole>);

			switch (userrole)
			{
				case "passenger":
					role = UserRole.Passenger;
					break;
				case "driver":
					role = UserRole.Driver;
					break;
				case "all":
					role = null;
					break;
			}

			var requestinfo = new RouteSearchRequest
			{
				Filter = new RouteSearchFilter
				{
					LocationFilter = new RouteSearch_LocationFilter
					{
						#region "Location: From"
						From_Location = from_location,
						From_Province = from_province,
						From_City = from_city,
						From_District = from_district,

						From_Point = new Point
						{
							Longitude = from_lng,
							Latitude = from_lat
						},
						#endregion

						#region "Location: To"
						To_Location = to_location,
						To_Province = to_province,
						To_City = to_city,
						To_District = to_district,

						To_Point = new Point
						{
							Longitude = to_lng,
							Latitude = to_lat
						},
						#endregion

						RouteType = (ptype == "sxb") ? ((RouteType)Enum.Parse(typeof(RouteType), (0x10 + int.Parse(pcon)).ToString())) : ((RouteType)Enum.Parse(typeof(RouteType), (0x20 + int.Parse(pcon)).ToString()))
					},
					DateFilter = new RouteSearch_DateFilter
					{
						Date = (!string.IsNullOrEmpty(pdate)) ? (Convert.ToDateTime(pdate)) : ((DateTime?)null),
						Range = ((!string.IsNullOrEmpty(pcon)) ? (Convert.ToInt32(pcon)) : (0)) | ((!string.IsNullOrEmpty(pcon2)) ? (Convert.ToInt32(pcon2)) : (0))
					},
					UserRole = role
				},
				Page = pagenum
			};

			Session["searchfilter"] = requestinfo;

			return RedirectToAction("searchresult");
		}
		#endregion

		#region "Action: Create"
		[Authorize]
		public ActionResult Create()
		{
			var requestInfo = new RouteCreateRequest
			{
				User = this.CurrentUser,
				Route = new Route { },
				UserRole = (this.CurrentUser != null) ? (this.CurrentUser.UserRole) : UserRole.Passenger
			};

			if (TempData["ErrorMsg"] != null)
			{
				#region "Render Page From TempData"
				ViewBag.ErrorMsg = "亲，出错啦！\\n" + TempData["ErrorMsg"];

				#region "Location: From"
				requestInfo.Route.From_Province = Convert.ToString(TempData["From_Province"]);
				requestInfo.Route.From_City = Convert.ToString(TempData["From_City"]);
				requestInfo.Route.From_District = Convert.ToString(TempData["From_District"]);
				requestInfo.Route.From_Location = Convert.ToString(TempData["From_Location"]);
				requestInfo.Route.From_Point = (TempData["From_Point"] != null) ? ((Point)TempData["From_Point"]) : (new Point { Longitude = 0, Latitude = 0 });
				#endregion

				#region "Location: To"
				requestInfo.Route.To_Province = Convert.ToString(TempData["To_Province"]);
				requestInfo.Route.To_City = Convert.ToString(TempData["To_City"]);
				requestInfo.Route.To_District = Convert.ToString(TempData["To_District"]);
				requestInfo.Route.To_Location = Convert.ToString(TempData["To_Location"]);
				requestInfo.Route.To_Point = (TempData["To_Point"] != null) ? ((Point)TempData["To_Point"]) : (new Point { Longitude = 0, Latitude = 0 });
				#endregion

				#region "StartDate"
				if (TempData["StartDate_Date"] != null)
				{
					DateTime dtStart;
					DateTime dtTime;

					if (DateTime.TryParse(Convert.ToString(TempData["StartDate_Date"]), out dtStart))
					{
						ViewBag.StartDate_Date = dtStart.ToString("yyyy-MM-dd");
					}

					if (DateTime.TryParse(Convert.ToString(TempData["StartDate_Time"]), out dtTime))
					{
						ViewBag.StartDate_Time = dtTime.ToString("HH:mm");
					}
				}
				#endregion

				#region "Charge"
				var charge = 0;

				if (int.TryParse(Convert.ToString(TempData["Charge"]), out charge))
				{
					requestInfo.Route.Charge = charge;
				}
				else
				{
					requestInfo.Route.Charge = (decimal?)null;
				}
				#endregion

				#region "SeatCount"
				var seatCount = 0;

				if (int.TryParse(Convert.ToString(TempData["SeatCount"]), out seatCount))
				{
					requestInfo.Route.SeatCount = seatCount;
				}
				else
				{
					requestInfo.Route.SeatCount = (int?)null;
				}
				#endregion

				ViewBag.PincheType = Convert.ToString(TempData["pinchetype"]);
				ViewBag.IsLongTerm = Convert.ToBoolean(TempData["isLongTerm"]);
				#endregion
			}
			else
			{
				#region "Start Date"
				var dt = (DateTime.Now.Hour >= 8 && DateTime.Now.Hour < 17) ? 
					(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 0, 0)) :
					(
						(DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 8) ?
						(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0)) :
						(new DateTime(DateTime.Now.AddDays(1).Year, DateTime.Now.AddDays(1).Month, DateTime.Now.AddDays(1).Day, 8, 0, 0))
					);

				dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, (int)Math.Floor(dt.Minute / 10.0) * 10, 0);

				ViewBag.StartDate_Date = dt.ToString("yyyy-MM-dd");
				ViewBag.StartDate_Time = dt.ToString("HH:mm");
				#endregion
			}

			return View(requestInfo);
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(RouteCreateRequest requestInfo)
		{
			var client = CreateServiceClient<IRouteMatrixService>();

			try
			{
				var errorMsg = string.Empty;

				if (ModelState.IsValid)
				{
					#region "Retrieve Data from Form"

					#region "From/To Location"
					{
						if (requestInfo.Route.From_Point.Latitude == null || requestInfo.Route.From_Point.Longitude == null)
						{
							errorMsg += "【始发地】未获取正确的经纬度！\\n";
						}

						if (requestInfo.Route.To_Point.Latitude == null || requestInfo.Route.To_Point.Longitude == null)
						{
							errorMsg += "【目的地】未获取正确的经纬度！\\n";
						}
					}
					#endregion

					#region "StartDate"
					{
						switch (Request.Form["pinchetype"])
						{
							case "sxb":
								{
									#region "同城"
									switch (Request.Form["daysection"])
									{
										case "1":
										case "2":
										case "8":
											{
												var match = System.Text.RegularExpressions.Regex.Match(Request.Form["StartDate_Time"], @"^(\d{2})\:(\d{2})$");

												if (match.Success)
												{
													requestInfo.Route.StartDate = DateTime.Parse(string.Format("{0}-{1}-{2} {3}:00", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Request.Form["StartDate_Time"]));
												}
												else
												{
													errorMsg += "请输入正确的【出发时间】！\\n";
												}
											}
											break;
										case "4":
											{
												var match1 = System.Text.RegularExpressions.Regex.Match(Request.Form["StartDate_Date_Temp"], @"^(\d{4})\-(\d{2})\-(\d{2})$");
												var match2 = System.Text.RegularExpressions.Regex.Match(Request.Form["StartDate_Time"], @"^(\d{2})\:(\d{2})$");

												if (match1.Success && match2.Success)
												{
													requestInfo.Route.StartDate = DateTime.Parse(string.Format("{0} {1}:00", Request.Form["StartDate_Date_Temp"], Request.Form["StartDate_Time"]));
												}
												else
												{
													errorMsg += "请输入正确的【出发时间】！\\n";
												}
											}
											break;
									} 
									#endregion

									requestInfo.Route.RouteType = (RouteType)Enum.Parse(typeof(RouteType), (0x10 + int.Parse(Request.Form["daysection"])).ToString());
								}
								break;
							case "ct":
								{
									#region "长途"
									switch (Request.Form["daysection"])
									{
										case "1":
										case "2":
										case "8":
											break;
										case "4":
											{
												var match = System.Text.RegularExpressions.Regex.Match(Request.Form["StartDate_Date"], @"^(\d{4})\-(\d{2})\-(\d{2})$");

												if (match.Success)
												{
													requestInfo.Route.StartDate = DateTime.Parse(string.Format("{0} 00:00:00", Request.Form["StartDate_Date"]));
												}
												else
												{
													errorMsg += "请输入正确的【出发时间】！\\n";
												}
											}
											break;
									} 
									#endregion

									requestInfo.Route.RouteType = (RouteType)Enum.Parse(typeof(RouteType), (0x20 + int.Parse(Request.Form["daysection"])).ToString());
								}
								break;
						}
					}
					#endregion

					#region "Charge"
					if (!string.IsNullOrEmpty(requestInfo.Route.Charge.ToString()) && requestInfo.UserRole == UserRole.Driver)
					{
						var r = new Regex(@"\d*");

						if (!r.IsMatch(requestInfo.Route.Charge.ToString()))
						{
							errorMsg += "请输入正确的【金额】！\\n";
						}
					}
					#endregion

					#region "SeatCount"
					if (!string.IsNullOrEmpty(requestInfo.Route.SeatCount.ToString()) && requestInfo.UserRole == UserRole.Driver)
					{
						var r = new Regex(@"\d*");

						if (!r.IsMatch(requestInfo.Route.SeatCount.ToString()))
						{
							errorMsg += "请输入正确的【座位数】！\\n";
						}
					}
					#endregion

					#endregion

					if (string.IsNullOrEmpty(errorMsg))
					{
						requestInfo.User = this.CurrentUser;

						// Publish to Server
						var rID = client.Publish(requestInfo);

						if (rID.CompareTo(Guid.Empty) != 0)
						{
							return RedirectToAction("Detail", new { id = rID });
						}
					}
				}

				// Redirect to Create Page with Errors
				if (string.IsNullOrEmpty(errorMsg))
				{
					errorMsg = "很抱歉，发生未知错误，您的请求未能正确提交，请稍后再试！";
				}

				#region "Save to TempData"
				TempData["ErrorMsg"] = errorMsg;

				#region "Location: From"
				TempData["From_Province"] = requestInfo.Route.From_Province;
				TempData["From_City"] = requestInfo.Route.From_City;
				TempData["From_District"] = requestInfo.Route.From_District;
				TempData["From_Location"] = requestInfo.Route.From_Location;
				TempData["From_Point"] = requestInfo.Route.From_Point;
				#endregion

				#region "Location: To"
				TempData["To_Province"] = requestInfo.Route.To_Province;
				TempData["To_City"] = requestInfo.Route.To_City;
				TempData["To_District"] = requestInfo.Route.To_District;
				TempData["To_Location"] = requestInfo.Route.To_Location;
				TempData["To_Point"] = requestInfo.Route.To_Point;
				#endregion

				#region "StartDate"
				TempData["StartDate_Date"] = Request.Form["StartDate_Date"];
				TempData["StartDate_Time"] = Request.Form["StartDate_Time"];
				#endregion

				#region "Charge"
				TempData["Charge"] = requestInfo.Route.Charge;
				#endregion

				#region "SeatCount"
				TempData["SeatCount"] = requestInfo.Route.SeatCount;
				#endregion

				TempData["pinchetype"] = Request.Form["pinchetype"];
				TempData["isLongTerm"] = (Request.Form["pinchetype"] == "ct") ? (Convert.ToBoolean(Request.Form["isLongTerm"])) : (false);
				#endregion

				return RedirectToAction("Create");
			}
			finally
			{
				CloseServiceClient(client);
			}
		}
		#endregion

		#region "Action: Detail"
		[Authorize]
		[Filters.UserAuthNoRegisterFilter]
		public ActionResult Detail(Guid id)
		{
			var clientR = CreateServiceClient<IRepositoryService>();
			var clientM = CreateServiceClient<IRouteMatrixService>();

			try
			{
				var ur = clientR.GetUserRouteByRouteID(id);

				if (ur == null || ur.User == null)
				{
					throw new Exception("未授权的访问！");
				}

				ViewBag.IsWeChatBrowser = this.IsWeChatBrowser;
				ViewBag.IsRouteOwner = (this.CurrentUser != null && string.Compare(this.CurrentUser.UserGUID.ToString(), ur.User.UserGUID.ToString(), StringComparison.InvariantCultureIgnoreCase) == 0);

				if (HttpContext.Request.UrlReferrer == null)
				{
					ViewBag.ShowBackIcon = false;
				}
				else
				{
					ViewBag.ShowBackIcon = true;
				}

				if (!ViewBag.IsRouteOwner)
				{
					return RedirectToAction("View", new { id = ur.Route.RouteGUID });
				}

				// Route Orders
				ViewBag.RouteOrders = clientR.GetUserOrder(new UserOrderRequest
				{
					Filter = new UserOrderRequest.UserOrderRequestFilter
					{
						UserID = ur.User.UserGUID,
						RouteID = ur.Route.RouteGUID,
						OrderType = OrderType.All
					}
				}, true);

				// Route Matches
				ViewBag.RouteMatches = clientM.Search(new RouteSearchRequest
				{
					Filter = new RouteSearchFilter
					{
						LocationFilter = new RouteSearch_LocationFilter
						{
							#region "Location: From"
							From_Location = ur.Route.From_Location,
							From_Province = ur.Route.From_Province,
							From_City = ur.Route.From_City,
							From_District = ur.Route.From_District,

							From_Point = new Point
							{
								Longitude = ur.Route.From_Longitude,
								Latitude = ur.Route.From_Latitude
							},
							#endregion

							#region "Location: To"
							To_Location = ur.Route.To_Location,
							To_Province = ur.Route.To_Province,
							To_City = ur.Route.To_City,
							To_District = ur.Route.To_District,

							To_Point = new Point
							{
								Longitude = ur.Route.To_Longitude,
								Latitude = ur.Route.To_Latitude
							},
							#endregion

							RouteType = ur.Route.RouteType
						},
						DateFilter = new RouteSearch_DateFilter
						{
							Date = (ur.Route.RouteType == RouteType.Citywide_Temp || ur.Route.RouteType == RouteType.Intercity_Temp) ? ur.Route.StartDate : (DateTime?)null,
							Range = (int)SearchDateRange.EveryDay
						},
						UserID = ur.User.ID,
						UserRole = (ur.UserRole == UserRole.Driver) ? UserRole.Passenger : UserRole.Driver,
						ExcludeSelf = true
					},
					Page = 0
				});

				return View(ur);
			}
			finally
			{
				CloseServiceClient(clientR);
				CloseServiceClient(clientM);
			}
		}
		#endregion

		#region "Action: Edit"
		[Authorize]
		public ActionResult Edit(Guid id)
		{
			var client = CreateServiceClient<IRepositoryService>();

			try
			{
				var uo = client.GetUserRouteByRouteID(id, true);

				#region "Validation"
				if (uo == null)
				{
					throw new Exception(string.Format("未能在系统中找到指定的路线： {0}", id));
				}

				if (uo.User.UserGUID.CompareTo(this.CurrentUser.UserGUID) != 0)
				{
					throw new Exception(string.Format("您未授权修改此线路数据： UserID: {0}, RouteID: {1}", this.CurrentUser.UserGUID, id));
				}
				#endregion

				if (TempData["ErrorMsg"] != null)
				{
					#region "Render Page From TempData"
					ViewBag.ErrorMsg = "亲，出错啦！\\n" + TempData["ErrorMsg"];
					#endregion
				}
				else
				{
					#region "StartDate"
					if (uo.Route.StartDate != null && uo.Route.StartDate.HasValue)
					{
						ViewBag.StartDate_Date = uo.Route.StartDate.Value.ToString("yyyy-MM-dd");
						ViewBag.StartDate_Time = uo.Route.StartDate.Value.ToString("HH:mm");
					}
					else
					{
						var dt = (DateTime.Now.Hour >= 8 && DateTime.Now.Hour < 17) ?
								 (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 0, 0)) :
								 (
									(DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 8) ?
									(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0)) :
									(new DateTime(DateTime.Now.AddDays(1).Year, DateTime.Now.AddDays(1).Month, DateTime.Now.AddDays(1).Day, 8, 0, 0))
								);

						dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, (int)Math.Floor(dt.Minute / 10.0) * 10, 0);

						ViewBag.StartDate_Date = dt.ToString("yyyy-MM-dd");
						ViewBag.StartDate_Time = dt.ToString("HH:mm");
					} 
					#endregion
				}

				ViewBag.ReturnUrl = Request.QueryString["ReturnUrl"];

				return View(uo);
			}
			finally
			{
				CloseServiceClient(client);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public ActionResult Edit(UserRouteResult model, string returnUrl)
		{
			var clientR = CreateServiceClient<IRepositoryService>();
			var clientM = CreateServiceClient<IRouteMatrixService>();

			try
			{
				var errorMsg = string.Empty;

				if (ModelState.IsValid)
				{
					#region "Retrieve Data from Form"

					#region "From/To Location"
					{
						if (model.Route.From_Point.Latitude == null || model.Route.From_Point.Longitude == null)
						{
							errorMsg += "【始发地】未获取正确的经纬度！\\n";
						}

						if (model.Route.To_Point.Latitude == null || model.Route.To_Point.Longitude == null)
						{
							errorMsg += "【目的地】未获取正确的经纬度！\\n";
						}
					}
					#endregion

					#region "StartDate"
					{
						if (string.Compare(model.Route.From_City, model.Route.To_City) == 0)
						{
							#region "同城"
							switch (Request.Form["daysection"])
							{
								case "1":
								case "2":
								case "8":
									{
										var match = System.Text.RegularExpressions.Regex.Match(Request.Form["StartDate_Time"], @"^(\d{2})\:(\d{2})$");

										if (match.Success)
										{
											model.Route.StartDate = DateTime.Parse(string.Format("{0}-{1}-{2} {3}:00", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Request.Form["StartDate_Time"]));
										}
										else
										{
											errorMsg += "请输入正确的【出发时间】！\\n";
										}
									}
									break;
								case "4":
									{
										var match1 = System.Text.RegularExpressions.Regex.Match(Request.Form["StartDate_Date_Temp"], @"^(\d{4})\-(\d{2})\-(\d{2})$");
										var match2 = System.Text.RegularExpressions.Regex.Match(Request.Form["StartDate_Time"], @"^(\d{2})\:(\d{2})$");

										if (match1.Success && match2.Success)
										{
											model.Route.StartDate = DateTime.Parse(string.Format("{0} {1}:00", Request.Form["StartDate_Date_Temp"], Request.Form["StartDate_Time"]));
										}
										else
										{
											errorMsg += "请输入正确的【出发时间】！\\n";
										}
									}
									break;
							}
							#endregion

							model.Route.RouteType = (RouteType)Enum.Parse(typeof(RouteType), (0x10 + int.Parse(Request.Form["daysection"])).ToString());
						}
						else
						{
							#region "长途"
							switch (Request.Form["daysection"])
							{
								case "1":
								case "2":
								case "8":
									break;
								case "4":
									{
										var match = System.Text.RegularExpressions.Regex.Match(Request.Form["StartDate_Date"], @"^(\d{4})\-(\d{2})\-(\d{2})$");

										if (match.Success)
										{
											model.Route.StartDate = DateTime.Parse(string.Format("{0} 00:00:00", Request.Form["StartDate_Date"]));
										}
										else
										{
											errorMsg += "请输入正确的【出发时间】！\\n";
										}
									}
									break;
							}
							#endregion

							model.Route.RouteType = (RouteType)Enum.Parse(typeof(RouteType), (0x20 + int.Parse(Request.Form["daysection"])).ToString());
						}
					}
					#endregion

					#region "Charge"
					if (!string.IsNullOrEmpty(model.Route.Charge.ToString()) && model.UserRole == UserRole.Driver)
					{
						var r = new Regex(@"\d*");

						if (!r.IsMatch(model.Route.Charge.ToString()))
						{
							errorMsg += "请输入正确的【金额】！\\n";
						}
					}
					#endregion

					#region "SeatCount"
					if (!string.IsNullOrEmpty(model.Route.SeatCount.ToString()) && model.UserRole == UserRole.Driver)
					{
						var r = new Regex(@"\d*");

						if (!r.IsMatch(model.Route.SeatCount.ToString()))
						{
							errorMsg += "请输入正确的【座位数】！\\n";
						}
					}
					#endregion

					#endregion

					if (string.IsNullOrEmpty(errorMsg))
					{
						var ur = clientR.GetUserRouteByRouteID(model.Route.RouteGUID, true);
						var requestInfo = new RouteCreateRequest
						{
							Route = ur.Route,
							User = ur.User,
							UserRole = model.UserRole
						};

						// Apply Changes
						// From Location
						requestInfo.Route.From_Province = model.Route.From_Province;
						requestInfo.Route.From_City = model.Route.From_City;
						requestInfo.Route.From_District = model.Route.From_District;
						requestInfo.Route.From_Location = model.Route.From_Location;
						requestInfo.Route.From_Point = model.Route.From_Point;

						// To Location
						requestInfo.Route.To_Province = model.Route.To_Province;
						requestInfo.Route.To_City = model.Route.To_City;
						requestInfo.Route.To_District = model.Route.To_District;
						requestInfo.Route.To_Location = model.Route.To_Location;
						requestInfo.Route.To_Point = model.Route.To_Point;

						// Other Info
						requestInfo.Route.RouteType = model.Route.RouteType;
						requestInfo.Route.StartDate = model.Route.StartDate;
						requestInfo.Route.Charge = model.Route.Charge;
						requestInfo.Route.SeatCount = model.Route.SeatCount;
						requestInfo.Route.Note = model.Route.Note;

						// Publish to Server
						var rID = clientM.Publish(requestInfo);

						if (rID.CompareTo(Guid.Empty) != 0)
						{
							if (!string.IsNullOrEmpty(returnUrl))
							{
								return Redirect(returnUrl);
							}
							else
							{
								return RedirectToAction("Detail", new { id = model.Route.RouteGUID });
							}
						}
					}
				}

				if (string.IsNullOrEmpty(errorMsg))
				{
					errorMsg += "发生未知错误，请检查您的输入数据，并再次提交！";
				}

				#region "保存异常错误 & 当前数据"
				TempData["ErrorMsg"] = errorMsg;
				#endregion

				return RedirectToAction("Edit", new { id = model.Route.RouteGUID, ReturnUrl = returnUrl });
			}
			finally
			{
				CloseServiceClient(clientR);
				CloseServiceClient(clientM);
			}
		}
		#endregion

		#region "Action: List"
		[Authorize]
		public ActionResult List()
		{
			var client = CreateServiceClient<IRepositoryService>();

			try
			{
				var urs = client.LoadUserRouteByUserID(this.CurrentUser.UserGUID, 0);

				ViewBag.UserGUID = this.CurrentUser.UserGUID.ToString();

				return View(urs);
			}
			finally
			{
				CloseServiceClient(client);
			}
		}

		[HttpPost]
		[Authorize]
		public ActionResult ListMore(int pageNum)
		{
			var client = CreateServiceClient<IRepositoryService>();

			try
			{
				var urs = client.LoadUserRouteByUserID(this.CurrentUser.UserGUID, pageNum);

				bool flag = true;
				if (urs == null || urs.Count() == 0)
				{
					flag = false;
				}
				return Json(new { flag = flag, dataList = urs }, JsonRequestBehavior.DenyGet);
			}
			finally
			{
				CloseServiceClient(client);
			}
		}
		#endregion

		#region "Action: Latest"
		[Authorize]
		public ActionResult Latest()
		{
			var client = CreateServiceClient<IRepositoryService>();

			try
			{
				var ur = client.GetUserRouteNewest(this.CurrentUser.UserGUID, true);

				if (ur != null)
				{
					return RedirectToAction("Detail", new { id = ur.Route.RouteGUID });
				}

				return View("Info_NoRoute");
			}
			finally
			{
				CloseServiceClient(client);
			}
		}
		#endregion

		#region "Action: View"
		//[Authorize]
		//[Filters.UserAuthNoRegisterFilter]
		public ActionResult View(Guid id, float from_lng = 0.0f, float from_lat = 0.0f, float to_lng = 0.0f, float to_lat = 0.0f)
		{
			var client = CreateServiceClient<IRepositoryService>();

			try
			{
				if (HttpContext.Request.UrlReferrer == null)
				{
					ViewBag.ShowBackIcon = false;
				}
				else
				{
					ViewBag.ShowBackIcon = true;
				}

				ViewBag.FromPoint = new Point
				{
					Longitude = from_lng,
					Latitude = from_lat
				};

				ViewBag.ToPoint = new Point
				{
					Longitude = to_lng,
					Latitude = to_lat
				};

				var ur = client.GetUserRouteByRouteID(id);

				if (ur == null)
				{
					throw new Exception(string.Format("无效的路线ID：{0}", id));
				}

				ViewBag.IsRegisterted = (this.CurrentUser != null) ? (this.CurrentUser.IsRegisterted) : (false);
				ViewBag.IsRouteOwner = (this.CurrentUser != null && string.Compare(this.CurrentUser.UserGUID.ToString(), ur.User.UserGUID.ToString(), StringComparison.InvariantCultureIgnoreCase) == 0);
				ViewBag.IsWeChatBrowser = this.IsWeChatBrowser;

				return View(ur);
			}
			finally
			{
				CloseServiceClient(client);
			}
		}
		#endregion

		#region "Action: Plaza"
		[HttpGet]
		[AllowAnonymous]
		public ActionResult Plaza(string currentcity)
		{
			var client = CreateServiceClient<IRepositoryService>();

			try
			{
				var userID = (this.CurrentUser != null) ? ((int?)this.CurrentUser.ID) : ((int?)null);
				var userTrack = (userID != null && userID.HasValue) ? (client.GetUserTrack(userID.Value)) : (null);
				var city = string.Empty;
				if (currentcity == null)
				{
					#region "获取当前所在城市"
					if (userTrack != null)
					{
						if (userTrack.Route != null)
						{
							city = userTrack.Route.From_City;
						}
						else if (userTrack.Position != null && userTrack.Position.HasValue)
						{
							var loc = Location.Reverse(userTrack.Position.Value);

							if (loc != null)
							{
								city = loc.City;
							}
						}
					}
					#endregion
				}
				else
				{
					city = currentcity;
				}
				ViewBag.City = city;

				var results = client.LoadUserRouteNewest(new RouteSearchRequest { Filter = new RouteSearchFilter { LocationFilter = new RouteSearch_LocationFilter { From_City = city } }, Page = 0, Count = 10 });

				return View(results);
			}
			finally
			{
				CloseServiceClient(client);
			}
		}
		#endregion
	}
}
