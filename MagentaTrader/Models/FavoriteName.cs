using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class FavoriteName
    {
        public String Remarks { get; set; }
        public Int32 UserId { get; set; }
        public String User { get; set; }
        public Int32 NoOfSymbols { get; set; }
    }
}