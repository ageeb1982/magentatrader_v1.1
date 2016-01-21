using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class QuestradeOption
    {
        public string expiryDate { get; set; }
        public string description { get; set; }
        public string listingExchange { get; set; }
        public string optionExerciseType { get; set; }
        public List<Models.QuestradeOptionChainPerRoot> chainPerRoot { get; set; }
    }
}