using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class Affiliate
    {
        public int Id { get; set; }
        public int ProductPackageId { get; set; }
        public string ProductPackage { get; set; }
        public int UserId { get; set; }
        public string User { get; set; }
        public string AffiliateURL { get; set; }
        public decimal Price { get; set; }
    }
}