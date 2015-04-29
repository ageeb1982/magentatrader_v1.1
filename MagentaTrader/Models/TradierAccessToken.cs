using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class TradierAccessToken
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string issued_at { get; set; }
        public string scope { get; set; }
        public string status { get; set; }
    }
}