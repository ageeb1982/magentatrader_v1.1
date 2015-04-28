using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class TradierQuote
    {
        public string symbol { get; set; }
        public string description { get; set; }
        public string exch { get; set; }
        public string type { get; set; }
        public decimal last { get; set; }
        public decimal change { get; set; }
        public decimal change_percentage { get; set; }
        public decimal volume { get; set; }
        public decimal average_volume { get; set; }
        public decimal last_volume { get; set; }
        public string trade_date { get; set; }
        public decimal open { get; set; }
        public decimal high { get; set; }
        public decimal low { get; set; }
        public decimal close { get; set; }
        public decimal prevclose { get; set; }
        public decimal week_52_high { get; set; }
        public decimal week_52_low { get; set; }
        public decimal bid { get; set; }
        public decimal bidsize { get; set; }
        public string bidexch { get; set; }
        public string bid_date { get; set; }
        public decimal ask { get; set; }
        public string asksize { get; set; }
        public string askexch { get; set; }
        public string ask_date { get; set; }
    }
}