using SNS.Apps.KPC.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SNS.Apps.KPC.Admin
{
    public class ModelBinderConfig
    {
        public static void BindCustomModel()
        {
            ModelBinders.Binders.Add(typeof(Pagination), new PaginationDataBinder());
        }
    }
}