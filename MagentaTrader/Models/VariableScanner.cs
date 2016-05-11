using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class VariableScanner
    {
        public string Symbol { get; set; }
        public string Description { get; set; }
        public string UpDate { get; set; }
        public string UpRate { get; set; }
        public decimal UpPercentage { get; set; }
        public int UpNoOfDays { get; set; }
        public List<YearPercentage> UpYearPercentage { get; set; }
        public string DownDate { get; set; }
        public string DownRate { get; set; }
        public decimal DownPercentage { get; set; }
        public int DownNoOfDays { get; set; }
        public List<YearPercentage> DownYearPercentage { get; set; }
    }

    public class YearPercentage
    {
        public decimal Percentage { get; set; }
    }
}