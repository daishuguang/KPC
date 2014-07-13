using SNS.Apps.KPC.Admin.CustomAttribute;
using SNS.Apps.KPC.Admin.IServices;
using SNS.Apps.KPC.Admin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SNS.Apps.KPC.Admin.Controllers
{
    [CustomAuthorizeAttribute]
    public class UserController : BaseController
    {
        public IUserServices UserServices;

        public UserController()
        {
            UserServices = new UserServices();
        }
        //
        // GET: /User/
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetUser(string nickName, string mobile)
        {
            var user = UserServices.GetUser(nickName, mobile);
            UserServices.Dispose();
            return Json(
                new
                {
                    id = user.ID,
                    nickname = user.NickName,
                    mobile = user.Mobile,
                    openid = user.OpenID
                }, JsonRequestBehavior.AllowGet);
        }

    }
}
