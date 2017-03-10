using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class UserAlertSymbolResult
    {
        public int SymbolFilterResult { get; set; }
        public int StrategyFilterResult { get; set; }
        public int MACDFilterResult { get; set; }
        public int MagentaChannelFilterResult { get; set; }
        public int SeasonalityFilterResult { get; set; }
        public int AdditionalFilterResult { get; set; }
    }
}