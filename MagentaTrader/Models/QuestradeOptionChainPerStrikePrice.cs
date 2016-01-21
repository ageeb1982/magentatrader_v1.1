using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class QuestradeOptionChainPerStrikePrice
    {
        public decimal strikePrice { get; set; }
        public long callSymbolId { get; set; }
        public long putSymbolId { get; set; }
    }
}