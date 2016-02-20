using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class SectorSymbol
    {
        public int Id { get; set; }
        public int SectorId { get; set; }
        public int SymbolId { get; set; }
        public string Symbol { get; set; }
        public string Description { get; set; }
    }
}