using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class Symbol
    {
        public string SymbolDescription { get; set; }
	    public string Description { get; set; }
	    public string Exchange { get; set; }
        public decimal ClosePrice { get; set; }
        public decimal Volume { get; set; }
        public decimal GrowthDecayRate { get; set; }
	    public string LatestQuoteDate { get; set; }
	    public string CalendarUpDate { get; set; }
        public int CalendarUpDay { get; set; }
	    public string CalendarUpParticulars { get; set; }
	    public decimal CalendarUpDelta { get; set; }
	    public decimal CalendarUpPercentage { get; set; }
	    public string CalendarDownDate { get; set; }
        public int CalendarDownDay { get; set; }
	    public string CalendarDownParticulars { get; set; }
        public decimal CalendarDownDelta { get; set; }
        public decimal CalendarDownPercentage { get; set; }
        public decimal GrowthDecayRateW1 { get; set; }
        public decimal GrowthDecayRateW2 { get; set; }
        public decimal GrowthDecayRateW3 { get; set; }
        public decimal GrowthDecayRateM1 { get; set; }
        public decimal GrowthDecayRateM2 { get; set; }
        public decimal GrowthDecayRateM3 { get; set; }
        public int NoOfYears { get; set; }
        public int TrendNoOfDays { get; set; }
        public string WinLossCurrent30 { get; set; }
        public decimal WinLossAverageCurrent30 { get; set; }
        public string WinLoss20 { get; set; }
        public decimal WinLossAverage20 { get; set; }
        public string WinLoss40 { get; set; }
        public decimal WinLossAverage40 { get; set; }
        public string WinLoss60 { get; set; }
        public decimal WinLossAverage60 { get; set; }
        public decimal CorrelationCoefficient30 { get; set; }
        public decimal SeasonalityCorrelation { get; set; }
        public decimal MACDTrendNoOfDays { get; set; }
        public decimal MACDGrowthDecayRate { get; set; }
        public decimal EMATrendNoOfDays { get; set; }
        public decimal EMAGrowthDecayRate { get; set; }
        public string EMAStartDate { get; set; }
        public decimal EMALinear { get; set; }
        public decimal Nov7ClosePrice { get; set; }
        public int Nov7NumberOfDays { get; set; }
        public decimal Nov7CorrelationCoefficient { get; set; }
    }
}