using Newtonsoft.Json;
using SNS.Apps.KPC.Admin.Services.Security;
using SNS.Apps.KPC.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace SNS.Apps.KPC.Admin
{
    public static class UserAuthenticationConfig
    {
        public static void AuthenticateAndRegisterCurrentUser()
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                CustomPrincipalSerializeModel userSerialzeModel = JsonConvert.DeserializeObject<CustomPrincipalSerializeModel>(authTicket.UserData);
                if (userSerialzeModel == null)
                    return;
                CustomPrincipal user = new CustomPrincipal(authTicket.Name)
                {
                    FirstName = userSerialzeModel.FirstName,
                    LastName = userSerialzeModel.LastName,
                    roles = userSerialzeModel.Roles,
                    UserId = userSerialzeModel.UserId,
                    CreatedTime = userSerialzeModel.CreatedTime
                };
                HttpContext.Current.User = user;
            }
        }
    }
}