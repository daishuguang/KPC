using SNS.Apps.KPC.Admin.CustomAttribute;
using SNS.Apps.KPC.Admin.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SNS.Apps.KPC.Admin.Controllers
{
    [CustomAuthorizeAttribute]
    public class ConfigurationController : BaseController
    {
        //
        // GET: /Configuration/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SaveConfiguratonForm(FormCollection form)
        {
            string orderInterval = form.Get("OrderInterval");
            string msg = string.Empty;
            int statusCode = 0;
            if (CommonHelper.ParseInt(orderInterval, 6000) > 0)
            {
                ConfigurationManager.AppSettings["IntervalCheckTime"] = orderInterval;
                msg = "success";
                statusCode = (int)SNS.Apps.KPC.Admin.Utilities.EnumType.StatusCode.Success;
            }
            else
            {
                msg = "error";
                statusCode = (int)SNS.Apps.KPC.Admin.Utilities.EnumType.StatusCode.Error;
            }

            return Json(new
            {
                msg = msg,
                statusCode = statusCode
            });
        }

    }
}
