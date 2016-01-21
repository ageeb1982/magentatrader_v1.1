using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class QuestradeSymbol
    {
        public string symbol { get; set; }
        public long symbolId { get; set; }
        public string description { get; set; }
        public string securityType { get; set; }
        public string listingExchange { get; set; }
        public bool isTradable { get; set; }
        public bool isQuotable { get; set; }
        public string currency { get; set; }
    }
}