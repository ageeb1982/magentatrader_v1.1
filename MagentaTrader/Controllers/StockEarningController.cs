using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MagentaTrader.Controllers
{
    public class StockEarningController : ApiController
    {
        private Data.MagentaTradersDBDataContext db = new Data.MagentaTradersDBDataContext();

        public static int GetQuarter(DateTime dateTime)
        {
            if (dateTime.Month <= 3) return 1;
            if (dateTime.Month <= 6) return 2;
            if (dateTime.Month <= 9) return 3;

            return 4;
        }

        // GET api/StockEarning/FB
        [Authorize]
        public List<Models.StockEarning> Get(string symbol)
        {
            List<Models.StockEarning> earnings = new List<Models.StockEarning>();

            try
            {
                var Symbols = from d in db.MstSymbols where d.Symbol == symbol select d;

                DateTime date = Symbols.FirstOrDefault().LatestQuoteDate.Value;
                DateTime date1 = date.AddMonths(-6);
                DateTime date2 = date.AddMonths(6);

                earnings = (from d in db.TrnStockEarnings
                            where (d.Symbol == symbol) &&
                                  (d.EarningDate >= date1 && d.EarningDate <= date2)
                            orderby d.EarningDate descending
                            select new Models.StockEarning
                            {
                                EarningDate = Convert.ToString(d.EarningDate.Year) + "-" + Convert.ToString(d.EarningDate.Month + 100).Substring(1, 2) + "-" + Convert.ToString(d.EarningDate.Day + 100).Substring(1, 2),
                                Symbol = d.Symbol
                            }).ToList();
            }
            catch
            {
                earnings = new List<Models.StockEarning>();
            }

            return earnings.ToList();
        }

        // GET api/StockEarningHistory/FB/2015-01-01
        [Authorize]
        [Route("api/GetZacksEarningHistory/{Symbol}")]
        public HttpResponseMessage GetZacksEarnings(string symbol)
        {
            try
            {
                WebRequest req = HttpWebRequest.Create("https://www.zacks.com/stock/research/" + symbol + "/earnings-announcements");
                req.Method = "GET";

                string source = "";
                string parseSource = "";
                string earningString = "";
                bool continueEarningString = false;
                long i = 1;

                using (StreamReader reader = new StreamReader(req.GetResponse().GetResponseStream())) { source = reader.ReadToEnd(); }

                foreach (char c in source)
                {
                    if (i > 4)
                    {
                        if (parseSource == "[  [")
                        {
                            earningString = parseSource;
                            continueEarningString = true;
                        }
                        else if (parseSource == "]  ]")
                        {
                            var array = JArray.Parse(earningString);
                            List<Models.StockEarning> earnings = new List<Models.StockEarning>();
                            var symbols = from d in db.MstSymbols where d.Symbol == symbol select d;
                            DateTime maxDate = DateTime.MinValue;
                            DateTime minDate = DateTime.MinValue;
                            if (symbols.Any()) {
                                foreach (var a in array)
                                {
                                    DateTime earningDate = Convert.ToDateTime(a[0].ToString());
                                    string periodEnding = a[1].ToString();
                                    Decimal estimatedValue = a[2].ToString() == "--" ? Decimal.Parse("0") : Decimal.Parse(a[2].ToString().Replace("$", ""));
                                    Decimal reportedValue = a[3].ToString() == "--" ? Decimal.Parse("0") : Decimal.Parse(a[3].ToString().Replace("$", ""));
                                    string earningTime = a[5].ToString() == "--" ? "Before Open" : a[5].ToString();
                                    if (earningTime == "Before Open")
                                    {
                                        earningTime = "Before Market Open";
                                    }
                                    else
                                    {
                                        earningTime = "After Market Close";
                                    }

                                    Models.StockEarning e = new Models.StockEarning();
                                    e.SymbolId = symbols.FirstOrDefault().Id;
                                    e.Symbol = symbols.FirstOrDefault().Symbol;
                                    e.EarningDate = a[0].ToString();
                                    e.EarningTime = earningTime;
                                    e.PeriodEnding = periodEnding;
                                    e.EstimatedValue = estimatedValue;
                                    e.ReportedValue = reportedValue;
                                    earnings.Add(e);

                                    if (maxDate == DateTime.MinValue) maxDate = earningDate;
                                    else if (earningDate > maxDate) maxDate = earningDate;

                                    if (minDate == DateTime.MinValue) minDate = earningDate;
                                    else if (earningDate < minDate) minDate = earningDate;
                                }

                                if (earnings.Any())
                                {
                                    // Clean existing stock earning schedule records
                                    var stockEarnings = from d in db.TrnStockEarnings 
                                                        where d.Symbol == symbols.FirstOrDefault().Symbol && 
                                                              (d.EarningDate >= minDate && d.EarningDate <= maxDate) 
                                                        select d;
                                    if (stockEarnings.Any())
                                    {
                                        foreach (var se in stockEarnings)
                                        {
                                            se.Symbol = "XXX-" + se.Symbol;
                                            db.SubmitChanges();
                                        }
                                    }
                                    // Add new stock earning schedule records from Zacks
                                    foreach (Models.StockEarning e in earnings)
                                    {
                                        Data.TrnStockEarning newEarnings = new Data.TrnStockEarning();

                                        DateTime dt = Convert.ToDateTime(e.EarningDate);
                                        SqlDateTime earningDate = new SqlDateTime(new DateTime(dt.Year, dt.Month, dt.Day));

                                        newEarnings.Symbol = e.Symbol;
                                        newEarnings.SymbolId = e.SymbolId;
                                        newEarnings.EarningDate = earningDate.Value;
                                        newEarnings.EarningTime = e.EarningTime;
                                        newEarnings.PeriodEnding = e.PeriodEnding;
                                        newEarnings.EstimatedValue = e.EstimatedValue;
                                        newEarnings.ReportedValue = e.ReportedValue;

                                        db.TrnStockEarnings.InsertOnSubmit(newEarnings);
                                        db.SubmitChanges();
                                    }
                                }

                                return Request.CreateResponse(HttpStatusCode.OK);
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.NotFound);
                            }
                        }
                        else
                        {
                            if (continueEarningString == true)
                            {
                                earningString = earningString + c;
                            }
                        }
                        parseSource = parseSource.Substring(1, 3) + c;
                    }
                    else
                    {
                        if (parseSource.Length == 4)
                        {
                            parseSource = parseSource.Substring(1, 3) + c;
                        }
                        else
                        {
                            parseSource = parseSource + c;
                        }
                    }
                    i++;
                }

                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // GET api/StockEarningHistory/FB/2015-01-01
        [Authorize]
        [Route("api/StockEarningHistory/{Symbol}/{EarningDate}")]
        public List<Models.StockEarning> GetEarningHistory(string symbol, string earningDate)
        {
            List<Models.StockEarning> earnings = new List<Models.StockEarning>();

            try
            {
                var Symbols = from d in db.MstSymbols where d.Symbol == symbol select d;
                CultureInfo provider = CultureInfo.InvariantCulture;

                DateTime date = DateTime.ParseExact(earningDate, "yyyy-MM-dd",provider);
                int quarter = GetQuarter(date);

                // (((d.EarningDate.Month - 1) / 3) == (quarter-1)) &&

                earnings = (from d in db.TrnStockEarnings
                            where (d.Symbol == symbol) &&
                                  (date.Month > 1 ? (Math.Abs((date.Month - d.EarningDate.Month)) < 2) : (Math.Abs((date.Month - (d.EarningDate.Month == 12 ? 1 : d.EarningDate.Month))) < 2)) &&
                                  (date > d.EarningDate)
                            orderby d.EarningDate descending
                            select new Models.StockEarning
                            {
                                Symbol = d.Symbol,
                                EarningDate = Convert.ToString(d.EarningDate.Year) + "-" + Convert.ToString(d.EarningDate.Month + 100).Substring(1, 2) + "-" + Convert.ToString(d.EarningDate.Day + 100).Substring(1, 2),
                                EarningTime = d.EarningTime
                            }).ToList();
            }
            catch
            {
                earnings = new List<Models.StockEarning>();
            }

            return earnings.ToList();
        }

        // GET api/StockEarningHistoryChronologically/FB/2015-01-01
        [Authorize]
        [Route("api/StockEarningHistoryChronologically/{Symbol}/{EarningDate}")]
        public List<Models.StockEarning> GetEarningHistoryChronologically(string symbol, string earningDate)
        {
            List<Models.StockEarning> earnings = new List<Models.StockEarning>();

            try
            {
                var Symbols = from d in db.MstSymbols where d.Symbol == symbol select d;
                CultureInfo provider = CultureInfo.InvariantCulture;

                DateTime date = DateTime.ParseExact(earningDate, "yyyy-MM-dd", provider);
                int quarter = GetQuarter(date);

                earnings = (from d in db.TrnStockEarnings
                            where (d.Symbol == symbol) &&
                                  (date > d.EarningDate)
                            orderby d.EarningDate descending
                            select new Models.StockEarning
                            {
                                Symbol = d.Symbol,
                                EarningDate = Convert.ToString(d.EarningDate.Year) + "-" + Convert.ToString(d.EarningDate.Month + 100).Substring(1, 2) + "-" + Convert.ToString(d.EarningDate.Day + 100).Substring(1, 2),
                                EarningTime = d.EarningTime
                            }).ToList();
            }
            catch
            {
                earnings = new List<Models.StockEarning>();
            }

            return earnings.ToList();
        }

        // GET api/StockEarningYear/FB/2015
        [Authorize]
        [Route("api/StockEarningYear/{Symbol}/{Year}")]
        public List<Models.StockEarning> GetStockEarningYear(string symbol, int year)
        {
            List<Models.StockEarning> earnings = new List<Models.StockEarning>();
            try
            {
                earnings = (from d in db.TrnStockEarnings
                            where (d.Symbol == symbol) &&
                                  (d.EarningDate.Year == year)
                            orderby d.EarningDate descending
                            select new Models.StockEarning
                            {
                                Id = d.Id,
                                SymbolId = d.SymbolId,
                                Symbol = d.Symbol,
                                EarningDate = Convert.ToString(d.EarningDate.Year) + "-" + Convert.ToString(d.EarningDate.Month + 100).Substring(1, 2) + "-" + Convert.ToString(d.EarningDate.Day + 100).Substring(1, 2),
                                EarningTime = d.EarningTime
                            }).ToList();
            }
            catch
            {
                earnings = new List<Models.StockEarning>();
            }

            return earnings;
        }

        // POST api/AddStockEarning
        [Authorize]
        [Route("api/AddStockEarning")]
        public int Post(Models.StockEarning value)
        {
            try
            {

                Data.TrnStockEarning NewEarnings = new Data.TrnStockEarning();

                DateTime dt = Convert.ToDateTime(value.EarningDate);
                SqlDateTime EarningDate = new SqlDateTime(new DateTime(dt.Year,dt.Month,dt.Day));

                NewEarnings.SymbolId = db.MstSymbols.Where(d => d.Symbol == value.Symbol).First().Id;
                NewEarnings.Symbol = value.Symbol;
                NewEarnings.EarningDate = EarningDate.Value;
                NewEarnings.EarningTime = value.EarningTime;

                db.TrnStockEarnings.InsertOnSubmit(NewEarnings);
                db.SubmitChanges();

                return NewEarnings.Id;
            }
            catch
            {
                return 0;
            }
        }

        // PUT /api/UpdateStockEarning/5
        [Authorize]
        [Route("api/UpdateStockEarning/{Id}")]
        public HttpResponseMessage Put(String Id, Models.StockEarning value)
        {
            Id = Id.Replace(",", "");
            int id = Convert.ToInt32(Id);

            try
            {
                var StockEarnings = from d in db.TrnStockEarnings where d.Id == id select d;

                if (StockEarnings.Any())
                {
                    var UpdatedStockEarnings = StockEarnings.FirstOrDefault();

                    DateTime dt = Convert.ToDateTime(value.EarningDate);
                    SqlDateTime EarningDate = new SqlDateTime(new DateTime(dt.Year, dt.Month, dt.Day));

                    UpdatedStockEarnings.SymbolId = db.MstSymbols.Where(d => d.Symbol == value.Symbol).First().Id;
                    UpdatedStockEarnings.Symbol = value.Symbol;
                    UpdatedStockEarnings.EarningDate = EarningDate.Value;
                    UpdatedStockEarnings.EarningTime = value.EarningTime;

                    db.SubmitChanges();

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

        // DELETE api/DeleteStockEarning/5
        [Authorize]
        [Route("api/DeleteStockEarning/{Id}")]
        public HttpResponseMessage Delete(int Id)
        {
            Data.TrnStockEarning DeleteStockEarning = db.TrnStockEarnings.Where(d => d.Id == Id).First();

            if (DeleteStockEarning != null)
            {
                db.TrnStockEarnings.DeleteOnSubmit(DeleteStockEarning);
                try
                {
                    db.SubmitChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
        }
    }
}
