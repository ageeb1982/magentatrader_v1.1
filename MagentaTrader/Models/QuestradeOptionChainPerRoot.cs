using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class QuestradeOptionChainPerRoot
    {
        public string optionRoot { get; set; }
        public List<Models.QuestradeOptionChainPerStrikePrice> chainPerStrikePrice { get; set; }
        public long multiplier { get; set; }
    }
}