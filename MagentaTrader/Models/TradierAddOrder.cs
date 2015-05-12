using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class TradierAddOrder
    {
        public string token { get; set; }
        public string account { get; set; }
        public string symbol { get; set; }
        public string duration { get; set; }
        public string side { get; set; }
        public decimal quantity { get; set; } 
        public string type { get; set; }
        public decimal price { get; set; } 
        public decimal stop { get; set; }
        public string option_symbol { get; set; }
    }
}