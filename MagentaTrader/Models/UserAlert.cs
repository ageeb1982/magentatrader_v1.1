using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class UserAlert
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string User { get; set; }
        public string Description { get; set; }
        public string Strategy { get; set; }
        public string Exchange { get; set; }
        public decimal Price { get; set; }
        public decimal Volume { get; set; }
        public decimal GrowthDecayRate { get; set; }
        public string GrowthDecayTime { get; set; }
        public int NoOfYears { get; set; }
        public int Correlation30 { get; set; }
        public string MACDOccurrence { get; set; }
        public bool IsActive { get; set; }
        public string EncodedDate { get; set; }
        public string AlertVia { get; set; }
    }
}