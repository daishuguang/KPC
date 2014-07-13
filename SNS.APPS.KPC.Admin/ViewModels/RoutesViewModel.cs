using SNS.Apps.KPC.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SNS.Apps.KPC.Admin.ViewModels
{
    public class RoutesViewModel
    {
        public int total { get; set; }
        public List<Routes> rows { get; set; }
    }
}