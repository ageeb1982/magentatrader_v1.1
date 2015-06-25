using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MagentaTrader.Controllers
{
    public class SymbolController : ApiController
    {
        private Data.MagentaTradersDBDataContext db = new Data.MagentaTradersDBDataContext();

        // GET api/SymbolCalendar/1/2015/80/10.5/10/1000000
        [Authorize]
        [Route("api/SymbolCalendar/{Month}/{Year}/{Delta}/{Percentage}/{Price}/{Volume}")]
        public Models.CalendarSymbolWrapper GetSymbolCalendar(int Month, int Year, decimal Delta, decimal Percentage, decimal Price, decimal Volume)
        {
            var retryCounter = 0;

            List<Models.Symbol> upValues;
            List<Models.Symbol> downValues;

            while (true)
            {
                try
                {
                    // Up
                    var UpSymbols = from d in db.MstSymbols
                                    where (d.CalendarUpDate.Value.Month == Month) &&
                                          (d.CalendarUpDate.Value.Year == Year) &&
                                          (d.CalendarUpDelta >= Delta) &&
                                          (d.CalendarUpPercentage >= Percentage) && 
                                          (d.ClosePrice >= Price) && 
                                          (d.Volume >= Volume)
                                    select new Models.Symbol
                                    {
                                       SymbolDescription = d.Symbol,
                                       CalendarUpDate = d.CalendarUpDate.Value.ToShortDateString(),
                                       CalendarUpDay = d.CalendarUpDate.Value.Day,
                                       CalendarUpDelta = d.CalendarUpDelta.Value,
                                       CalendarUpPercentage = d.CalendarUpPercentage.Value
                                    };
                    if (UpSymbols.Count() > 0) {
                        upValues = UpSymbols.ToList();
                    } else {
                        upValues = new List<Models.Symbol>();
                    }

                    // Down
                    var DownSymbols = from d in db.MstSymbols
                                    where (d.CalendarDownDate.Value.Month == Month) &&
                                          (d.CalendarDownDate.Value.Year == Year) &&
                                          (d.CalendarDownDelta >= Delta) &&
                                          (d.CalendarDownPercentage >= Percentage) &&
                                          (d.ClosePrice >= Price) &&
                                          (d.Volume >= Volume)
                                    select new Models.Symbol
                                    {
                                        SymbolDescription = d.Symbol,
                                        CalendarDownDate = d.CalendarDownDate.Value.ToShortDateString(),
                                        CalendarDownDay = d.CalendarDownDate.Value.Day,
                                        CalendarDownDelta = d.CalendarDownDelta.Value,
                                        CalendarDownPercentage = d.CalendarDownPercentage.Value
                                    };
                    if (DownSymbols.Count() > 0)
                    {
                        downValues = DownSymbols.ToList();
                    }
                    else
                    {
                        downValues = new List<Models.Symbol>();
                    }

                    break;
                }
                catch
                {
                    if (retryCounter == 3)
                    {
                        upValues = new List<Models.Symbol>();
                        downValues = new List<Models.Symbol>();
                        break;
                    }

                    System.Threading.Thread.Sleep(1000);
                    retryCounter++;
                }
            }

            Models.CalendarSymbolWrapper values = new Models.CalendarSymbolWrapper();
            values.UpSymbols = upValues.ToList();
            values.DownSymbols = downValues.ToList();
            return values;
        }

        // GET api/SymbolScreener/NYSE/10/1000000/100/C0/8/0.80
        [Authorize]
        [Route("api/SymbolScreener/{Exchange}/{Price}/{Volume}/{GrowthDecayRate}/{GrowthDecayTime}/{NoOfYears}/{Correlation30}")]
        public List<Models.Symbol> GetSymbolScreener(string Exchange, decimal Price, decimal Volume, decimal GrowthDecayRate, string GrowthDecayTime, int NoOfYears, decimal Correlation30)
        {
            var retryCounter = 0;

            List<Models.Symbol> Values;

            while (true)
            {
                try 
                {
                    var Symbols = from d in db.MstSymbols
                                  where (Exchange == "All" ? true : d.Exchange == Exchange) &&
                                        (d.ClosePrice >= Price) &&
                                        (d.Volume >= Volume) &&
                                        (d.NoOfYears >= NoOfYears) &&
                                        (d.CorrelationCoefficient30 >= (Correlation30/100)) && 
                                        (GrowthDecayTime == "C0" && GrowthDecayRate >= 0 ? ((d.GrowthDecayRate.Value == null ? 0 : d.GrowthDecayRate.Value) >= GrowthDecayRate ? true : false) : true) == true &&
                                        (GrowthDecayTime == "C0" && GrowthDecayRate < 0 ? (GrowthDecayRate >= (d.GrowthDecayRate.Value == null ? 0 : d.GrowthDecayRate.Value) ? true : false) : true) == true &&
                                        (GrowthDecayTime == "W1" && GrowthDecayRate >= 0 ? ((d.GrowthDecayRateW1.Value == null ? 0 : d.GrowthDecayRateW1.Value) >= GrowthDecayRate ? true : false) : true) == true &&
                                        (GrowthDecayTime == "W1" && GrowthDecayRate < 0 ? (GrowthDecayRate >= (d.GrowthDecayRateW1.Value == null ? 0 : d.GrowthDecayRateW1.Value) ? true : false) : true) == true &&
                                        (GrowthDecayTime == "W2" && GrowthDecayRate >= 0 ? ((d.GrowthDecayRateW2.Value == null ? 0 : d.GrowthDecayRateW2.Value) >= GrowthDecayRate ? true : false) : true) == true &&
                                        (GrowthDecayTime == "W2" && GrowthDecayRate < 0 ? (GrowthDecayRate >= (d.GrowthDecayRateW2.Value == null ? 0 : d.GrowthDecayRateW2.Value) ? true : false) : true) == true &&
                                        (GrowthDecayTime == "W3" && GrowthDecayRate >= 0 ? ((d.GrowthDecayRateW3.Value == null ? 0 : d.GrowthDecayRateW3.Value) >= GrowthDecayRate ? true : false) : true) == true &&
                                        (GrowthDecayTime == "W3" && GrowthDecayRate < 0 ? (GrowthDecayRate >= (d.GrowthDecayRateW3.Value == null ? 0 : d.GrowthDecayRateW3.Value) ? true : false) : true) == true &&
                                        (GrowthDecayTime == "M1" && GrowthDecayRate >= 0 ? ((d.GrowthDecayRateM1.Value == null ? 0 : d.GrowthDecayRateM1.Value) >= GrowthDecayRate ? true : false) : true) == true &&
                                        (GrowthDecayTime == "M1" && GrowthDecayRate < 0 ? (GrowthDecayRate >= (d.GrowthDecayRateM1.Value == null ? 0 : d.GrowthDecayRateM1.Value) ? true : false) : true) == true &&
                                        (GrowthDecayTime == "M2" && GrowthDecayRate >= 0 ? ((d.GrowthDecayRateM2.Value == null ? 0 : d.GrowthDecayRateM2.Value) >= GrowthDecayRate ? true : false) : true) == true &&
                                        (GrowthDecayTime == "M2" && GrowthDecayRate < 0 ? (GrowthDecayRate >= (d.GrowthDecayRateM2.Value == null ? 0 : d.GrowthDecayRateM2.Value) ? true : false) : true) == true &&
                                        (GrowthDecayTime == "M3" && GrowthDecayRate >= 0 ? ((d.GrowthDecayRateM3.Value == null ? 0 : d.GrowthDecayRateM3.Value) >= GrowthDecayRate ? true : false) : true) == true &&
                                        (GrowthDecayTime == "M3" && GrowthDecayRate < 0 ? (GrowthDecayRate >= (d.GrowthDecayRateM3.Value == null ? 0 : d.GrowthDecayRateM3.Value) ? true : false) : true) == true
                                  select new Models.Symbol
                                  {
                                      SymbolDescription = d.Symbol,
                                      Description = d.Description,
                                      Exchange = d.Exchange,
                                      ClosePrice = d.ClosePrice.Value,
                                      Volume = d.Volume.Value,
                                      GrowthDecayRate = d.GrowthDecayRate.Value == null ? 0 : d.GrowthDecayRate.Value,
                                      GrowthDecayRateW1 = d.GrowthDecayRateW1.Value == null ? 0 : d.GrowthDecayRateW1.Value,
                                      GrowthDecayRateW2 = d.GrowthDecayRateW2.Value == null ? 0 : d.GrowthDecayRateW2.Value,
                                      GrowthDecayRateW3 = d.GrowthDecayRateW3.Value == null ? 0 : d.GrowthDecayRateW3.Value,
                                      GrowthDecayRateM1 = d.GrowthDecayRateM1.Value == null ? 0 : d.GrowthDecayRateM1.Value,
                                      GrowthDecayRateM2 = d.GrowthDecayRateM2.Value == null ? 0 : d.GrowthDecayRateM2.Value,
                                      GrowthDecayRateM3 = d.GrowthDecayRateM3.Value == null ? 0 : d.GrowthDecayRateM3.Value,
                                      NoOfYears = d.NoOfYears.Value == null ? 0 : d.NoOfYears.Value,
                                      TrendNoOfDays = d.TrendNoOfDays == null ? 0 : d.TrendNoOfDays.Value,
                                      WinLossCurrent30 = d.WinLossCurrent30 == null ? "NA" : d.WinLossCurrent30,
                                      WinLossAverageCurrent30 = d.WinLossAverageCurrent30.Value == null ? 0 : d.WinLossAverageCurrent30.Value,
                                      WinLoss20 = d.WinLoss20 == null ? "NA" : d.WinLoss20,
                                      WinLossAverage20 = d.WinLossAverage20.Value == null ? 0 : d.WinLossAverage20.Value,
                                      WinLoss40 = d.WinLoss40 == null ? "NA" : d.WinLoss40,
                                      WinLossAverage40 = d.WinLossAverage40.Value == null ? 0 : d.WinLossAverage40.Value,
                                      WinLoss60 = d.WinLoss60 == null ? "NA" : d.WinLoss60,
                                      WinLossAverage60 = d.WinLossAverage60.Value == null ? 0 : d.WinLossAverage60.Value,
                                      CorrelationCoefficient30 = d.CorrelationCoefficient30.Value,
                                      SeasonalityCorrelation = d.SeasonalityCorrelation.Value
                                  };
                    if (Symbols.Count() > 0)
                    {
                        Values = Symbols.ToList();
                    }
                    else
                    {
                        Values = new List<Models.Symbol>();
                    }
                    break;
                }
                catch
                {
                    if (retryCounter == 3)
                    {
                        Values = new List<Models.Symbol>();
                        break;
                    }

                    System.Threading.Thread.Sleep(1000);
                    retryCounter++;
                }
            }
            return Values;
        }
    }
}
