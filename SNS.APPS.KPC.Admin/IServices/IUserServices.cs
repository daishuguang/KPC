using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SNS.Apps.KPC.Admin.Models;
using KPCLibs = SNS.Apps.KPC.Libs.Models;

namespace SNS.Apps.KPC.Admin.IServices
{
    public interface IUserServices : IDisposable
    {
        Users GetUserById(int id);
        KPCLibs.DataStores.tbl_User GetKpcUserById(int id);
        Users GetUser(string nickName,string mobile);

    }
}