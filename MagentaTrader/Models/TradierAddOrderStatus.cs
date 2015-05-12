using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class TradierAddOrderStatus
    {
        public int id { get; set; }
        public string status { get; set; }
        public string partner_id { get; set; }
    }
}