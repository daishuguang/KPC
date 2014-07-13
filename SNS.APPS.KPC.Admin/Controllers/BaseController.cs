using SNS.Apps.KPC.Admin.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SNS.Apps.KPC.Admin.Controllers
{
    [HandleError(View="Error/Index")]
    [HandleError(ExceptionType = typeof(System.Data.SqlClient.SqlException), View = "Error/DataBaseError")]
    public class BaseController : Controller
    {
        protected virtual new CustomPrincipal User
        {
            get { return HttpContext.User as CustomPrincipal; }
        }
    }
}
