using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SNS.Apps.KPC.Open.Controllers
{
    public class MapController : Controller
    {
        [AllowAnonymous]
        public ActionResult Create_Map()
        {
            return View();
        }

		 [AllowAnonymous]
        public ActionResult Detail_Map()
        {
            return View();
        }
    }
}
