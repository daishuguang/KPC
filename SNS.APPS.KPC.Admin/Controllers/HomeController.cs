using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using SNS.Apps.KPC.Admin.IServices;
using SNS.Apps.KPC.Admin.Services;
using SNS.Apps.KPC.Admin.Services.Security;

namespace SNS.Apps.KPC.Admin.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        public AuthenticationServicesBase AuthenticationServices;
        //
        // GET: /Home/
        public HomeController()
        {
         
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            AuthenticationServices = new AuthenticationServices(requestContext.HttpContext);
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View("Index");
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            string loginMsg = string.Empty;
            bool result = AuthenticationServices.Authenticate(username, password);
            if (result)
            {
                return RedirectToAction("Index", "Route");
            }
              
            loginMsg = "登录失败，请重试";
            ViewBag.LoginMsg = loginMsg;
            return View("Index");
        }
    }
}
