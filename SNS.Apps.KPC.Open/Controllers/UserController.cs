using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Text;
using System.Text.RegularExpressions;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.IServices;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Models.WeChat;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;

namespace SNS.Apps.KPC.Open.Controllers
{
    public class UserController : Base.BaseController
    {
        #region "Action: Login"
        public ActionResult Login()
        {
            var returnUrl = Request.QueryString["ReturnUrl"];

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                #region "已登录用户"
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

                return RedirectToAction("Search", "Route");
                #endregion
            }
            else if (this.IsWeChatBrowser)
            {
				#region "微信用户：转入授权页面"
				var authUrl = WeChat_OAuth2_Util.BuildAuthUrl(HttpUtility.UrlEncode(string.Format("{0}/account/wechatauth?returnurl={1}", ConfigStore.CommonSettings.App_Site_Url.TrimEnd('/'), HttpUtility.UrlEncode(returnUrl))), this.ExtendChannel);

				if (ConfigStore.CommonSettings.Trace_Mode)
				{
					DBLogger.Instance.InfoFormat("开始获取oAurh 2认证： {0}", authUrl);
				}

				return Redirect(authUrl); 
				#endregion
            }
            else
            {
				#region "网页用户：展示登录页面"
				var model = new UserSignInModel();

				ViewBag.ReturnUrl = returnUrl;

				if (TempData["ErrorMsg"] != null)
				{
					ViewBag.ErrorMsg = "亲，出错啦！\\n\\n" + TempData["ErrorMsg"].ToString().Replace("\r", "\\r").Replace("\n", "\\n");
				}

				Session["ValidationCode"] = null;

				return View(model); 
				#endregion
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserSignInModel model, string returnUrl)
        {
            var client = CreateServiceClient<IRepositoryService>();

            try
            {
				var errMsg = new StringBuilder();

                if (ModelState.IsValid)
                {
                    #region "Fields: Mobile"
                    if (string.IsNullOrEmpty(model.Mobile))
                    {
                        errMsg.AppendLine("手机号不能为空！");
                    }
                    #endregion

                    #region "Fields: Password"
                    if (string.IsNullOrEmpty(model.Password))
                    {
                        errMsg.AppendLine("亲，您没输入密码哦！");
                    }
                    #endregion

					#region "Fields: ValidationCode"
					if (string.IsNullOrEmpty(model.ValidationCode))
					{
						errMsg.AppendLine("亲，您没输入验证码哦！");
					}
					else if (string.Compare(Convert.ToString(Session["ValidationCode"]), model.ValidationCode, StringComparison.InvariantCultureIgnoreCase) != 0)
					{
						errMsg.AppendLine("亲，您没输入验证码不对哦！");
					}
					#endregion

					#region "开始登录"
					if (errMsg.Length == 0)
					{
						var userInstance = client.GetUserWithMobile(model.Mobile, Libs.Utils.CommonUtility.GetMd5Str32(model.Password));

						if (userInstance == null)
						{
							errMsg.AppendLine("亲，您尚未绑定手机号，轻点右下角去注册！或者你输入的密码有误，请重试！");
						}
						else
						{
							#region "写入授权 Cookie"
							//FormsAuthentication.SetAuthCookie(userInstance.UserGUID.ToString(), false);
							CustomAuthentication.SetAuthCookie(userInstance);
							#endregion

							#region "页面跳转"
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
							else
							{
								return RedirectToAction("Search", "Route");
							}
							#endregion
						}
					} 
					#endregion
                }
				else
				{
					#region "收集Model验证错误"
					foreach (var item in ModelState.Where(p => p.Value.Errors.Count > 0))
					{
						foreach (var err in item.Value.Errors)
						{
							errMsg.AppendLine(err.ErrorMessage);
						}
					} 
					#endregion
				}

				if (errMsg.Length == 0)
                {
                    errMsg.AppendLine("发生未知错误，请检查您的输入数据，并再次提交！");
                }

				TempData["ErrorMsg"] = errMsg.ToString();

                return RedirectToAction("Login", new { ReturnUrl = returnUrl });
            }
            finally
            {
                CloseServiceClient(client);
            }
        }

		[HttpPost]
		[ActionName("LoginClient")]
		public ActionResult Login(string mobile, string password)
		{
			var client = CreateServiceClient<IRepositoryService>();

			try
			{
				var errMsg = string.Empty;
				var userInstance = client.GetUserWithMobile(mobile, Libs.Utils.CommonUtility.GetMd5Str32(password));

				if (userInstance == null)
				{
					errMsg = "fail";

					return Json(new { flag = "fail" });
				}
				else
				{
					#region "写入授权 Cookie"
					//FormsAuthentication.SetAuthCookie(userInstance.UserGUID.ToString(), false);
					CustomAuthentication.SetAuthCookie(userInstance);
					#endregion

					return Json(new { flag = "success" });
				}
			}
			finally
			{
				CloseServiceClient(client);
			}
		} 
        #endregion

        #region "Action: Register"
        public ActionResult Register()
        {
            var returnUrl = Request.QueryString["ReturnUrl"];

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.CheckPassword = (!this.IsWeChatBrowser);
            ViewBag.CheckRefCode = this.IsWeChatBrowser;

            if (this.CurrentUser != null && this.CurrentUser.IsRegisterted)
            {
                #region "已注册用户"
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("EditProfile");
                }
                #endregion
            }
            else
            {
				#region "未注册用户"
				var model = default(UserRegisterModel);

				if (TempData["ErrorMsg"] != null)
				{
					ViewBag.ErrorMsg = "亲，出错啦！\\n\\n" + TempData["ErrorMsg"].ToString().Replace("\r", "\\r").Replace("\n", "\\n");

					model = (UserRegisterModel)TempData["Request"];

					model.Password = null;
					model.RefCode = null;
				}
				else if (this.CurrentUser != null)
				{
					model = new UserRegisterModel
					{
						UserGUID = this.CurrentUser.UserGUID,
						//UserRole = this.CurrentUser.UserRole,
						NickName = this.CurrentUser.NickName,
						Mobile = this.CurrentUser.Mobile,
						WeChatID = this.CurrentUser.WeChatID,
						PortraitsUrl = this.CurrentUser.PortraitsUrl,
						PortraitsThumbUrl = this.CurrentUser.PortraitsThumbUrl,
						//LicencePlateNumber = this.CurrentUser.LicencePlateNumber,

						// 推荐码
						RefID = (this.CurrentUser.RefID != null && this.CurrentUser.RefID.HasValue) ? (this.CurrentUser.RefID.Value) : (0),
						RefCode = null
					};
				}
				else if (this.IsWeChatBrowser)
				{
					return RedirectToAction("Login", new { ReturnUrl = "/user/register" });
				}
				else
				{
					model = new UserRegisterModel() { RefID = 0 };
				}

				return View(model); 
				#endregion
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(UserRegisterModel model, string returnUrl)
        {
            var clientR = CreateServiceClient<IRepositoryService>();
            var clientW = CreateServiceClient<IWeChatService>();

            try
            {
				var errMsg = new StringBuilder();

                if (ModelState.IsValid)
                {
                    #region "Field: Mobile"
                    if (string.IsNullOrEmpty(model.Mobile))
                    {
                        errMsg.AppendLine("亲，您未填手机号哦！");
                    }
                    else if (!Regex.IsMatch(model.Mobile, @"^\d{11}$"))
                    {
                        errMsg.AppendLine("亲，您的手机号填错了吧！");
                    }
                    #endregion

                    #region "Field: VerifyCode"
                    if (!string.IsNullOrEmpty(model.Mobile))
                    {
                        if (string.IsNullOrEmpty(Request.Form["VerifyCode"]))
                        {
                            errMsg.AppendLine("亲，请输入您收到的验证码！");
                        }
                        else if (!clientR.VerifyMobile(model.Mobile, Request.Form["VerifyCode"]))
                        {
                            errMsg.AppendLine("亲，您输入的验证码错误，请重试！");
                        }
                    }
                    #endregion

                    #region "Field: Password1 & Password2"
                    if (!this.IsWeChatBrowser)
                    {
                        if (string.IsNullOrEmpty(Request.Form["Password1"]))
                        {
                            errMsg.AppendLine("亲，为安全起见，推荐您设置登录密码（最大8位字符）！");
                        }
                        else if (string.Compare(Request.Form["Password1"], Request.Form["Password2"], StringComparison.InvariantCultureIgnoreCase) != 0)
                        {
                            errMsg.AppendLine("亲，您两次输入的密码不一致，请重试！");
                        }
                    }
                    #endregion

					#region "Field: RefCode"
					if (this.IsWeChatBrowser && !string.IsNullOrEmpty(model.RefCode) && !Regex.IsMatch(model.RefCode.Trim(), @"^\d{6}$"))
					{
						 errMsg.AppendLine("亲，您输入的推荐码有误哦！");
					} 
					else if (!this.IsWeChatBrowser)
					{
						model.RefCode = null;
					}
					#endregion

                    #region "开始注册流程"
                    if (errMsg.Length == 0)
                    {
                        model.IsExternal = (!this.IsWeChatBrowser);
                        model.Password = (model.IsExternal && !string.IsNullOrEmpty(Request.Form["Password1"])) ? (Libs.Utils.CommonUtility.GetMd5Str32((Request.Form["Password1"]))) : (null);

                        // 注册用户
                        var userInstance = clientR.Register(model);

                        if (userInstance != null && userInstance.IsRegisterted)
                        {
                            #region "向推荐人发送微信通知消息"
							if (this.IsWeChatBrowser)
							{
								var enableNotify = (
														(userInstance.RefID != null && userInstance.RefID.HasValue && userInstance.RefID.Value != 0) &&
														((model.RefID == null || !model.RefID.HasValue || model.RefID.Value == 0) && (!string.IsNullOrEmpty(model.RefCode)))
												   );

								if (enableNotify && clientR.CheckIsRegisterFirst(userInstance.ID))
								{
									var agent = clientR.GetPromoter(userInstance.RefID);

									if (agent != null && agent.CheckIsRef(UserPromotionType.User, model.RefCode))
									{
										clientW.SendMessage(agent.Promoter.OpenID, string.Format("{0} 您好，您推荐的用户 {1} 已成功首次注册快拼车公众账号！", agent.Promoter.NickName, userInstance.NickName), false);
									}
								}
							}
                            #endregion

							#region "写入授权 Cookie"
							//FormsAuthentication.SetAuthCookie(userInstance.UserGUID.ToString(), false);
							CustomAuthentication.SetAuthCookie(userInstance);
							#endregion

							// 是否有有效/待生效的保单
							var uiInstance = clientR.GetInsuranceOrderLatest(userInstance.ID);

							if (uiInstance != null)
							{
								// 转向 returnUrl
								return Redirect(returnUrl);
							}
							else
							{
								// 转向保险页面
								return RedirectToAction("Create", "Insurance", new { returnUrl = returnUrl });
							}
                        }
                    }
                    #endregion
                }
				else
				{
					#region "收集 Model 验证错误"
					foreach (var item in ModelState.Where(p => p.Value.Errors.Count > 0))
					{
						foreach (var err in item.Value.Errors)
						{
							errMsg.AppendLine(err.ErrorMessage);
						}
					}
					#endregion
				}

				if (errMsg.Length == 0)
				{
					errMsg.AppendLine("很抱歉，发生未知错误，您的请求未能正确提交，请稍后再试！");
				}

				#region "保存现场用户输入"
				TempData["OKMsg"] = null;
				TempData["ErrorMsg"] = errMsg.ToString();
				TempData["Request"] = model;
				#endregion

                return RedirectToAction("Register", new { ReturnUrl = returnUrl });
            }
            finally
            {
                CloseServiceClient(clientR);
                CloseServiceClient(clientW);
            }
        }
        #endregion

        #region "Action: Reset"
        public ActionResult Reset()
        {
            var model = new UserResetModel();

            ViewBag.ReturnUrl = Request.QueryString["ReturnUrl"];

            if (TempData["ErrorMsg"] != null)
            {
                ViewBag.ErrorMsg = Convert.ToString(TempData["ErrorMsg"]);

                model.Mobile = Convert.ToString(TempData["Mobile"]);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Reset(UserResetModel model, string returnUrl)
        {
            var client = CreateServiceClient<IRepositoryService>();

            try
            {
                var errMsg = string.Empty;

                if (ModelState.IsValid)
                {
                    #region "Fields: Mobile"
                    if (string.IsNullOrEmpty(model.Mobile))
                    {
                        errMsg += @"手机号不能为空！\r\n";
                    }
                    #endregion

                    #region "Field: VerifyCode"
                    if (!string.IsNullOrEmpty(model.Mobile))
                    {
                        if (string.IsNullOrEmpty(model.VerifyCode))
                        {
                            errMsg += @"亲，请输入您收到的验证码！\r\n";
                        }
                        else if (!client.VerifyMobile(model.Mobile, model.VerifyCode))
                        {
                            errMsg += @"亲，您输入的验证码错误，请重试！\r\n";
                        }
                    }
                    #endregion

                    #region "Fields: Password1 & Password2"
                    if (string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.ConfirmPassword))
                    {
                        errMsg += @"亲，您没输入密码哦！\r\n";
                    }
                    else if (string.Compare(model.Password, model.ConfirmPassword) != 0)
                    {
                        errMsg += @"亲，您两侧输入密码不一致哦！\r\n";
                    }
                    #endregion

                    #region "开始重置密码"
                    if (string.IsNullOrEmpty(errMsg))
                    {
                        if (client.ResetUserPass(model.Mobile, Libs.Utils.CommonUtility.GetMd5Str32(model.Password)))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            errMsg += @"未能重置此账号密码，您的手机号尚未绑定，首先需要注册哟！";
                        }
                    }
                    #endregion
                }

                #region "保存异常错误 & 当前数据"
                if (!string.IsNullOrEmpty(errMsg))
                {
                    TempData["ErrorMsg"] = errMsg;
                    TempData["Mobile"] = model.Mobile;
                }
                #endregion

                return RedirectToAction("Reset", new { ReturnUrl = returnUrl });
            }
            finally
            {
                CloseServiceClient(client);
            }
        }
        #endregion

        #region "Action: EditProfile"
        [Authorize]
        [HttpGet]
        public ActionResult EditProfile()
        {
			var clientR = CreateServiceClient<IRepositoryService>();

			try
			{
				var model = default(UserEditModel);

				if (TempData["ErrorMsg"] != null)
				{
					ViewBag.ErrorMsg = "亲，出错啦！\\n\\n" + TempData["ErrorMsg"].ToString().Replace("\r", "\\r").Replace("\n", "\\n");

					model = (UserEditModel)TempData["Request"];

					model.RefCode = null;
				}
				else
				{
					if (TempData["OKMsg"] != null)
					{
						ViewBag.OKMsg = TempData["OKMsg"];
					}

					model = new UserEditModel
					{
						UserGUID = this.CurrentUser.UserGUID,
						NickName = this.CurrentUser.NickName,
						Mobile = this.CurrentUser.Mobile,
						Gender = this.CurrentUser.Gender,
						WeChatID = this.CurrentUser.WeChatID,
						UserRole = this.CurrentUser.UserRole,
						LicencePlateNumber = this.CurrentUser.LicencePlateNumber,
						PortraitsUrl = this.CurrentUser.PortraitsUrl,
						PortraitsThumbUrl = this.CurrentUser.PortraitsThumbUrl,

						RefID = this.CurrentUser.RefID
					};
				}

				TempData["OKMsg"] = TempData["ErrorMsg"] = null;

				ViewBag.ReturnUrl = Request.QueryString["ReturnUrl"];
				ViewBag.TelOrigin = this.CurrentUser.Mobile;
				ViewBag.CheckRefCode = this.IsWeChatBrowser;

				// 是否有有效/待生效的保单
				var uiInstance = clientR.GetInsuranceOrderLatest(this.CurrentUser.ID);

				ViewBag.InsuranceFolio = (uiInstance != null) ? (uiInstance.Folio) : (null);
				/* End */

				return View(model);
			}
			finally
			{
				CloseServiceClient(clientR);
			}
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(UserEditModel model, string returnUrl)
        {
            var clientR = CreateServiceClient<IRepositoryService>();
            var clientW = CreateServiceClient<IWeChatService>();

            try
            {
				var errMsg = new StringBuilder();

				if (ModelState.IsValid)
				{
					#region "Verify Phone Number"
					if (string.IsNullOrEmpty(model.Mobile) || !Regex.IsMatch(model.Mobile.Trim(), @"^1[358][0-9]{9}$"))
					{
						errMsg.AppendLine("[手机]号码不正确！");
					}
					#endregion

					#region "Verify VerifyCode"
					if (Request.Form["IsModified"] == "true")
					{
						if (!clientR.VerifyMobile(model.Mobile, Request.Form["VerifyCode"]))
						{
							errMsg.AppendLine("您的手机号码未通过验证！");
						}
					}
					#endregion

					#region "Field: RefCode"
					if (this.IsWeChatBrowser && !string.IsNullOrEmpty(model.RefCode) && !Regex.IsMatch(model.RefCode.Trim(), @"^\d{6}$"))
					{
						errMsg.AppendLine("亲，您输入的推荐码有误哦！");
					}
					else if (!this.IsWeChatBrowser)
					{
						model.RefCode = null;
					}
					#endregion

					#region "开始提交数据"
					if (errMsg.Length == 0)
					{
						model.UserGUID = this.CurrentUser.UserGUID;

						var userInstance = clientR.EditProfile(model);

						if (userInstance != null)
						{
							#region "向推荐人发送微信通知消息"
							if (this.IsWeChatBrowser)
							{
								var enableNotify = (
														(userInstance.RefID != null && userInstance.RefID.HasValue && userInstance.RefID.Value != 0) &&
														((model.RefID == null || !model.RefID.HasValue || model.RefID.Value == 0) && (!string.IsNullOrEmpty(model.RefCode)))
												   );

								if (enableNotify && clientR.CheckIsRegisterFirst(userInstance.ID))
								{
									var agent = clientR.GetPromoter(userInstance.RefID);

									if (agent != null && agent.CheckIsRef(UserPromotionType.User, model.RefCode))
									{
										clientW.SendMessage(agent.Promoter.OpenID, string.Format("{0} 您好，您推荐的用户 {1} 已成功首次注册快拼车公众账号！", agent.Promoter.NickName, userInstance.NickName), false);
									}
								}
							}
							#endregion

							this.CurrentUser = userInstance;

							TempData["ErrorMsg"] = null;
							TempData["OKMsg"] = "保存成功";

							if (string.IsNullOrEmpty(returnUrl))
							{
								return RedirectToAction("EditProfile");
							}
							else
							{
								return Redirect(returnUrl);
							}
						}
					} 
					#endregion
				}
				else
				{
					#region "收集 Model 验证错误"
					foreach (var item in ModelState.Where(p => p.Value.Errors.Count > 0))
					{
						foreach (var err in item.Value.Errors)
						{
							errMsg.AppendLine(err.ErrorMessage);
						}
					} 
					#endregion
				}

				if (errMsg.Length == 0)
				{
					errMsg.AppendLine("很抱歉，发生未知错误，您的请求未能正确提交，请稍后再试！");
				}

                #region "保存现场用户输入"
                TempData["OKMsg"] = null;
				TempData["ErrorMsg"] = errMsg.ToString();
				TempData["Request"] = model;
                #endregion

				return RedirectToAction("EditProfile", new { returnUrl = returnUrl });
            }
            finally
            {
                CloseServiceClient(clientR);
                CloseServiceClient(clientW);
            }
        }
        #endregion

        #region "Action: ViewProfile"
        public ActionResult ViewProfile(Guid id)
        {
            var client = CreateServiceClient<IRepositoryService>();

            try
            {
                var user = client.GetUser(id);

                if (user == null)
                {
                    throw new Exception(string.Format("Fail to load data for User '{0}' from Database", id));
                }

                ViewBag.User = user;
                ViewBag.IsRegisterted = (this.CurrentUser != null) ? (this.CurrentUser.IsRegisterted) : (false);

                return View(client.LoadUserRouteByUserID(id));
            }
            finally
            {
                CloseServiceClient(client);
            }
        }
        #endregion

		#region "Action: MixedMobile"
		[Authorize]
		[Filters.UserAuthNoRegisterFilter]
		public ActionResult MixedMobile(Guid id)
		{
			var client = CreateServiceClient<IRepositoryService>();

			try
			{
				var user = client.GetUser(id);

				#region "Verify"
				if (user == null)
				{
					throw new Exception(string.Format("Fail to load data for User '{0}' from Database", id));
				}
				#endregion

				if (!string.IsNullOrEmpty(user.Mobile))
				{
					return File((ValidationCoder.CreateInstance()).GetImage(user.Mobile), "image/png");
				}

				return File((ValidationCoder.CreateInstance()).GetImage(new string('0', 11)), "image/png");
			}
			finally
			{
				CloseServiceClient(client);
			}
		} 
		#endregion

		#region "Action: ValidationCode"
		public ActionResult ValidationCode()
		{
			var coder = ValidationCoder.CreateInstance();
			var imgData = coder.GetImage(4);

			Session["ValidationCode"] = coder.ValidationCode;

			return File(imgData, "image/png");
		} 
		#endregion

        #region "Action: Around"
        [Authorize]
		[Filters.UserAuthNoRegisterFilter]
        public ActionResult Around()
        {
            var client = CreateServiceClient<IRepositoryService>();

            try
            {
                // Get Current User
                ViewBag.UserTrack = client.GetUserTrack(this.CurrentUser.UserGUID);
                ViewBag.UserGUID = this.CurrentUser.UserGUID;

                if (ViewBag.UserTrack == null)
                {
                    return View("Info_NoUserTrack");
                }

                return View();
            }
            finally
            {
                CloseServiceClient(client);
            }
        }

        [Authorize]
		[Filters.UserAuthNoRegisterFilter]
        public ActionResult AroundList(string role = "all")
        {
            var client = CreateServiceClient<IRepositoryService>();
            var userrole = default(Nullable<UserRole>);

            try
            {
                switch (role)
                {
                    case "passenger":
                        userrole = UserRole.Passenger;
                        break;
                    case "driver":
                        userrole = UserRole.Driver;
                        break;
                }

                // Get Current User
                ViewBag.UserTrack = client.GetUserTrack(this.CurrentUser.UserGUID);
                ViewBag.UserGUID = this.CurrentUser.UserGUID;

				var results = client.LoadUserAround(new UserTrackRequestList
				{
					Filter = new UserTrackRequest.UserTrackRequestFilter
					{
						UserID = this.CurrentUser.UserGUID,
						UserRole = userrole
					},
					Page = 0,
					Count = 10
				});

				return View(results);
            }
            finally
            {
                CloseServiceClient(client);
            }
        }
        #endregion

        #region "Action: SignIn/SignOut"
        [Authorize]
        public ActionResult SignIn()
        {
            var returnUrl = Request.QueryString["ReturnUrl"];

            return Redirect(returnUrl);
        }

        [AllowAnonymous]
        public ActionResult SignOut()
        {
			FormsAuthentication.SignOut();

            if (ConfigStore.CommonSettings.Trace_Mode)
            {
                DBLogger.Instance.InfoFormat("用户 {0} 已退出登录！", this.CurrentUser != null ? string.Format("{0} ({1}, {2})", this.CurrentUser.NickName, this.CurrentUser.ID, this.CurrentUser.UserGUID) : (string.Empty));
            }

            if (this.IsWeChatBrowser)
            {
                return Content(@"<script>alert('您已经成功退出！'); document.addEventListener('WeixinJSBridgeReady', function onBridgeReady() { WeixinJSBridge.call('closeWindow'); });</script>");
            }
            else
            {
                return Content(@"<script>alert('您已经成功退出！'); window.location.href = '/user/login';</script>");
            }
        }
        #endregion
    }
}
