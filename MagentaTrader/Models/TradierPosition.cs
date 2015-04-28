using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class TradierPosition
    {
        public int id { get; set; }
        public decimal cost_basis { get; set; }
        public string date_acquired { get; set; }
        public decimal quantity { get; set; }
        public string symbol { get; set; }
    }
}