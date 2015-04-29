using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class TradierUser
    {
        public string account_number { get; set; }
        public bool day_trader { get; set; }
        public int option_level { get; set; }
        public string type { get; set; }
        public string last_update_date { get; set; }
        public string status { get; set; }
        public string classification { get; set; }
    }
}