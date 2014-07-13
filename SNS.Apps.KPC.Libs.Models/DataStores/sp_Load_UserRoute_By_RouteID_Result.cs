//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SNS.Apps.KPC.Libs.Models.DataStores
{
    using System;
    
    public partial class sp_Load_UserRoute_By_RouteID_Result
    {
        public Nullable<long> UserID { get; set; }
        public string UserOpenID { get; set; }
        public string UserGUID { get; set; }
        public string User_NickName { get; set; }
        public Nullable<bool> User_Gender { get; set; }
        public string User_Mobile { get; set; }
        public string User_WeChatID { get; set; }
        public string User_LicencePlateNumber { get; set; }
        public string User_PortraitsUrl { get; set; }
        public string User_PortraitsThumbUrl { get; set; }
        public Nullable<System.DateTime> User_CreateDate { get; set; }
        public Nullable<System.DateTime> User_UpdateDate { get; set; }
        public Nullable<int> User_Status { get; set; }
        public Nullable<bool> User_IsExtended { get; set; }
        public long RouteID { get; set; }
        public string RouteGUID { get; set; }
        public Nullable<int> RouteType { get; set; }
        public string From_Province { get; set; }
        public string From_City { get; set; }
        public string From_District { get; set; }
        public string From_Location { get; set; }
        public Nullable<double> From_Longitude { get; set; }
        public Nullable<double> From_Latitude { get; set; }
        public string To_Province { get; set; }
        public string To_City { get; set; }
        public string To_District { get; set; }
        public string To_Location { get; set; }
        public Nullable<double> To_Longitude { get; set; }
        public Nullable<double> To_Latitude { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<int> RepeatType { get; set; }
        public Nullable<int> RepeatCondition { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<double> Distance { get; set; }
        public Nullable<decimal> Charge { get; set; }
        public Nullable<int> SeatCount { get; set; }
        public string Note { get; set; }
        public Nullable<System.DateTime> Route_CreateDate { get; set; }
        public Nullable<System.DateTime> Route_UpdateDate { get; set; }
        public Nullable<bool> Route_IsExpired { get; set; }
        public Nullable<bool> Route_IsLongTerm { get; set; }
        public Nullable<int> Route_Status { get; set; }
        public int UserRole { get; set; }
    }
}
