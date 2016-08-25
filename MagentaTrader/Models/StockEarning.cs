using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class StockEarning
    {
        public int Id { get; set; }
        public int SymbolId { get; set; }
        public string Symbol { get; set; }
        public string EarningDate { get; set; }
        public string EarningTime { get; set; }
        public string PeriodEnding { get; set; }
        public decimal EstimatedValue { get; set; }
        public decimal ReportedValue { get; set; }
    }
}