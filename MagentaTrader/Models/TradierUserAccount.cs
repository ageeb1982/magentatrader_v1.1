using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class TradierUserAccount
    {
        public string name { get; set; }
        public string id { get; set; }
        public Models.TradierUser account { get; set; }
    }
}