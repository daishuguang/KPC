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
    
    public partial class User_Comment
    {
        public long ID { get; set; }
        public long UserID { get; set; }
        public long RouteID { get; set; }
        public long ScoreBy { get; set; }
        public Nullable<int> ScoreRate { get; set; }
        public string Comments { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
    }
}
