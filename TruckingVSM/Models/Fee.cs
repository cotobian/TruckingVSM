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
    
    public partial class Fee
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public Nullable<int> Price { get; set; }
        public string Unit { get; set; }
        public string staff_cd { get; set; }
        public Nullable<System.DateTime> update_time { get; set; }
        public Nullable<int> StdFeeID { get; set; }
    }
}
