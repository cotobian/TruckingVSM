using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TruckingVSM.Models
{
    public class TripViewModels
    {
        public int ID { get; set; }
        public string CntrNo { get; set; }
        public string Name { get; set; }
        public string TruckNo { get; set; }
        public string Driver { get; set; }
        public string CompleteDate { get; set; }
        public string Status { get; set; }
    }
}