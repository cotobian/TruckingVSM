//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TruckingVSM.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Trip
    {
        public int ID { get; set; }
        public Nullable<int> TruckID { get; set; }
        public Nullable<int> DriverID { get; set; }
        public Nullable<System.DateTime> CompleteDate { get; set; }
        public string staff_cd { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> update_time { get; set; }
        public string Note { get; set; }
        public string SendSMS { get; set; }
    }
}
