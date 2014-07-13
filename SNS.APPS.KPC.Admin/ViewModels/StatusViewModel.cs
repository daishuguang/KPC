using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SNS.Apps.KPC.Admin.ViewModels
{
    public class StatusViewModel
    {
        public string Message { get; set; }
        public SNS.Apps.KPC.Admin.Utilities.EnumType.StatusCode StatusCode { get; set; }
    }
}