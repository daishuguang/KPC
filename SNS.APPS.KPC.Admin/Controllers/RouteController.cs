using SNS.Apps.KPC.Admin.IServices;
using SNS.Apps.KPC.Admin.Models;
using SNS.Apps.KPC.Admin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SNS.Apps.KPC.Admin.ViewModels;
using SNS.Apps.KPC.Admin.CustomAttribute;
using SNS.Apps.KPC.Admin.Utilities;
using KPCModels = SNS.Apps.KPC.Libs.Models;
using KPCUtils = SNS.Apps.KPC.Libs.Utils;
using KPCIServices = SNS.Apps.KPC.Libs.IServices;
using KPCConfig = SNS.Apps.KPC.Libs.Configurations;

namespace SNS.Apps.KPC.Admin.Controllers
{
    [CustomAuthorizeAttribute]
    public class RouteController : BaseController
    {
        public IRouteServices RouteServices;
        public IUserServices UserServices;
        public KPCIServices.IRouteMatrixService RoutesMatrixServices;

        public RouteController()
        {
            RouteServices = new RouteServices();
            UserServices = new UserServices();
        }
        //
        // GET: /Route/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetRoutesData(int page = 1, int rows = 10, string sort = "CreateDate", string order = "desc")
        {
            int total = 0;
            var routes = RouteServices.GetRoutes(page, rows, sort, order, out total, new KeyValuePair<string, string>());
            SNSEntityJsonViewModel<ViewModels.RouteViewModel> model = new SNSEntityJsonViewModel<RouteViewModel>();
            model.rows = routes.ToList();
            model.total = total;
            RouteServices.Dispose();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult SearchRoutesData(int uid = 0, int page = 1, int rows = 10, string sort = "CreateDate", string order = "desc")
        {
            int total = 0;
            var routes = RouteServices.GetRoutesByUserID(uid, page, rows, sort, order, out total);
            SNSEntityJsonViewModel<ViewModels.RouteViewModel> model = new SNSEntityJsonViewModel<RouteViewModel>();
            model.rows = routes.ToList();
            model.total = total;
            RouteServices.Dispose();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PublishRoute()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PublishRoute(FormCollection form)
        {
            string userId = form.Get("UserId");
            string fromProvince = form.Get("From_Province");
            string fromCity = form.Get("From_City");
            string fromDistrict = form.Get("From_District");
            string fromLocation = form.Get("From_Location");
            double fromLongitude = double.Parse(form.Get("From_Longitude"));
            double fromLatitude = double.Parse(form.Get("From_Latitude"));
            string toProvince = form.Get("To_Province");
            string toCity = form.Get("To_City");
            string toDistrict = form.Get("To_District");
            string toLocation = form.Get("To_Location");
            double toLongitude = double.Parse(form.Get("To_Longitude"));
            double toLatitude = double.Parse(form.Get("To_Latitude"));

            string userRole = form.Get("userRole");
            int seatCount = CommonHelper.ParseInt(form.Get("seatCount"), 0);
            decimal charge = CommonHelper.ParseDecimal(form.Get("charge"), 0);

            string pincheType = form.Get("pincheType");
            string startDate = form.Get("startDate");
            string startDateTime = form.Get("startDateTime");
            string isLongTerm = form.Get("isLongTerm");
            string note = form.Get("message");


            KPCModels.DataStores.tbl_User dataUser = UserServices.GetKpcUserById(int.Parse(userId));
            KPCModels.User currentUser = new KPCModels.User(dataUser);
            var requestInfo = new KPCModels.RouteCreateRequest
            {
                User = new KPCModels.UserParticalModel(currentUser),
                Route = new KPCModels.RouteParticalModel { },
                UserRole = (currentUser != null) ? (currentUser.UserRole) : KPCModels.UserRole.Passenger
            };

            requestInfo.Route.From_Province = fromProvince;
            requestInfo.Route.From_City = fromCity;
            requestInfo.Route.From_District = fromDistrict;
            requestInfo.Route.From_Location = fromLocation;
            requestInfo.Route.From_Point = new KPCModels.Point { Longitude = fromLongitude, Latitude = fromLatitude };


            requestInfo.Route.To_Province = toProvince;
            requestInfo.Route.To_City = toCity;
            requestInfo.Route.To_District = toDistrict;
            requestInfo.Route.To_Location = toLocation;
            requestInfo.Route.To_Point = new KPCModels.Point { Longitude = toLongitude, Latitude = toLatitude };

            switch (pincheType)
            {
                case "sxb":
                    {
                        var match = System.Text.RegularExpressions.Regex.Match(startDateTime, @"^(\d{2})\:(\d{2})$");
                        if (match.Success)
                        {
                            requestInfo.Route.StartDate = DateTime.Parse(string.Format("{0}-{1}-{2} {3}:00", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, startDateTime));
                        }
                    }
                    break;
                case "ct":
                    {
                        var match = System.Text.RegularExpressions.Regex.Match(startDate, @"^(\d{4})\-(\d{2})\-(\d{2})$");

                        if (match.Success)
                        {
                            requestInfo.Route.StartDate = DateTime.Parse(string.Format("{0}-{1}-{2} 00:00:00", match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value));
                        }
                        requestInfo.Route.IsLongTerm = Convert.ToBoolean(isLongTerm);
                    }
                    break;
            }


            requestInfo.Route.Charge = charge;
            requestInfo.Route.SeatCount = seatCount;
            requestInfo.Route.Note = note;

            var statusModel = new StatusViewModel();
            RoutesMatrixServices = KPCUtils.WCFServiceClientUtility.CreateServiceChanel<KPCIServices.IRouteMatrixService>(KPCConfig.ConfigStore.APIServiceSettings.RouteMatrix_Service_Entry);
            if (ModelState.IsValid)
            {
                // Publish to Server
                try
                {
                    var rID = RoutesMatrixServices.Publish(requestInfo);

                    if (rID.CompareTo(Guid.Empty) != 0)
                    {
                        ViewBag.Msg = "发布成功！";
                        statusModel.Message = ViewBag.Msg;
                        statusModel.StatusCode = EnumType.StatusCode.Success;
                    }
                    else
                    {
                        ViewBag.Msg = "发布失败，请检查数据并重试！";
                        statusModel.Message = ViewBag.Msg;
                        statusModel.StatusCode = EnumType.StatusCode.Error;
                    }
                }
                catch
                {
                    ViewBag.Msg = "提交数据发生错误，请重试！";
                    statusModel.Message = ViewBag.Msg;
                    statusModel.StatusCode = EnumType.StatusCode.UnknowException;
                }
                finally
                {
                    KPCUtils.WCFServiceClientUtility.CloseServiceChannel(RoutesMatrixServices);
                }
            }

            //return View();
            return Json(statusModel);
        }
    }
}
