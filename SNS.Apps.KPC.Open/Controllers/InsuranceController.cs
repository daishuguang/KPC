using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Text;
using System.Text.RegularExpressions;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.IServices;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Utils;

namespace SNS.Apps.KPC.Open.Controllers
{
    public class InsuranceController : Base.BaseController
    {
        #region "Action: Create"
        [Authorize]
        public ActionResult Create()
        {
            var clientR = CreateServiceClient<IRepositoryService>();

            try
            {
                ViewBag.ReturnUrl = Request.QueryString["ReturnUrl"];
                
                #region "已有保单时转向保单详情页面"
                var uiInstance = clientR.GetInsuranceOrderLatest(this.CurrentUser.ID);

                if (uiInstance != null)
                {
                    return RedirectToAction("Detail", "Insurance", new { folio = uiInstance.Folio, returnUrl = ViewBag.ReturnUrl });
                }
                #endregion

                var model = default(InsuranceSubmitModel);

                if (TempData["ErrorMsg"] != null)
                {
                    ViewBag.ErrorMsg = "亲，出错啦！\\n\\n" + TempData["ErrorMsg"].ToString().Replace("\r", "\\r").Replace("\n", "\\n");

                    model = (InsuranceSubmitModel)TempData["Request"];
                }
                else
                {
                    uiInstance = clientR.GetInsuranceOrderLatest(this.CurrentUser.ID, true);

                    var userPrivy = (uiInstance == null) ? (clientR.GetUserPrivy(this.CurrentUser.ID)) : (null);

                    model = new InsuranceSubmitModel
                    {
                        Name = (uiInstance != null) ? (uiInstance.Requestor.Name) : (userPrivy != null ? userPrivy.Name : null),
                        //Mobile = (uiInstance != null) ? (uiInstance.Requestor.Mobile) : (this.CurrentUser.Mobile),
                        //IdentityNo = (uiInstance != null) ? (uiInstance.Requestor.IdentityNo) : (userPrivy != null ? userPrivy.IdentityNo : null),
                        Birthday = (uiInstance != null && uiInstance.Requestor.Birthday != null && uiInstance.Requestor.Birthday.HasValue) ? (uiInstance.Requestor.Birthday.Value) : (((userPrivy != null && userPrivy.Birthday != null && userPrivy.Birthday.HasValue) ? (userPrivy.Birthday.Value) : (new DateTime(1980, 1, 1, 0, 0, 0)))),
                        Province = (uiInstance != null) ? (uiInstance.Province) : null,
                        City = (uiInstance != null) ? (uiInstance.City) : null,
                        District = (uiInstance != null) ? (uiInstance.District) : null,
                        Location = (uiInstance != null) ? (uiInstance.Location) : null,
                        Gender = (this.CurrentUser.Gender != null && this.CurrentUser.Gender.HasValue) ? (this.CurrentUser.Gender.Value) : (true),
                    };
                }

                TempData["OKMsg"] = TempData["ErrorMsg"] = null;

                return View(model);
            }
            finally
            {
                CloseServiceClient(clientR);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(InsuranceSubmitModel model, string returnUrl)
        {
            var clientR = CreateServiceClient<IRepositoryService>();
            var clientI = CreateServiceClient<IInsuranceService>();

            try
            {
                var errMsg = new StringBuilder();

                if (ModelState.IsValid)
                {
                    #region "Validation"

                    #region "姓名"
                    if (string.IsNullOrEmpty(model.Name))
                    {
                        errMsg.AppendLine("请确保输入您真实的姓名！");
                    }
                    else
                    {
                        model.Name = model.Name.Trim();
                    }
                    #endregion

                    #region "身份证号"
                    //if (string.IsNullOrEmpty(model.IdentityNo))
                    //{
                    //	errMsg.AppendLine("请确保输入您真实的身份证号码！");
                    //}
                    //else
                    //{
                    //	model.IdentityNo = model.IdentityNo.Trim();

                    //	if (!Regex.IsMatch(model.IdentityNo, @"^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$") && !Regex.IsMatch(model.IdentityNo, @"^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}([0-9]|X)$"))
                    //	{
                    //		errMsg.AppendLine("您输入的身份证号码无效！");
                    //	}
                    //	else
                    //	{
                    //		var dtBirthday = DateTime.MinValue;

                    //		if (DateTime.TryParse(string.Format("{0}-{1}-{2}", model.IdentityNo.Substring(6, 4), model.IdentityNo.Substring(10, 2), model.IdentityNo.Substring(12, 2)), out dtBirthday))
                    //		{
                    //			if (dtBirthday.Date.CompareTo(model.Birthday.Date) != 0)
                    //			{
                    //				errMsg.AppendLine("您输入的生日与身份证号码不一致！");
                    //			}
                    //		}
                    //		else
                    //		{
                    //			errMsg.AppendLine("您输入的身份证号码无效！");
                    //		}
                    //	}
                    //} 
                    #endregion

                    #region "生日"
                    if (model.Birthday == null)
                    {
                        errMsg.AppendLine("请选择您真实的出生日期！！");
                    }
                    else if (model.Birthday.Value.CompareTo(DateTime.Now.Date.AddYears(-55)) < 0 || model.Birthday.Value.CompareTo(DateTime.Now.Date.AddYears(-18)) > 0)
                    {
                        errMsg.AppendLine("很抱歉，本次保险的适用年龄为 18 ~ 55 周岁，您将无法完成投保！");
                    }
                    #endregion

                    #region "手机号"
                    //if (string.IsNullOrEmpty(model.Mobile))
                    //{
                    //	errMsg.AppendLine("请输入您的手机号！");
                    //}
                    //else
                    //{
                    //	model.Mobile = model.Mobile.Trim();

                    //	if (!Regex.IsMatch(model.Mobile, "^[1][358][0-9]{9}$"))
                    //	{
                    //		errMsg.AppendLine("请输入的手机号无效！");
                    //	}
                    //}
                    #endregion

                    #region "省"
                    //if (string.IsNullOrEmpty(model.Province))
                    //{
                    //	errMsg.AppendLine("请输入您当前居住的省市！");
                    //} 
                    #endregion

                    #region "城市"
                    if (string.IsNullOrEmpty(model.City))
                    {
                        errMsg.AppendLine("请输入您当前居住的城市！");
                    }
                    else if (!CommonUtility.CheckIsInsuranceAvailableCity(model.City.Trim()))
                    {
                        errMsg.AppendLine("很抱歉，您当前所在的城市暂时不支持本次保险服务！");
                    }
                    else
                    {
                        model.City = model.City.Trim();
                    }
                    #endregion

                    #region "区"
                    //if (string.IsNullOrEmpty(model.District))
                    //{
                    //	errMsg.AppendLine("请输入您当前居住的区！");
                    //}
                    #endregion

                    #region "地址"
                    //if (string.IsNullOrEmpty(model.Location))
                    //{
                    //	errMsg.AppendLine("请输入您当前居住地址！");
                    //}
                    #endregion

                    #endregion

					#region "开始提交投保申请"
					if (errMsg.Length == 0)
					{
						model.Mobile = this.CurrentUser.Mobile;
						model.District = model.Location = model.City;

						var result = clientI.Create(new Libs.Models.InsuranceSubmitRequest
						{
							RequestorID = this.CurrentUser.ID,
							Request = model
						});

						if (result.Status == RequestStatus.OK)
						{
							TempData["OKMsg"] = "恭喜，您已经参保成功，快拼车让您拼车更安全，更容易！ ^_^";
							TempData["ErrorMsg"] = null;
							TempData["Request"] = null;

							if (!string.IsNullOrEmpty(returnUrl))
							{
								return Redirect(returnUrl);
							}
							else
							{
								return Content(string.Format("<script>alert('{0}');</script>", TempData["OKMsg"]));
							}
						}
						else if (!string.IsNullOrEmpty(result.Message))
						{
							errMsg.AppendLine(result.Message);
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

				TempData["OKMsg"] = null;
				TempData["ErrorMsg"] = errMsg.ToString();
				TempData["Request"] = model;

				return RedirectToAction("Create", "Insurance", new { fromurl = (!string.IsNullOrEmpty(Request.QueryString["FromUrl"]) ? "insurance_create" : string.Empty), returnUrl = returnUrl });
            }
            finally
            {
                CloseServiceClient(clientR);
            }
        }
        #endregion

        #region "Action: Detail"
        [Authorize]
        public ActionResult Detail(string folio = "")
        {
            var clientR = CreateServiceClient<IRepositoryService>();

            try
            {
                var uiInstance = default(UserInsuranceResult);

                if (string.IsNullOrEmpty(folio))
                {
                    uiInstance = clientR.GetInsuranceOrderLatest(this.CurrentUser.ID);

                    if (uiInstance == null)
                    {
                        return RedirectToAction("Create", "Insurance", new { fromurl = "insurance_detail", returnUrl = "/insurance/detail" });
                    }
                }
                else
                {
                    uiInstance = clientR.GetInsuranceOrder(folio);
                }

                #region "Validation"
                if (uiInstance == null)
                {
                    throw new Exception(string.Format("无效的保单号：{0}", folio));
                }
                else if (uiInstance.Requestor.ID != this.CurrentUser.ID)
                {
                    throw new UnauthorizedAccessException("您无权查看此保险工作单信息！");
                }
                #endregion

                if (TempData["OKMsg"] != null)
                {
                    ViewBag.OKMsg = ViewData["OKMsg"];

                    TempData["OKMsg"] = null;
                }

                ViewBag.ReturnUrl = Request.QueryString["ReturnUrl"];

                return View(uiInstance);
            }
            finally
            {
                CloseServiceClient(clientR);
            }
        }
        #endregion

		#region "Action: Terms"
		public ActionResult Terms()
		{
			return View();
		} 
		#endregion
    }
}