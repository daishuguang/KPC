//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SNS.Apps.KPC.Admin
{
    using System;
    using System.Collections.Generic;
    
    public partial class User_Import
    {
        public long ID { get; set; }
        public string UserGUID { get; set; }
        public string NickName { get; set; }
        public Nullable<bool> Gender { get; set; }
        public string Mobile { get; set; }
        public Nullable<int> UserRole { get; set; }
        public string PortraitsUrl { get; set; }
        public string PortraitsThumbUrl { get; set; }
        public Nullable<bool> IsImported { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}
