using SNS.Apps.KPC.Admin.CustomAttribute;
using SNS.Apps.KPC.Admin.IServices;
using SNS.Apps.KPC.Admin.Services;
using SNS.Apps.KPC.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SNS.Apps.KPC.Admin.Controllers
{
    [CustomAuthorizeAttribute]
    public class OrderController : BaseController
    {
        public IOrderServices OrderServices;
        public OrderController()
        {
            OrderServices = new OrderServices();
        }

        //
        // GET: /Order/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetOrderData(int page = 1, int rows = 10, string sort = "CreateDate", string order = "desc")
        {
            int total = 0;
            var orderList = OrderServices.GetOrders(page, rows, sort, order, out total);
            SNSEntityJsonViewModel<OrderViewModel> model = new SNSEntityJsonViewModel<OrderViewModel>();
            model.rows = orderList.ToList();
            model.total = total;
            OrderServices.Dispose();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CheckOrderByDatetime()
        {
            int newOrderCount = 0;
            string msg = "您有新订单";
            var model = OrderServices.GetNewOrdersWithInintervalTime();
            if (model.ToList().Count > 0)
                newOrderCount = model.Count();
            OrderServices.Dispose();
            return Json(new
            {
                ordercount = newOrderCount,
                msg = msg
            }, JsonRequestBehavior.AllowGet);


        }



    }
}
