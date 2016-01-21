using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public int SymbolId { get; set; }
        public string Symbol { get; set; }
        public string SymbolDescription { get; set; }
        public string Exchange { get; set; }
        public int UserId { get; set; }
        public string User { get; set; }
        public string Remarks { get; set; }
        public bool IsShared { get; set; }
        public string EncodedDate { get; set; }
        public string Trend { get; set; }
    }
}