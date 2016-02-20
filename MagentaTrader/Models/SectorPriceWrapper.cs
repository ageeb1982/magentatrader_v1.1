using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class SectorPriceWrapper
    {
        public string Sector { get; set; }
        public string SectorDescription { get; set; }
        public List<Models.SectorPrice> SectorPrices { get; set; }
    }
}