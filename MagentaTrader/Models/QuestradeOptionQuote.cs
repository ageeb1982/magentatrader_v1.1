using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class QuestradeOptionQuote
    {
        public string underlying { get; set; }
        public long underlyingId { get; set; }
        public string symbol { get; set; }
        public long symbolId { get; set; }
        public decimal bidPrice { get; set; }
        public decimal bidSize { get; set; }
        public decimal askPrice { get; set; }
        public decimal askSize { get; set; }
        public decimal lastTradePriceTrHrs { get; set; }
        public decimal lastTradePrice { get; set; }
        public decimal lastTradeSize { get; set; }
        public string lastTradeTick { get; set; }
        public string lastTradeTime { get; set; }
        public long volume { get; set; }
        public decimal openPrice { get; set; }
        public decimal highPrice { get; set; }
        public decimal lowPrice { get; set; }
        public decimal volatility { get; set; }
        public decimal delta { get; set; }
        public decimal gamma { get; set; }
        public decimal theta { get; set; }
        public decimal vega { get; set; }
        public decimal rho { get; set; }
        public long openInterest { get; set; }
        public long delay { get; set; }
        public bool isHalted { get; set; }
        public long VWAP { get; set; }
    }
}