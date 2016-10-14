using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class UserAlertSymbol
    {
        public int Id { get; set; }
        public int UserAlertId { get; set; }
        public int SymbolId { get; set; }
        public string Symbol { get; set; }
        public string SymbolDescription { get; set; }
        public string Trend { get; set; }
        public string EncodedDate { get; set; }
    }
}