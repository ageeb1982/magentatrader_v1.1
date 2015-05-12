using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class TradierPreviewEquity
    {
        public decimal commission { get; set; }
        public decimal cost { get; set; }
        public bool  extended_hours { get; set; }
        public decimal fees { get; set; }
        public decimal margin_change { get; set; }
        public decimal quantity { get; set; }
        public string status { get; set; }
    }
}