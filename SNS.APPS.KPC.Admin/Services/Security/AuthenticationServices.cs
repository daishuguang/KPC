using Newtonsoft.Json;
using SNS.Apps.KPC.Admin.IServices;
using SNS.Apps.KPC.Admin.ViewModels;
using System;
using System.Configuration;
using System.Web;
using System.Web.Security;

namespace SNS.Apps.KPC.Admin.Services.Security
{
    public class AuthenticationServices : AuthenticationServicesBase
    {
        public HttpContextBase HttpContext { get; private set; }
        public AuthenticationServices(HttpContextBase httpContext)
        {
            this.HttpContext = httpContext;
        }
        
        public override void GenerateAuthorizedUserToken()
        {

            string userData = JsonConvert.SerializeObject(base.UserPrincipalModel);
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                                             "UserName",
                                             DateTime.Now,
                                             DateTime.Now.AddMinutes(30), // value of time out property
                                             false, // Value of IsPersistent property
                                             userData,
                                             FormsAuthentication.FormsCookiePath);
            string encryptedTicket = FormsAuthentication.Encrypt(ticket);
            HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            if (HttpContext.Request.Cookies.Get(faCookie.Name) != null)
            {
                HttpContext.Request.Cookies.Remove(faCookie.Name);
            }
            HttpContext.Response.Cookies.Add(faCookie);
        }
    }
}