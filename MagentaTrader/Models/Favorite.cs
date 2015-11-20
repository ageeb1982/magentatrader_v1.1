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
        public int UserlId { get; set; }
        public string Remarks { get; set; }
    }
}