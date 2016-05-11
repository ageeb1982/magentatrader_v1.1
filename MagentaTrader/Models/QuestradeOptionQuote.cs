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
        public double? bidPrice { get; set; }
        public long? bidSize { get; set; }
        public double? askPrice { get; set; }
        public long? askSize { get; set; }
        public double? lastTradePriceTrHrs { get; set; }
        public double? lastTradePrice { get; set; }
        public long? lastTradeSize { get; set; }
        public string lastTradeTick { get; set; }
        public string lastTradeTime { get; set; }
        public long? volume { get; set; }
        public double? openPrice { get; set; }
        public double? highPrice { get; set; }
        public double? lowPrice { get; set; }
        public double? volatility { get; set; }
        public double? delta { get; set; }
        public double? gamma { get; set; }
        public double? theta { get; set; }
        public double? vega { get; set; }
        public double? rho { get; set; }
        public double? openInterest { get; set; }
        public double? delay { get; set; }
        public bool? isHalted { get; set; }
        public double? VWAP { get; set; }
    }
}