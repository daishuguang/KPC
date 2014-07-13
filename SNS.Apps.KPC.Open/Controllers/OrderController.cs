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
    public class OrderController : Base.BaseController
    {
        #region "Action: Create"
        [Authorize]
        public ActionResult Create(Guid id)
        {
            var client = CreateServiceClient<IRepositoryService>();

            try
            {
				var routeIndent = client.GetUserRouteByRouteID(id);

				#region "Verify"
				if (routeIndent == null)
				{
					throw new Exception(string.Format("无效的参数：id：{0}", id));
				} 
				#endregion

				#region "路线Owner，转到View页面"
				if (routeIndent.User.UserGUID.CompareTo(this.CurrentUser.UserGUID) == 0)
				{
					return RedirectToAction("View", "Route", new { id = id });
				}
				#endregion

                if (TempData["ErrorMsg"] != null)
                {
                    ViewBag.ErrorMsg = Convert.ToString(TempData["ErrorMsg"]);
                }

				if (TempData["OKMsg"] != null)
				{
					ViewBag.OKMsg = Convert.ToString(TempData["OKMsg"]);
				}

				TempData["OKMsg"] = TempData["ErrorMsg"] = null;

                ViewBag.CurrentUser = this.CurrentUser;
				ViewBag.IsRegisterted = (this.CurrentUser != null) ? (this.CurrentUser.IsRegisterted) : (false);
                
				var dtStart = DateTime.Now.AddHours(1);

                ViewBag.StartDate_Date = dtStart.ToString("yyyy-MM-dd");
                ViewBag.StartDate_Time = string.Format("{0:D2}:{1:D2}", dtStart.Hour, (10 * (dtStart.Minute / 10)));

				// 是否有有效/待生效的保险
				var ui = client.GetInsuranceOrderLatest(this.CurrentUser.ID);

				ViewBag.InsuranceFolio = (ui != null) ? (ui.Folio) : (null);
				/* End */

                return View(routeIndent);
            }
            finally
            {
                CloseServiceClient(client);
            }
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
		public ActionResult Create(UserRouteResult orderInfo)
        {
            var clientR = CreateServiceClient<IRepositoryService>();
            var clientW = CreateServiceClient<IWeChatService>();

            try
            {
                var errMsg = string.Empty;
                var requestInfo = new UserOrderCreateRequest
                {
                    RequestorID = this.CurrentUser.UserGUID,
                    RequestorRole = (orderInfo.UserRole == UserRole.Driver) ? (UserRole.Passenger) : (UserRole.Driver),

                    SupplierID = orderInfo.User.UserGUID,
                    SupplierRole = orderInfo.UserRole,
                    RouteID = orderInfo.Route.RouteGUID,

                    Note = (!string.IsNullOrEmpty(Request.Form["Order_Note"].Trim())) ? (Request.Form["Order_Note"].Trim()) : (null)
                };

                #region "Charge"
                if (requestInfo.RequestorRole == UserRole.Driver)
                {
                    if (!string.IsNullOrEmpty(Request.Form["Order_Charge"]))
                    {
                        var charge = default(decimal);

                        if (decimal.TryParse(Request.Form["Order_Charge"], out charge))
                        {
                            requestInfo.Charge = charge;
                        }
                        else
                        {
                            errMsg += "【拼车费用】输入错误！\r\n";
                        }
                    }
                }
                else
                {
                    requestInfo.Charge = orderInfo.Route.Charge;
                }
                #endregion

                #region "StartDate"
                if (!string.IsNullOrEmpty(Request.Form["StartDate_Date"]) && !string.IsNullOrEmpty(Request.Form["StartDate_Time"]))
                {
                    var startDate = default(DateTime);

                    if (DateTime.TryParse(string.Format("{0} {1}:00", Request.Form["StartDate_Date"], Request.Form["StartDate_Time"]), out startDate))
                    {
                        requestInfo.StartDate = startDate;
                    }
                    else
                    {
                        errMsg += "【发车时间】输入错误！\r\n";
                    }
                }
                #endregion

                if (string.IsNullOrEmpty(errMsg))
                {
                    var folio = clientR.CreateUserOrder(requestInfo);

                    if (!string.IsNullOrEmpty(folio))
                    {
                        var uo = clientR.GetUserOrder(folio);

                        // 发送短信/微信通知对方
                        //if (!string.IsNullOrEmpty(uo.Supplier.Mobile))
						if (false)
                        {
							var isCitywide = (((int)uo.Route.RouteType &0x10) != 0);
							var route_desc_from = (isCitywide ? string.Format("{0}{1}", uo.Route.From_District, uo.Route.From_Location) : string.Format("{0}{1}", uo.Route.From_City, uo.Route.From_Location));
							var route_desc_to = (isCitywide ? string.Format("{0}{1}", uo.Route.To_District, uo.Route.To_Location) : string.Format("{0}{1}", uo.Route.To_City, uo.Route.To_Location));

							// 尝试发送微信
							if (!clientW.SendMessage(
								uo.Supplier.OpenID,
								string.Format(@"{0}，您好！快拼车已经帮您找到从 {1} 到 {2} 的拼友。详情请进入微信快拼车，个人中心->我的拼单查看，或者点击拼单号：<a href=""{3}/order/detail/{4}"">{4}</a>直接进入拼单详情。", uo.Supplier.NickName, route_desc_from, route_desc_to, ConfigStore.CommonSettings.App_Site_Url, uo.Folio),
								false
							))
							{
								#region "发送短信"
								var clientS = CreateServiceClient<ISMSService>();

								try
								{
									route_desc_from = (isCitywide ? string.Format("{0}", (!string.IsNullOrEmpty(uo.Route.From_Location) ? uo.Route.From_Location : uo.Route.From_District)) : uo.Route.From_City);
									route_desc_to = (isCitywide ? string.Format("{0}", (!string.IsNullOrEmpty(uo.Route.To_Location) ? uo.Route.To_Location : uo.Route.To_District)) : uo.Route.To_City);

									clientS.SendSMS_Notification(new Libs.Models.SMS.SMSMessageSendRequest()
									{
										Channel = 0,
										Mobiles = new string[] { uo.Supplier.Mobile },
										Content = string.Format("{0}，您好！快拼车已经帮您找到从 {1} 到 {2} 的拼友。详情请进入微信快拼车，个人中心->我的拼单查看。", uo.Supplier.NickName, route_desc_from, route_desc_to)
									});
								}
								finally
								{
									CloseServiceClient(clientS);
								} 
								#endregion
							}
                        }

						#region "构造 ReturnUrl"
						var returnUrl = string.Empty;

						if (requestInfo.RequestorRole == UserRole.Passenger)
						{
							var payMethod = (PaymentMethod)Enum.Parse(typeof(PaymentMethod), Request.Form["pay_ways"]);

							if (payMethod != PaymentMethod.DirectPay)
							{
								if (!string.IsNullOrEmpty(Request.Form["cash_ways"]))
								{
									var cashMethod = (PaymentMethod)Enum.Parse(typeof(PaymentMethod), Request.Form["cash_ways"]);

									returnUrl = string.Format("/payment/pay/{0}/{1}", folio, ((int)payMethod | (int)cashMethod));
								}
							}
							else
							{
								returnUrl = string.Format("/payment/pay/{0}/{1}", folio, (int)payMethod);
							}
						}
						else
						{
							returnUrl = string.Format("/order/detail/{0}", folio);
						}
						#endregion

						if (string.IsNullOrEmpty(Request.Form["InsuranceFolio"]))
						{
							// 转向保险页面
							return RedirectToAction("Create", "Insurance", new { returnUrl = returnUrl });
						}
						else
						{
							// 转向拼单详情/支付页面
							return Redirect(returnUrl);
						}
                    }

                    errMsg += "发生未知错误，未能成功创建您的拼单，请稍后重试！";
                }

                TempData["ErrorMsg"] = errMsg;

                return RedirectToAction("Create", new { id = orderInfo.Route.RouteGUID });
            }
            finally
            {
                CloseServiceClient(clientR);
                CloseServiceClient(clientW);
            }
        }
        #endregion

        public ActionResult ToDetail(string folio)
        {
            ViewBag.URL = folio;
            return View();
        }

        #region "Action: Detail"
        [Authorize]
        public ActionResult Detail(string folio)
        {
            var clientR = CreateServiceClient<IRepositoryService>();

            try
            {
                #region "验证参数"
                if (string.IsNullOrEmpty(folio))
                {
                    throw new ArgumentNullException("未指定参数： Folio");
                }

                var uo = clientR.GetUserOrder(folio, true);

                if (uo == null)
                {
                    throw new Exception(string.Format("未能找到 Folio 为 '{0}' 的订单！", folio));
                }

				if (!this.CurrentUser.UserGUID.Equals(uo.Requestor.UserGUID) && !this.CurrentUser.UserGUID.Equals(uo.Supplier.UserGUID))
				{
					throw new UnauthorizedAccessException("您未授权访问该拼单信息！");
				}
                #endregion

                ViewBag.CurrentUserID = this.CurrentUser.UserGUID;
                ViewBag.IsRegisterted = this.CurrentUser.IsRegisterted;
                ViewBag.IsRequestor = this.CurrentUser.UserGUID.Equals(uo.Requestor.UserGUID);
                ViewBag.IsSupplier = this.CurrentUser.UserGUID.Equals(uo.Supplier.UserGUID);
                ViewBag.IsDateStarted = (DateTime.Now.CompareTo(uo.StartDate.Value) >= 0);
                ViewBag.IsDateClosed = (DateTime.Now.CompareTo(uo.StartDate.Value.AddDays(7)) >= 0);

				if ((int)uo.Status >= (int)OrderStatus.PendingForPayment)
				{
					var up = clientR.GetPayment(uo.ID);

					if (up != null)
					{
						ViewBag.IsPaied = (up.PayStatus == PaymentStatus.Success);
						ViewBag.PayMethod = up.PayMethod;

						if (((int)up.PayMethod & (int)PaymentMethod.OnlinPay_Deposit) != 0)
						{
							ViewBag.Amount_Deposit = (up.PayAmount.HasValue) ? (up.PayAmount.Value.ToString("C2")) : (0M).ToString("C2");
							ViewBag.Amount_Residue = (uo.Charge.HasValue && uo.Charge.Value != 0) ? ((uo.Charge.Value - (up.PayAmount.HasValue ? up.PayAmount.Value : 0)).ToString("C2")) : (string.Format("面议金额 - {0}", (up.PayAmount.HasValue ? up.PayAmount.Value.ToString("C2") : (0M).ToString("C2"))));
						}
						//else if (((int)up.PayMethod & (int)PaymentMethod.OnlinPay_FullCash) != 0)
						//{
						//	ViewBag.Amount_Deposit = up.PayAmount.Value.ToString("C2");
						//	ViewBag.Amount_Residue = string.Empty;
						//}
					}
					else
					{
						ViewBag.IsPaied = false;
						ViewBag.PayMethod = PaymentMethod.DirectPay;
					}
				}

				#region "获取当前订单的保单信息"
				var uoi = clientR.GetInsuranceOrder(this.CurrentUser.ID, uo.ID);

				if (uoi != null)
				{
					ViewBag.ShowInsuranceLink = true;
					ViewBag.InsuranceFolio = uoi.Folio;
				}
				else
				{
					var ui = clientR.GetInsuranceOrderLatest(this.CurrentUser.ID);

					// 无生效/待生效的保单： 我要参保
					if (ui == null)
					{
						ViewBag.ShowInsuranceLink = true;
						ViewBag.InsuranceFolio = null;
					}
					// 有生效的保单，但不适用于本次拼单： 无任何展示
					else
					{
						ViewBag.ShowInsuranceLink = false;
						ViewBag.InsuranceFolio = null;
					}
				} 
				#endregion

				#region "页面提示信息"
				if (TempData["ErrorMsg"] != null)
				{
					ViewBag.ErrorMsg = Convert.ToString(TempData["ErrorMsg"]);
				}

				if (TempData["OKMsg"] != null)
				{
					ViewBag.ErrorMsg = Convert.ToString(TempData["OKMsg"]);
				}

				TempData["OKMsg"] = TempData["ErrorMsg"] = null; 
				#endregion

                return View(uo);
            }
            finally
            {
                CloseServiceClient(clientR);
            }
        }
        #endregion

		#region "Action: Edit"
		[Authorize]
		public ActionResult Edit(string folio)
		{
			var client = CreateServiceClient<IRepositoryService>();

			try
			{
				#region "验证参数"
				if (string.IsNullOrEmpty(folio))
				{
					throw new ArgumentNullException("未指定参数： Folio");
				}

				var uo = client.GetUserOrder(folio, true);

				if (uo == null)
				{
					throw new Exception(string.Format("未能找到 Folio 为 '{0}' 的订单！", folio));
				}

				if (!this.CurrentUser.UserGUID.Equals(uo.Requestor.UserGUID) && !this.CurrentUser.UserGUID.Equals(uo.Supplier.UserGUID))
				{
					throw new UnauthorizedAccessException("您未授权访问该拼单信息！");
				}
				#endregion

				#region "验证状态、角色"
				if (uo.Status != OrderStatus.PendingForPayment)
				{
					return RedirectToAction("Detail", new { folio = folio });
				}

				if ((this.CurrentUser.UserGUID.Equals(uo.Requestor.UserGUID) && uo.RequestorRole != UserRole.Driver) ||
					(this.CurrentUser.UserGUID.Equals(uo.Supplier.UserGUID) && uo.SupplierRole != UserRole.Driver))
				{
					return RedirectToAction("Detail", new { folio = folio });
				}
				#endregion

				ViewBag.ReturnUrl = Request.QueryString["ReturnUrl"];
				ViewBag.CurrentUserID = this.CurrentUser.UserGUID;
				ViewBag.IsRegisterted = this.CurrentUser.IsRegisterted;
				ViewBag.IsRequestor = this.CurrentUser.UserGUID.Equals(uo.Requestor.UserGUID);
				ViewBag.IsSupplier = this.CurrentUser.UserGUID.Equals(uo.Supplier.UserGUID);
				ViewBag.IsDateStarted = (DateTime.Now.CompareTo(uo.StartDate.Value) >= 0);
				ViewBag.IsDateClosed = (DateTime.Now.CompareTo(uo.StartDate.Value.AddDays(7)) >= 0);

				if (TempData["ErrorMsg"] != null)
				{
					ViewBag.ErrorMsg = Convert.ToString(TempData["ErrorMsg"]);
				}

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
		public ActionResult Edit(UserOrderResult updateInfo, string folio, string returnUrl)
		{
			var client = CreateServiceClient<IRepositoryService>();

			try
			{
				#region "验证参数"
				if (string.IsNullOrEmpty(folio))
				{
					throw new ArgumentNullException("未指定参数： Folio");
				}

				var uo = client.GetUserOrder(folio, true);

				if (uo == null)
				{
					throw new Exception(string.Format("未能找到 Folio 为 '{0}' 的订单！", folio));
				}

				if (!this.CurrentUser.UserGUID.Equals(uo.Requestor.UserGUID) && !this.CurrentUser.UserGUID.Equals(uo.Supplier.UserGUID))
				{
					throw new UnauthorizedAccessException("您未授权访问该拼单信息！");
				}
				#endregion

				#region "验证状态、角色"
				if (uo.Status != OrderStatus.PendingForPayment)
				{
					return RedirectToAction("Detail", new { folio = folio });
				}

				if ((this.CurrentUser.UserGUID.Equals(uo.Requestor.UserGUID) && uo.RequestorRole != UserRole.Driver) ||
					(this.CurrentUser.UserGUID.Equals(uo.Supplier.UserGUID) && uo.SupplierRole != UserRole.Driver))
				{
					return RedirectToAction("Detail", new { folio = folio });
				}
				#endregion

				ViewBag.CurrentUserID = this.CurrentUser.UserGUID;
				ViewBag.IsRegisterted = this.CurrentUser.IsRegisterted;
				ViewBag.IsRequestor = this.CurrentUser.UserGUID.Equals(uo.Requestor.UserGUID);
				ViewBag.IsSupplier = this.CurrentUser.UserGUID.Equals(uo.Supplier.UserGUID);
				ViewBag.IsDateStarted = (DateTime.Now.CompareTo(uo.StartDate.Value) >= 0);
				ViewBag.IsDateClosed = (DateTime.Now.CompareTo(uo.StartDate.Value.AddDays(7)) >= 0);

				var errorMsg = string.Empty;

				if (ModelState.IsValid)
				{
					var requestInfo = new UserOrderUpdateRequest(folio);

					#region "StartDate"
					if (!string.IsNullOrEmpty(Request.Form["StartDate_Date"]) && !string.IsNullOrEmpty(Request.Form["StartDate_Time"]))
					{
						var dtStart = DateTime.MinValue;

						if (DateTime.TryParse(string.Format("{0} {1}:00", Request.Form["StartDate_Date"], Request.Form["StartDate_Time"]), out dtStart))
						{
							requestInfo.StartDate = dtStart;
						}
						else
						{
							errorMsg += @"【发车时间】输入错误，请核对数据后再重试！";
						}
					}
					else
					{
						errorMsg += @"【发车时间】是必填项哦！";
					}
					#endregion

					requestInfo.Charge = updateInfo.Charge;
					requestInfo.Note = updateInfo.Note;

					if (string.IsNullOrEmpty(errorMsg))
					{
						if (client.UpdateUserOrder(requestInfo))
						{
							if (!string.IsNullOrEmpty(returnUrl))
							{
								if (Url.IsLocalUrl(returnUrl))
								{
									return Redirect(returnUrl);
								}
								else
								{
									Response.Redirect(returnUrl);

									return null;
								}
							}

							return RedirectToAction("Detail", new { folio = folio });
						}
					}
				}

				if (string.IsNullOrEmpty(errorMsg))
				{
					errorMsg += @"发生异常错误，未能提交您的变更，请重试！";
				}

				if (!string.IsNullOrEmpty(errorMsg))
				{
					TempData["ErrorMsg"] = errorMsg;
				}

				return RedirectToAction("Edit", new { folio = folio, returnUrl = returnUrl });
			}
			finally
			{
				CloseServiceClient(client);
			}
		}
		#endregion

		#region "Action: Cancel"
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public ActionResult Cancel(string folio)
		{
			var clientR = CreateServiceClient<IRepositoryService>();
			var clientW = CreateServiceClient<IWeChatService>();

			try
			{
				#region "验证参数"
				if (string.IsNullOrEmpty(folio))
				{
					throw new ArgumentNullException("未指定参数： Folio");
				}

				var uo = clientR.GetUserOrder(folio, true);
				
				if (uo == null)
				{
					throw new Exception(string.Format("未能找到 Folio 为 '{0}' 的订单！", folio));
				}

				var isRequestor = this.CurrentUser.UserGUID.Equals(uo.Requestor.UserGUID);
				var isSupplier = this.CurrentUser.UserGUID.Equals(uo.Supplier.UserGUID);

				if (!isRequestor && !isSupplier)
				{
					throw new UnauthorizedAccessException("您未授权访问该拼单信息！");
				}
				#endregion

				#region "验证状态、角色"
				if (uo.Status != OrderStatus.PendingForPayment)
				{
					return RedirectToAction("Detail", new { folio = folio });
				}
				#endregion

				if (clientR.CancelUserOrder(this.CurrentUser.UserGUID, folio))
				{
					//clientS.SendSMS_Notification(new Libs.Models.SMS.SMSMessageSendRequest 
					//{
					//	Channel = 0,
					//	Mobiles = new string[] { (isRequestor) ? (uo.Supplier.Mobile) : (uo.Requestor.Mobile) },
					//	Content = string.Format("很抱歉，用户 {0} 已经取消了此拼单，也许下次你们会有拼在一起的机会哦~", this.CurrentUser.NickName)
					//});

					clientW.SendMessage(
						(isRequestor) ? (uo.Supplier.OpenID) : (uo.Requestor.OpenID),
						string.Format("很抱歉，用户 {0} 已经取消了拼单：{1}，也许下次你们会有拼在一起的机会哦~", this.CurrentUser.NickName, uo.Folio)
					);
				}
				else
				{
					TempData["ErrorMsg"] = @"很抱歉，发生异常错误，您的操作未能执行，请稍后重试！";
				}

				return RedirectToAction("Detail", new { folio = folio });
			}
			finally
			{
				CloseServiceClient(clientR);
				CloseServiceClient(clientW);
			}
		} 
		#endregion

		#region "Action: Confirm"
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public ActionResult Confirm(string folio)
		{
			var clientR = CreateServiceClient<IRepositoryService>();
			var clientW = CreateServiceClient<IWeChatService>();

			try
			{
				#region "验证参数"
				if (string.IsNullOrEmpty(folio))
				{
					throw new ArgumentNullException("未指定参数： Folio");
				}

				var uo = clientR.GetUserOrder(folio, true);

				if (uo == null)
				{
					throw new Exception(string.Format("未能找到 Folio 为 '{0}' 的订单！", folio));
				}
				#endregion

				#region "验证角色，状态"
				var isRequestor = this.CurrentUser.UserGUID.Equals(uo.Requestor.UserGUID);
				var isSupplier = this.CurrentUser.UserGUID.Equals(uo.Supplier.UserGUID);

				if (!isRequestor && !isSupplier)
				{
					throw new UnauthorizedAccessException("您未授权访问该拼单信息！");
				}

				if (!((int)uo.Status > (int)OrderStatus.PendingForPayment && (int)uo.Status < (int)OrderStatus.Completed))
				{
					return RedirectToAction("Detail", new { folio = folio });
				}
				#endregion

				if (clientR.ConfirmUserOrder(this.CurrentUser.UserGUID, folio))
				{
					//clientS.SendSMS_Notification(new Libs.Models.SMS.SMSMessageSendRequest
					//{
					//	Channel = 0,
					//	Mobiles = new string[] { (isRequestor) ? (uo.Requestor.Mobile) : (uo.Supplier.Mobile) },
					//	Content = string.Format("感谢您的此次拼车体验，有任何意见或者建议请及时反馈给快拼车。期待您的再次惠顾，谢谢！", this.CurrentUser.NickName)
					//});

					clientW.SendMessage(
						(isRequestor) ? (uo.Requestor.OpenID) : (uo.Supplier.OpenID),
						string.Format("感谢您的此次拼车体验，有任何意见或者建议请及时反馈给快拼车。期待您的再次惠顾，谢谢！", this.CurrentUser.NickName)
					);
				}
				else
				{
					TempData["ErrorMsg"] = @"很抱歉，发生异常错误，您的操作未能执行，请稍后重试！";
				}

				return RedirectToAction("Detail", new { folio = folio });
			}
			finally
			{
				CloseServiceClient(clientR);
				CloseServiceClient(clientW);
			}
		}
		#endregion

        #region "Action: List"
        [Authorize]
        public ActionResult List(int orderType = (int)OrderType.All)
        {
            ViewBag.OrderType = orderType;
			ViewBag.CurrentUserID = this.CurrentUser.UserGUID;

            return View();
        }
        #endregion
    }
}