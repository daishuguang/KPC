using SNS.Apps.KPC.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SNS.Apps.KPC.Admin.ViewModels
{
    public class RouteViewModel : Routes
    {
        public long PublisherID { get; set; }
        public string PublisherNickName { get; set; }
        public string PublisherMobile { get; set; }
        public int PublisherRoleType { get; set; }

        public string RouteTypeContent
        {
            get
            {
                return Enum.GetName(typeof(SNS.Apps.KPC.Admin.Utilities.EnumType.RouteType), RouteType);
            }
        }
        public string PublisherRole
        {
            get { return Enum.GetName(typeof(SNS.Apps.KPC.Admin.Utilities.EnumType.UserRole), PublisherRoleType); }
        }
    }
}