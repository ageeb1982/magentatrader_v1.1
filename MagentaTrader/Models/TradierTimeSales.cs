using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class TradierTimeSales
    {
        public string time { get; set; }
        public int timestamp { get; set; }
        public double price { get; set; }
        public double open { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double close { get; set; }
        public int volume { get; set; }
        public double vwap { get; set; }
    }
}