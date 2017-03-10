using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MagentaTrader.Controllers
{
    public class UserAlertAPIController : ApiController
    {
        private Data.MagentaTradersDBDataContext db = new Data.MagentaTradersDBDataContext();

        private Boolean convertWinLossToPercent(string WinLoss, decimal limit)
        {
            int separatorLocation  = WinLoss.IndexOf("/");
            if (separatorLocation > 0)
            {
                string numerator = WinLoss.Substring(0, separatorLocation - 1);
                string denumerator = WinLoss.Substring(separatorLocation + 1);

                string direction = numerator.Substring(numerator.Length - 1);

                numerator = new String(numerator.Where(Char.IsDigit).ToArray());
                denumerator = new String(denumerator.Where(Char.IsDigit).ToArray());

                int n = Int32.Parse(numerator);
                int d = Int32.Parse(denumerator);

                int m = 1;
                if (direction == "L") m = -1;

                int wl = 0;
                if (n + d > 0) wl = (n / (n + d)) * 100 * m;
                else wl =  100 * m;

                if (direction == "L")
                {
                    return wl <= limit;
                }
                else
                {
                    return wl >= limit;
                }
            }
            else
            {
                return true;
            }
        }
        private List<Data.MstSymbol> GetResultLevelSymbol(long userAlertId)
        {
            List<Data.MstSymbol> symbols = new List<Data.MstSymbol>();
            string exchange;

            if (userAlertId > 0)
            {
                var userAlerts = from d in db.TrnUserAlerts where d.Id == userAlertId select d;
                exchange = userAlerts.First().SymbolExchange;
                if (userAlerts.Any()) {
                    var filterSymbols = from d in db.MstSymbols select d;
                    if (exchange == "US")
                    {
                        filterSymbols = from d in db.MstSymbols
                                        where d.Exchange == "AMEX" || d.Exchange == "NYSE" || d.Exchange == "NASDAQ"
                                        select d;
                    }
                    else
                    {
                        filterSymbols = from d in db.MstSymbols
                                        where d.Exchange == exchange
                                        select d;
                    }
                    foreach (Data.MstSymbol s in filterSymbols)
                    {
                        symbols.Add(s);
                    }
                }
            }
            return symbols.ToList();
        }
        private List<Data.MstSymbol> GetResultLevelStrategy(long userAlertId, List<Data.MstSymbol> filteredSymbols)
        {
            List<Data.MstSymbol> symbols = new List<Data.MstSymbol>();
            string strategy;
            if (filteredSymbols.Any())
            {
                if (userAlertId > 0)
                {
                    var userAlerts = from d in db.TrnUserAlerts where d.Id == userAlertId select d;
                    if (userAlerts.Any())
                    {
                        strategy = userAlerts.First().Strategy;
                        var strategySymbols = from d in filteredSymbols select d;
                        if (strategy == "MED")
                        {
                            strategySymbols = from d in filteredSymbols 
                                              where d.MACDGrowthDecayRate < 0 &&  d.EMAGrowthDecayRate < 0
                                              select d;
                        }
                        else if (strategy == "MEU")
                        {
                            strategySymbols = from d in filteredSymbols
                                              where d.MACDGrowthDecayRate >= 0 && d.EMAGrowthDecayRate >= 0
                                              select d;
                        }
                        foreach (Data.MstSymbol s in strategySymbols)
                        {
                            symbols.Add(s);
                        }
                    }
                }
            }
            return symbols.ToList();
        }
        private List<Data.MstSymbol> GetResultLevelMACD(long userAlertId, List<Data.MstSymbol> filteredSymbols)
        {
            List<Data.MstSymbol> symbols = new List<Data.MstSymbol>();
            string macdCrossover;
            string macdEMA;
            if (filteredSymbols.Any())
            {
                if (userAlertId > 0)
                {
                    var userAlerts = from d in db.TrnUserAlerts where d.Id == userAlertId select d;
                    if (userAlerts.Any())
                    {
                        macdCrossover = userAlerts.First().MACDCrossover;
                        macdEMA = userAlerts.First().MACDEMA;
                        var macdSymbols = from d in filteredSymbols select d;
                        if (macdEMA == "BEFORE")
                        {
                            macdSymbols = from d in filteredSymbols
                                          where macdCrossover == "ALL" ? true : d.MACDPosition == macdCrossover &&
                                                d.MACDLastCrossoverNoOfDays > d.EMALastCrossoverNoOfDays
                                          select d;
                        }
                        else if (macdEMA == "AFTER")
                        {
                            macdSymbols = from d in filteredSymbols
                                          where macdCrossover == "ALL" ? true : d.MACDPosition == macdCrossover &&
                                                d.MACDLastCrossoverNoOfDays <= d.EMALastCrossoverNoOfDays
                                          select d;
                        }
                        else
                        {
                            macdSymbols = from d in filteredSymbols
                                          where macdCrossover == "ALL" ? true : d.MACDPosition == macdCrossover
                                          select d;
                        }
                        foreach (Data.MstSymbol s in macdSymbols)
                        {
                            symbols.Add(s);
                        }
                    }
                }
            }
            return symbols.ToList();
        }
        private List<Data.MstSymbol> GetResultLevelMagentaChannel(long userAlertId, List<Data.MstSymbol> filteredSymbols)
        {
            List<Data.MstSymbol> symbols = new List<Data.MstSymbol>();
            string magentaChannelBegins;
            int magentaChannelCorrelation30;
            int magentaChannelDays;
            decimal magentaChannelAGRADR;
            if (filteredSymbols.Any())
            {
                if (userAlertId > 0)
                {
                    var userAlerts = from d in db.TrnUserAlerts where d.Id == userAlertId select d;
                    if (userAlerts.Any())
                    {
                        magentaChannelBegins = userAlerts.First().MagentaChannelBegins;
                        magentaChannelCorrelation30 = userAlerts.First().MagentaChannelCorrelation30;
                        magentaChannelDays = userAlerts.First().MagentaChannelDays;
                        magentaChannelAGRADR = userAlerts.First().MagentaChannelAGRADR;
                        
                        var magentaChannelSymbols = from d in filteredSymbols select d;

                        if (magentaChannelBegins == "ALL")
                        {
                            magentaChannelSymbols = from d in filteredSymbols
                                                    where d.CorrelationCoefficient30 >= magentaChannelCorrelation30 &&
                                                          d.TrendNoOfDays >= magentaChannelDays &&
                                                          magentaChannelAGRADR >= 0 ? d.GrowthDecayRate >= magentaChannelAGRADR : d.GrowthDecayRate <= magentaChannelAGRADR
                                                    select d;
                        }
                        else if (magentaChannelBegins == "MACD")
                        {
                            magentaChannelSymbols = from d in filteredSymbols
                                                    where d.TrendNoOfDays >= d.MACDLastCrossoverNoOfDays &&
                                                          d.CorrelationCoefficient30 >= magentaChannelCorrelation30 &&
                                                          d.TrendNoOfDays >= d.MACDLastCrossoverNoOfDays &&
                                                          magentaChannelAGRADR >= 0 ? d.GrowthDecayRate >= magentaChannelAGRADR : d.GrowthDecayRate <= magentaChannelAGRADR
                                                    select d;
                        }
                        else if (magentaChannelBegins == "EMA")
                        {
                            magentaChannelSymbols = from d in filteredSymbols
                                                    where d.TrendNoOfDays >= d.EMALastCrossoverNoOfDays &&
                                                          d.CorrelationCoefficient30 >= magentaChannelCorrelation30 &&
                                                          d.TrendNoOfDays >= d.EMALastCrossoverNoOfDays &&
                                                          magentaChannelAGRADR >= 0 ? d.GrowthDecayRate >= magentaChannelAGRADR : d.GrowthDecayRate <= magentaChannelAGRADR
                                                    select d;
                        }
                        foreach (Data.MstSymbol s in magentaChannelSymbols)
                        {
                            symbols.Add(s);
                        }
                    }
                }
            }
            return symbols.ToList();
        }
        private List<Data.MstSymbol> GetResultLevelSeasonality(long userAlertId, List<Data.MstSymbol> filteredSymbols)
        {
            List<Data.MstSymbol> symbols = new List<Data.MstSymbol>();
            string seasonalityTrend;
            decimal seasonalityWinLossPercent;
            decimal seasonalityGainLossPercent;
            if (filteredSymbols.Any())
            {
                if (userAlertId > 0)
                {
                    var userAlerts = from d in db.TrnUserAlerts where d.Id == userAlertId select d;
                    if (userAlerts.Any())
                    {
                        seasonalityTrend = userAlerts.First().SeasonalityTrend == null ? "30" : userAlerts.First().SeasonalityTrend;
                        seasonalityWinLossPercent = userAlerts.First().SeasonalityWinLossPercent;
                        seasonalityGainLossPercent = userAlerts.First().SeasonalityGainLossPercent;

                        var seasonalitySymbols = from d in filteredSymbols
                                                 where convertWinLossToPercent(d.WinLossCurrent30, seasonalityWinLossPercent) && 
                                                       d.WinLossAverageCurrent30 >= seasonalityGainLossPercent
                                                 select d;

                        if (seasonalityTrend == "20")
                        {
                            seasonalitySymbols = from d in filteredSymbols
                                                 where convertWinLossToPercent(d.WinLoss20, seasonalityWinLossPercent) && 
                                                       d.WinLossAverage20 >= seasonalityGainLossPercent
                                                 select d;
                        }
                        else if (seasonalityTrend == "40")
                        {
                            seasonalitySymbols = from d in filteredSymbols
                                                 where convertWinLossToPercent(d.WinLoss40, seasonalityWinLossPercent) && 
                                                       d.WinLossAverage40 >= seasonalityGainLossPercent
                                                 select d;
                        }
                        else if (seasonalityTrend == "60")
                        {
                            seasonalitySymbols = from d in filteredSymbols
                                                 where convertWinLossToPercent(d.WinLoss60, seasonalityWinLossPercent) && 
                                                       d.WinLossAverage60 >= seasonalityGainLossPercent
                                                 select d;
                        }

                        foreach (Data.MstSymbol s in seasonalitySymbols)
                        {
                            symbols.Add(s);
                        }
                    }
                }
            }
            return symbols.ToList();
        }
        private List<Data.MstSymbol> GetResultLevelAdditionalFilter(long userAlertId, List<Data.MstSymbol> filteredSymbols)
        {
            List<Data.MstSymbol> symbols = new List<Data.MstSymbol>();
            decimal additionalFilterPrice;
            decimal additionalFilterVolume;
            int additionalFilterNoOfYears;
            if (filteredSymbols.Any())
            {
                if (userAlertId > 0)
                {
                    var userAlerts = from d in db.TrnUserAlerts where d.Id == userAlertId select d;
                    if (userAlerts.Any())
                    {
                        additionalFilterPrice = userAlerts.First().AdditionalFilterPrice;
                        additionalFilterVolume = userAlerts.First().AdditionalFilterVolume;
                        additionalFilterNoOfYears = userAlerts.First().AdditionalFilterNoOfYears;

                        var additionalFilterSymbols = from d in filteredSymbols
                                                      where d.ClosePrice >= additionalFilterPrice &&
                                                            d.Volume >= additionalFilterVolume &&
                                                            d.NoOfYears >= additionalFilterNoOfYears
                                                      select d;

                        foreach (Data.MstSymbol s in additionalFilterSymbols)
                        {
                            symbols.Add(s);
                        }
                    }
                }
            }
            return symbols.ToList();
        }
        
        // Add User Alert Symbols in the Database
        private void GetResultSymbols(long userAlertId)
        {
            bool symbolFilter;
            bool strategyFilter;
            bool macdFilter;
            bool magentaChannelFilter;
            bool seasonalityFilter;
            bool additionalFilter;

            if (userAlertId > 0)
            {
                var userAlerts = from d in db.TrnUserAlerts where d.Id == userAlertId select d;
                if (userAlerts.Any())
                {
                    // Filter symbols
                    List<Data.MstSymbol> symbolResults = new List<Data.MstSymbol>();
                    symbolFilter = userAlerts.First().SymbolFilter;
                    strategyFilter = userAlerts.First().StrategyFilter;
                    macdFilter = userAlerts.First().MACDFilter;
                    magentaChannelFilter =userAlerts.First().MagentaChannelFilter;
                    seasonalityFilter = userAlerts.First().SeasonalityFilter;
                    additionalFilter = userAlerts.First().AdditionalFilter;

                    if (symbolFilter == true)
                    {
                        symbolResults = GetResultLevelSymbol(userAlertId);
                        if (strategyFilter == true)
                        {
                            symbolResults = GetResultLevelStrategy(userAlertId, symbolResults);
                        }
                        if (macdFilter == true)
                        {
                            symbolResults = GetResultLevelMACD(userAlertId, symbolResults);
                        }
                        if (magentaChannelFilter == true)
                        {
                            symbolResults = GetResultLevelMagentaChannel(userAlertId, symbolResults);
                        }
                        if (seasonalityFilter == true)
                        {
                            symbolResults = GetResultLevelSeasonality(userAlertId, symbolResults);
                        }
                        if (additionalFilter == true)
                        {
                            symbolResults = GetResultLevelAdditionalFilter(userAlertId, symbolResults);
                        }
                    }

                    // Delete existing data
                    var userAlertSymbols = from d in db.TrnUserAlertSymbols where d.UserAlertId == userAlertId select d;
                    if (userAlertSymbols.Any())
                    {
                        foreach (Data.TrnUserAlertSymbol s in userAlertSymbols)
                        {
                            db.TrnUserAlertSymbols.DeleteOnSubmit(s);
                            db.SubmitChanges();
                        }
                    }

                    // Add symbols
                    if (symbolResults.Any())
                    {
                        foreach (var s in symbolResults)
                        {
                            Data.TrnUserAlertSymbol newUserAlertSymbol = new Data.TrnUserAlertSymbol();

                            newUserAlertSymbol.UserAlertId = Convert.ToInt16(userAlertId);
                            newUserAlertSymbol.SymbolId = s.Id;
                            newUserAlertSymbol.Symbol = s.Symbol;
                            newUserAlertSymbol.Trend = "";
                            newUserAlertSymbol.EncodedDate = userAlerts.First().EncodedDate;

                            db.TrnUserAlertSymbols.InsertOnSubmit(newUserAlertSymbol);
                            db.SubmitChanges();
                        }
                    }

                }
            }
        }

        // Get the number of results per filter
        [Authorize]
        [Route("api/GetUserAlertResult/{Id}")]
        public Models.UserAlertSymbolResult GetUserAlertResult(String Id)
        {
            bool symbolFilter;
            bool strategyFilter;
            bool macdFilter;
            bool magentaChannelFilter;
            bool seasonalityFilter;
            bool additionalFilter;

            Id = Id.Replace(",", "");
            int userAlertId = Convert.ToInt32(Id);

            Models.UserAlertSymbolResult result = new Models.UserAlertSymbolResult();

            if (userAlertId > 0)
            {
                var userAlerts = from d in db.TrnUserAlerts where d.Id == userAlertId select d;
                if (userAlerts.Any())
                {
                    // Filter symbols
                    List<Data.MstSymbol> symbolResults = new List<Data.MstSymbol>();
                    symbolFilter = userAlerts.First().SymbolFilter;
                    strategyFilter = userAlerts.First().StrategyFilter;
                    macdFilter = userAlerts.First().MACDFilter;
                    magentaChannelFilter = userAlerts.First().MagentaChannelFilter;
                    seasonalityFilter = userAlerts.First().SeasonalityFilter;
                    additionalFilter = userAlerts.First().AdditionalFilter;

                    result.SymbolFilterResult = 0;
                    result.StrategyFilterResult = 0;
                    result.MACDFilterResult = 0;
                    result.MagentaChannelFilterResult = 0;
                    result.SeasonalityFilterResult = 0;
                    result.AdditionalFilterResult = 0;

                    if (symbolFilter == true)
                    {
                        symbolResults = GetResultLevelSymbol(userAlertId);
                        result.SymbolFilterResult = symbolResults.Count();
                        if (strategyFilter == true)
                        {
                            symbolResults = GetResultLevelStrategy(userAlertId, symbolResults);
                            result.StrategyFilterResult = symbolResults.Count();
                        }
                        if (macdFilter == true)
                        {
                            symbolResults = GetResultLevelMACD(userAlertId, symbolResults);
                            result.MACDFilterResult = symbolResults.Count();
                        }
                        if (magentaChannelFilter == true)
                        {
                            symbolResults = GetResultLevelMagentaChannel(userAlertId, symbolResults);
                            result.MagentaChannelFilterResult = symbolResults.Count();
                        }
                        if (seasonalityFilter == true)
                        {
                            symbolResults = GetResultLevelSeasonality(userAlertId, symbolResults);
                            result.SeasonalityFilterResult = symbolResults.Count();
                        }
                        if (additionalFilter == true)
                        {
                            symbolResults = GetResultLevelAdditionalFilter(userAlertId, symbolResults);
                            result.AdditionalFilterResult = symbolResults.Count();
                        }
                    }
                }
            }

            return result;
        }

        // GET api/GetUserAlert/Username
        [Authorize]
        [Route("api/GetUserAlert/{UserName}")]
        public Models.UserAlert GetUserAlert(string UserName)
        {
            var userAlert = from d in db.TrnUserAlerts
                            where d.MstUser.UserName == UserName
                            select new Models.UserAlert
                            {
                                Id = d.Id,
                                UserId = d.UserId,
                                User = d.MstUser.UserName,
                                Description = d.Description,
                                IsActive = d.IsActive,
                                EncodedDate = Convert.ToString(d.EncodedDate.Year) + "-" + Convert.ToString(d.EncodedDate.Month + 100).Substring(1, 2) + "-" + Convert.ToString(d.EncodedDate.Day + 100).Substring(1, 2),
                                AlertVia = d.AlertVia,
                                SymbolFilter = d.SymbolFilter,
                                SymbolExchange = d.SymbolExchange,
                                SymbolUserFavoritesId = d.SymbolUserFavoritesId == null ? 0 : d.SymbolUserFavoritesId.Value,
                                SymbolUserFavorites = d.SymbolUserFavoritesId == null ? "NA" : db.TrnUserFavorites.Where(f=>f.Id==d.SymbolUserFavoritesId).First().Description,
                                StrategyFilter = d.StrategyFilter,
                                Strategy = d.Strategy,
                                StrategyGrowthDecayRate = d.StrategyGrowthDecayRate,
                                StrategyGrowthDecayTime = d.StrategyGrowthDecayTime,
                                MACDFilter = d.MACDFilter,
                                MACDCrossover = d.MACDCrossover,
                                MACDEMA = d.MACDEMA,
                                MagentaChannelFilter = d.MagentaChannelFilter,
                                MagentaChannelBegins = d.MagentaChannelBegins,
                                MagentaChannelCorrelation30 = d.MagentaChannelCorrelation30,
                                MagentaChannelDays = d.MagentaChannelDays,
                                MagentaChannelAGRADR = d.MagentaChannelAGRADR,
                                SeasonalityFilter = d.SeasonalityFilter,
                                SeasonalityTrend = d.SeasonalityTrend == null ? "30" : d.SeasonalityTrend,
                                SeasonalityWinLossPercent = d.SeasonalityWinLossPercent,
                                SeasonalityGainLossPercent = d.SeasonalityGainLossPercent,
                                AdditionalFilter = d.AdditionalFilter,
                                AdditionalFilterPrice = d.AdditionalFilterPrice,
                                AdditionalFilterVolume = d.AdditionalFilterVolume,
                                AdditionalFilterNoOfYears = d.AdditionalFilterNoOfYears
                            };

            if (userAlert.Any())
            {
                return userAlert.First();
            } else
            {
                return new Models.UserAlert();
            }
        }

        // GET api/GetUserAlertSymbols/Id
        [Authorize]
        [Route("api/GetUserAlertSymbols/{Id}")]
        public List<Models.Symbol> GetUserAlertSymbols(String Id)
        {
            Id = Id.Replace(",", "");
            int id = Convert.ToInt32(Id);

            var symbols = from d in db.TrnUserAlertSymbols where d.UserAlertId == id
                          select new Models.Symbol
                          {
                              SymbolDescription = d.Symbol,
                              Description = d.MstSymbol.Description,
                              Exchange = d.MstSymbol.Exchange,
                              ClosePrice = d.MstSymbol.ClosePrice.Value,
                              Volume = d.MstSymbol.Volume.Value,
                              GrowthDecayRate = d.MstSymbol.GrowthDecayRate.Value == null ? 0 : d.MstSymbol.GrowthDecayRate.Value,
                              GrowthDecayRateW1 = d.MstSymbol.GrowthDecayRateW1.Value == null ? 0 : d.MstSymbol.GrowthDecayRateW1.Value,
                              GrowthDecayRateW2 = d.MstSymbol.GrowthDecayRateW2.Value == null ? 0 : d.MstSymbol.GrowthDecayRateW2.Value,
                              GrowthDecayRateW3 = d.MstSymbol.GrowthDecayRateW3.Value == null ? 0 : d.MstSymbol.GrowthDecayRateW3.Value,
                              GrowthDecayRateM1 = d.MstSymbol.GrowthDecayRateM1.Value == null ? 0 : d.MstSymbol.GrowthDecayRateM1.Value,
                              GrowthDecayRateM2 = d.MstSymbol.GrowthDecayRateM2.Value == null ? 0 : d.MstSymbol.GrowthDecayRateM2.Value,
                              GrowthDecayRateM3 = d.MstSymbol.GrowthDecayRateM3.Value == null ? 0 : d.MstSymbol.GrowthDecayRateM3.Value,
                              NoOfYears = d.MstSymbol.NoOfYears.Value == null ? 0 : d.MstSymbol.NoOfYears.Value,
                              TrendNoOfDays = d.MstSymbol.TrendNoOfDays == null ? 0 : d.MstSymbol.TrendNoOfDays.Value,
                              WinLossCurrent30 = d.MstSymbol.WinLossCurrent30 == null ? "NA" : d.MstSymbol.WinLossCurrent30,
                              WinLossAverageCurrent30 = d.MstSymbol.WinLossAverageCurrent30.Value == null ? 0 : d.MstSymbol.WinLossAverageCurrent30.Value,
                              WinLoss20 = d.MstSymbol.WinLoss20 == null ? "NA" : d.MstSymbol.WinLoss20,
                              WinLossAverage20 = d.MstSymbol.WinLossAverage20.Value == null ? 0 : d.MstSymbol.WinLossAverage20.Value,
                              WinLoss40 = d.MstSymbol.WinLoss40 == null ? "NA" : d.MstSymbol.WinLoss40,
                              WinLossAverage40 = d.MstSymbol.WinLossAverage40.Value == null ? 0 : d.MstSymbol.WinLossAverage40.Value,
                              WinLoss60 = d.MstSymbol.WinLoss60 == null ? "NA" : d.MstSymbol.WinLoss60,
                              WinLossAverage60 = d.MstSymbol.WinLossAverage60.Value == null ? 0 : d.MstSymbol.WinLossAverage60.Value,
                              CorrelationCoefficient30 = d.MstSymbol.CorrelationCoefficient30.Value == null ? 0 : d.MstSymbol.CorrelationCoefficient30.Value,
                              SeasonalityCorrelation = d.MstSymbol.SeasonalityCorrelation.Value == null ? 0 : d.MstSymbol.SeasonalityCorrelation.Value,
                              MACDTrendNoOfDays = d.MstSymbol.MACDTrendNoOfDays.Value == null ? 0 : d.MstSymbol.MACDTrendNoOfDays.Value,
                              MACDGrowthDecayRate = d.MstSymbol.MACDGrowthDecayRate.Value == null ? 0 : d.MstSymbol.MACDGrowthDecayRate.Value,
                              EMATrendNoOfDays = d.MstSymbol.EMATrendNoOfDays.Value == null ? 0 : d.MstSymbol.EMATrendNoOfDays.Value,
                              EMAGrowthDecayRate = d.MstSymbol.EMAGrowthDecayRate.Value == null ? 0 : d.MstSymbol.EMAGrowthDecayRate.Value,
                              EMAStartDate = d.MstSymbol.EMAStartDate == null ? "NA" : Convert.ToString(d.MstSymbol.EMAStartDate.Value.Year).Substring(2, 2) + "-" + Convert.ToString(d.MstSymbol.EMAStartDate.Value.Month + 100).Substring(1, 2) + "-" + Convert.ToString(d.MstSymbol.EMAStartDate.Value.Day + 100).Substring(1, 2),
                              EMALinear = d.MstSymbol.EMALinear.Value == null ? 0 : d.MstSymbol.EMALinear.Value
                          };

            return symbols.ToList();
        }

        // POST api/AddUserAlert
        [Authorize]
        [Route("api/AddUserAlert")]
        public int Post(Models.UserAlert value)
        {
            try
            {
                Data.TrnUserAlert newUserAlert = new Data.TrnUserAlert();

                var userId = (from d in db.MstUsers where d.UserName.Equals(value.User) select d).FirstOrDefault().Id;

                DateTime dt = Convert.ToDateTime(value.EncodedDate);
                SqlDateTime EncodedDate = new SqlDateTime(new DateTime(dt.Year, dt.Month, dt.Day));

                newUserAlert.UserId = userId;
                newUserAlert.Description = value.Description;
                newUserAlert.IsActive = value.IsActive;
                newUserAlert.EncodedDate = EncodedDate.Value;
                newUserAlert.AlertVia = value.AlertVia;

                newUserAlert.SymbolFilter = value.SymbolFilter;
                newUserAlert.SymbolExchange = value.SymbolExchange;
                newUserAlert.SymbolUserFavoritesId = value.SymbolUserFavoritesId;

                newUserAlert.StrategyFilter = value.StrategyFilter;
                newUserAlert.Strategy = value.Strategy;
                newUserAlert.StrategyGrowthDecayRate = value.StrategyGrowthDecayRate;
                newUserAlert.StrategyGrowthDecayTime = value.StrategyGrowthDecayTime;

                newUserAlert.MACDFilter = value.MACDFilter;
                newUserAlert.MACDCrossover = value.MACDCrossover;
                newUserAlert.MACDEMA = value.MACDEMA;

                newUserAlert.MagentaChannelFilter = value.MagentaChannelFilter;
                newUserAlert.MagentaChannelBegins = value.MagentaChannelBegins;
                newUserAlert.MagentaChannelCorrelation30 = value.MagentaChannelCorrelation30;
                newUserAlert.MagentaChannelDays = value.MagentaChannelDays;
                newUserAlert.MagentaChannelAGRADR = value.MagentaChannelAGRADR;

                newUserAlert.SeasonalityFilter = value.SeasonalityFilter;
                newUserAlert.SeasonalityTrend = value.SeasonalityTrend == null ? "30" : value.SeasonalityTrend;
                newUserAlert.SeasonalityWinLossPercent = value.SeasonalityWinLossPercent;
                newUserAlert.SeasonalityGainLossPercent = value.SeasonalityGainLossPercent;

                newUserAlert.AdditionalFilter = value.AdditionalFilter;
                newUserAlert.AdditionalFilterPrice = value.AdditionalFilterPrice;
                newUserAlert.AdditionalFilterVolume = value.AdditionalFilterVolume;
                newUserAlert.AdditionalFilterNoOfYears = value.AdditionalFilterNoOfYears;

                db.TrnUserAlerts.InsertOnSubmit(newUserAlert);
                db.SubmitChanges();

                GetResultSymbols(newUserAlert.Id);

                return newUserAlert.Id;
            }
            catch
            {
                return 0;
            }
        }

        // POST api/UpdateUserAlert/1
        [Authorize]
        [Route("api/UpdateUserAlert/{Id}")]
        public HttpResponseMessage Put(String Id, Models.UserAlert value)
        {
            Id = Id.Replace(",", "");
            int id = Convert.ToInt32(Id);

            try
            {
                var userAlerts = from d in db.TrnUserAlerts where d.Id == id select d;

                if (userAlerts.Any())
                {
                    var updateUserAlert = userAlerts.FirstOrDefault();

                    DateTime dt = Convert.ToDateTime(value.EncodedDate);
                    SqlDateTime EncodedDate = new SqlDateTime(new DateTime(dt.Year, dt.Month, dt.Day));

                    updateUserAlert.Description = value.Description;
                    updateUserAlert.Strategy = value.Strategy;
                    updateUserAlert.IsActive = value.IsActive;
                    updateUserAlert.EncodedDate = EncodedDate.Value;
                    updateUserAlert.AlertVia = value.AlertVia;

                    updateUserAlert.SymbolFilter = value.SymbolFilter;
                    updateUserAlert.SymbolExchange = value.SymbolExchange;
                    updateUserAlert.SymbolUserFavoritesId = value.SymbolUserFavoritesId;

                    updateUserAlert.StrategyFilter = value.StrategyFilter;
                    updateUserAlert.Strategy = value.Strategy;
                    updateUserAlert.StrategyGrowthDecayRate = value.StrategyGrowthDecayRate;
                    updateUserAlert.StrategyGrowthDecayTime = value.StrategyGrowthDecayTime;

                    updateUserAlert.MACDFilter = value.MACDFilter;
                    updateUserAlert.MACDCrossover = value.MACDCrossover;
                    updateUserAlert.MACDEMA = value.MACDEMA;

                    updateUserAlert.MagentaChannelFilter = value.MagentaChannelFilter;
                    updateUserAlert.MagentaChannelBegins = value.MagentaChannelBegins;
                    updateUserAlert.MagentaChannelCorrelation30 = value.MagentaChannelCorrelation30;
                    updateUserAlert.MagentaChannelDays = value.MagentaChannelDays;
                    updateUserAlert.MagentaChannelAGRADR = value.MagentaChannelAGRADR;

                    updateUserAlert.SeasonalityFilter = value.SeasonalityFilter;
                    updateUserAlert.SeasonalityTrend = value.SeasonalityTrend == null ? "30" : value.SeasonalityTrend;
                    updateUserAlert.SeasonalityWinLossPercent = value.SeasonalityWinLossPercent;
                    updateUserAlert.SeasonalityGainLossPercent = value.SeasonalityGainLossPercent;

                    updateUserAlert.AdditionalFilter = value.AdditionalFilter;
                    updateUserAlert.AdditionalFilterPrice = value.AdditionalFilterPrice;
                    updateUserAlert.AdditionalFilterVolume = value.AdditionalFilterVolume;
                    updateUserAlert.AdditionalFilterNoOfYears = value.AdditionalFilterNoOfYears;

                    db.SubmitChanges();

                    GetResultSymbols(updateUserAlert.Id);

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (NullReferenceException)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

    }
}
