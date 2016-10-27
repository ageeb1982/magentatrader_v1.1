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

        // Add User Alert Symbols in the Database
        private void GetResultSymbols(long userAlertId)
        {
            string strategy;
            string exchange;
            decimal price;
            decimal volume;
            decimal growthDecayRate;
            string growthDecayTime;
            int noOfYears;
            decimal correlation30;

            if (userAlertId > 0)
            {
                var userAlerts = from d in db.TrnUserAlerts where d.Id == userAlertId select d;
                if (userAlerts.Any())
                {
                    strategy = userAlerts.First().Strategy;
                    exchange = userAlerts.First().Exchange;
                    price = userAlerts.First().Price;
                    volume = userAlerts.First().Volume;
                    growthDecayRate = userAlerts.First().GrowthDecayRate;
                    growthDecayTime = userAlerts.First().GrowthDecayTime;
                    noOfYears = userAlerts.First().NoOfYears;
                    correlation30 = userAlerts.First().Correlation30;

                    var symbols = from d in db.MstSymbols where d.Exchange == "NA" select d;
                    if (strategy == "MED")
                    {
                        // Magenta Early Down
                        symbols = from d in db.MstSymbols
                                  where (exchange == "All" ? true : d.Exchange == exchange) &&
                                            (d.ClosePrice >= price) &&
                                            (d.Exchange == "FOREX" ? true : d.Volume >= volume) &&
                                            (d.NoOfYears >= noOfYears) &&
                                            (d.MACDGrowthDecayRate < 0) &&
                                            (d.EMAGrowthDecayRate < 0)
                                  select d;
                    }
                    else if (strategy == "MEU")
                    {
                        // Magenta Early Up
                        symbols = from d in db.MstSymbols
                                  where (exchange == "All" ? true : d.Exchange == exchange) &&
                                                (d.ClosePrice >= price) &&
                                                (d.Exchange == "FOREX" ? true : d.Volume >= volume) &&
                                                (d.NoOfYears >= noOfYears) &&
                                                (d.MACDGrowthDecayRate >= 0) &&
                                                (d.EMAGrowthDecayRate >= 0)
                                  select d;
                    }
                    else
                    {
                        // Customized
                        symbols = from d in db.MstSymbols
                                  where (exchange == "All" ? true : d.Exchange == exchange) &&
                                            (d.ClosePrice >= price) &&
                                            (d.Exchange == "FOREX" ? true : d.Volume >= volume) &&
                                            (d.NoOfYears >= noOfYears) &&
                                            (d.CorrelationCoefficient30 >= (correlation30 / 100)) &&
                                            (growthDecayTime == "C0" && growthDecayRate >= 0 ? ((d.GrowthDecayRate.Value == null ? 0 : d.GrowthDecayRate.Value) >= growthDecayRate ? true : false) : true) == true &&
                                            (growthDecayTime == "C0" && growthDecayRate < 0 ? (growthDecayRate >= (d.GrowthDecayRate.Value == null ? 0 : d.GrowthDecayRate.Value) ? true : false) : true) == true &&
                                            (growthDecayTime == "W1" && growthDecayRate >= 0 ? ((d.GrowthDecayRateW1.Value == null ? 0 : d.GrowthDecayRateW1.Value) >= growthDecayRate ? true : false) : true) == true &&
                                            (growthDecayTime == "W1" && growthDecayRate < 0 ? (growthDecayRate >= (d.GrowthDecayRateW1.Value == null ? 0 : d.GrowthDecayRateW1.Value) ? true : false) : true) == true &&
                                            (growthDecayTime == "W2" && growthDecayRate >= 0 ? ((d.GrowthDecayRateW2.Value == null ? 0 : d.GrowthDecayRateW2.Value) >= growthDecayRate ? true : false) : true) == true &&
                                            (growthDecayTime == "W2" && growthDecayRate < 0 ? (growthDecayRate >= (d.GrowthDecayRateW2.Value == null ? 0 : d.GrowthDecayRateW2.Value) ? true : false) : true) == true &&
                                            (growthDecayTime == "W3" && growthDecayRate >= 0 ? ((d.GrowthDecayRateW3.Value == null ? 0 : d.GrowthDecayRateW3.Value) >= growthDecayRate ? true : false) : true) == true &&
                                            (growthDecayTime == "W3" && growthDecayRate < 0 ? (growthDecayRate >= (d.GrowthDecayRateW3.Value == null ? 0 : d.GrowthDecayRateW3.Value) ? true : false) : true) == true &&
                                            (growthDecayTime == "M1" && growthDecayRate >= 0 ? ((d.GrowthDecayRateM1.Value == null ? 0 : d.GrowthDecayRateM1.Value) >= growthDecayRate ? true : false) : true) == true &&
                                            (growthDecayTime == "M1" && growthDecayRate < 0 ? (growthDecayRate >= (d.GrowthDecayRateM1.Value == null ? 0 : d.GrowthDecayRateM1.Value) ? true : false) : true) == true &&
                                            (growthDecayTime == "M2" && growthDecayRate >= 0 ? ((d.GrowthDecayRateM2.Value == null ? 0 : d.GrowthDecayRateM2.Value) >= growthDecayRate ? true : false) : true) == true &&
                                            (growthDecayTime == "M2" && growthDecayRate < 0 ? (growthDecayRate >= (d.GrowthDecayRateM2.Value == null ? 0 : d.GrowthDecayRateM2.Value) ? true : false) : true) == true &&
                                            (growthDecayTime == "M3" && growthDecayRate >= 0 ? ((d.GrowthDecayRateM3.Value == null ? 0 : d.GrowthDecayRateM3.Value) >= growthDecayRate ? true : false) : true) == true &&
                                            (growthDecayTime == "M3" && growthDecayRate < 0 ? (growthDecayRate >= (d.GrowthDecayRateM3.Value == null ? 0 : d.GrowthDecayRateM3.Value) ? true : false) : true) == true
                                  select d;
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

                    if (symbols.Any())
                    {
                        // Add symbols
                        foreach (var s in symbols)
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
                                Strategy = d.Strategy,
                                Exchange = d.Exchange,
                                Price = d.Price,
                                Volume = d.Volume,
                                GrowthDecayRate = d.GrowthDecayRate,
                                GrowthDecayTime = d.GrowthDecayTime,
                                NoOfYears = d.NoOfYears,
                                Correlation30 = d.Correlation30,
                                MACDOccurrence = d.MACDOccurrence,
                                IsActive = d.IsActive,
                                EncodedDate = Convert.ToString(d.EncodedDate.Year) + "-" + Convert.ToString(d.EncodedDate.Month + 100).Substring(1, 2) + "-" + Convert.ToString(d.EncodedDate.Day + 100).Substring(1, 2),
                                AlertVia = d.AlertVia
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
                newUserAlert.Strategy = value.Strategy;
                newUserAlert.Exchange = value.Exchange;
                newUserAlert.Price = value.Price;
                newUserAlert.Volume = value.Volume;
                newUserAlert.GrowthDecayRate = value.GrowthDecayRate;
                newUserAlert.GrowthDecayTime = value.GrowthDecayTime;
                newUserAlert.NoOfYears = value.NoOfYears;
                newUserAlert.Correlation30 = value.Correlation30;
                newUserAlert.MACDOccurrence = value.MACDOccurrence;
                newUserAlert.IsActive = value.IsActive;
                newUserAlert.EncodedDate = EncodedDate.Value;
                newUserAlert.AlertVia = value.AlertVia;

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
                    updateUserAlert.Exchange = value.Exchange;
                    updateUserAlert.Price = value.Price;
                    updateUserAlert.Volume = value.Volume;
                    updateUserAlert.GrowthDecayRate = value.GrowthDecayRate;
                    updateUserAlert.GrowthDecayTime = value.GrowthDecayTime;
                    updateUserAlert.NoOfYears = value.NoOfYears;
                    updateUserAlert.Correlation30 = value.Correlation30;
                    updateUserAlert.MACDOccurrence = value.MACDOccurrence;
                    updateUserAlert.IsActive = value.IsActive;
                    updateUserAlert.EncodedDate = EncodedDate.Value;
                    updateUserAlert.AlertVia = value.AlertVia;

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
