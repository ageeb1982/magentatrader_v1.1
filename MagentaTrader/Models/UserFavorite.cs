using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class UserFavorite
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string User { get; set; }
        public string Description { get; set; }
        public bool IsShared { get; set; }
        public string EncodedDate { get; set; }
        public int NoOfSymbols { get; set; }
    }
}