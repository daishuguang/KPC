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
    
    public partial class tbl_Phone_Verify
    {
        public long ID { get; set; }
        public string Phonenum { get; set; }
        public string Code { get; set; }
        public Nullable<int> Channel { get; set; }
        public Nullable<int> Count { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}
