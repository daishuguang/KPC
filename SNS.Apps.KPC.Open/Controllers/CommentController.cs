using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Utils;

namespace SNS.Apps.KPC.Open.Controllers
{
    public class CommentController : Base.BaseController
    {
        public ActionResult Comment()
        {
            return View();
        }
    }
}
