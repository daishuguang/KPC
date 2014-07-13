using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SNS.Apps.KPC.Admin.IServices
{
    public interface IAuthenticationServices
    {
        bool Authenticate(string username, string password);
    }
}