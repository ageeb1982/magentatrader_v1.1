using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class TradierOption
    {
        public string symbol { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public decimal? volume { get; set; }
        public decimal? last { get; set; }
        public decimal? change { get; set; }
        public decimal? bid { get; set; }
        public decimal? bidsize { get; set; }
        public decimal? ask { get; set; }
        public decimal? asksize { get; set; }
        public decimal? open_interest { get; set; }
        public decimal strike { get; set; }
        public decimal? contract_size { get; set; }
        public string expiration_date { get; set; }
        public string expiration_type { get; set; }
        public string option_type { get; set; }
        public string root_symbol { get; set; }
    }
}