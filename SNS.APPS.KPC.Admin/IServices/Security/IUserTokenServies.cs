using SNS.Apps.KPC.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SNS.Apps.KPC.Admin.IServices
{
    public interface IUserTokenServies
    {
        void GenerateAuthorizedUserToken();
    }
}