using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class UserAlert
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string User { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string EncodedDate { get; set; }
        public string AlertVia { get; set; }

        public bool SymbolFilter { get; set; }
        public string SymbolExchange { get; set; }
        public int SymbolUserFavoritesId { get; set; }
        public string SymbolUserFavorites { get; set; }

        public bool StrategyFilter { get; set; }
        public string Strategy { get; set; }
        public decimal StrategyGrowthDecayRate { get; set; }
        public string StrategyGrowthDecayTime { get; set; }

        public bool MACDFilter { get; set; }
        public string MACDCrossover { get; set; }
        public string MACDEMA { get; set; }

        public bool MagentaChannelFilter { get; set; }
        public string MagentaChannelBegins { get; set; }
        public int MagentaChannelCorrelation30 { get; set; }
        public int MagentaChannelDays { get; set; }
        public decimal MagentaChannelAGRADR { get; set; }

        public bool SeasonalityFilter { get; set; }
        public decimal SeasonalityWinLossPercent { get; set; }
        public decimal SeasonalityGainLossPercent { get; set; }

        public bool AdditionalFilter { get; set; }
        public decimal AdditionalFilterPrice { get; set; }
        public decimal AdditionalFilterVolume { get; set; }
        public int AdditionalFilterNoOfYears { get; set; }

    }
}