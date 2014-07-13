using SNS.Apps.KPC.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SNS.Apps.KPC.Admin.ViewModels
{
    public class WeChatMessageDetailsViewModel : WeChatMessage
    {
        public string MsgRenderContent { get; set; }
        public string MsgTypeString { get; set; }
    }

    public class WeChatMessageAndUserViewModel : WeChatMessageDetailsViewModel
    {
        public long UserID { get; set; }
        public string OpenID { get; set; }
        public string FakeID { get; set; }
        public string UserGUID { get; set; }
        public string NickName { get; set; }
        public Nullable<bool> Gender { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public string WeChatID { get; set; }
        public string QQ { get; set; }
        public Nullable<int> UserRole { get; set; }
        public string LicencePlateNumber { get; set; }
        public string PortraitsUrl { get; set; }
        public string PortraitsThumbUrl { get; set; }
        public Nullable<bool> IsSynced { get; set; }
        public Nullable<bool> EnableNotify { get; set; }
        public Nullable<bool> IsExtended { get; set; }
        public Nullable<int> ExtendChannel { get; set; }
        public new Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> RefType { get; set; }
        public Nullable<long> RefID { get; set; }

    }

    public class WeChatMessageUserRouteViewModel : WeChatMessage
    {
        //public long UserID { get; set; }
        //public Routes
    }
   
}