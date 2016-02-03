using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class QuestradeOptionFilter
    {
        public string optionType { get; set; }
        public long underlyingId { get; set; }
        public string expiryDate { get; set; }
        public decimal minstrikePrice { get; set; }
        public decimal maxstrikePrice { get; set; }
    }
}