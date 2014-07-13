using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SNS.Apps.KPC.Admin.Controllers
{
    [AllowAnonymous]
    public class ErrorController : BaseController
    {
        //
        // GET: /Error/
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult DataBaseError()
        {
            return View();
        }

    }
}
