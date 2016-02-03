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
        public long? delta { get; set; }
        public long? gamma { get; set; }
        public long? theta { get; set; }
        public long? vega { get; set; }
        public long? rho { get; set; }
        public long? openInterest { get; set; }
        public long? delay { get; set; }
        public bool? isHalted { get; set; }
        public double? VWAP { get; set; }
    }
}