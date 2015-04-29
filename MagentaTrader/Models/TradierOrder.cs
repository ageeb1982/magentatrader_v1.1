using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace MagentaTrader.Models
{
    public class TradierOrder
    {
        public int id { get; set; }
        public string type { get; set; }
        public string symbol { get; set; }
        public string side { get; set; }
        public decimal quantity { get; set; }
        public string status { get; set; }
        public string duration { get; set; }
        public decimal avg_fill_price { get; set; }
        public decimal exec_quantity { get; set; }
        public decimal last_fill_price { get; set; }
        public decimal remaining_quantity { get; set; }
        public string create_date { get; set; }
        public string transaction_date { get; set; }
        [XmlElement("class")]
        public string order_class { get; set; }
        public string option_symbol { get; set; }
    }
}