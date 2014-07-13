using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SNS.Apps.KPC.Admin.Models;

namespace SNS.Apps.KPC.Admin.ViewModels
{
    public class OrderViewModel : User_Order
    {
        public string RequestorName { get; set; }
        public string RequestorMobile { get; set; }
        public string SupplierName { get; set; }
        public string SupplierMobile { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
    }
}