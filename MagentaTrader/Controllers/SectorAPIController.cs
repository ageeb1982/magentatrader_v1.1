using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MagentaTrader.Controllers
{
    public class SectorAPIController : ApiController
    {
        private Data.MagentaTradersDBDataContext db = new Data.MagentaTradersDBDataContext();

        // Sector

        [Authorize]
        [HttpGet]
        [Route("api/Sector")]
        public List<Models.Sector> Get()
        {
            var retryCounter = 0;
            List<Models.Sector> values;

            while (true)
            {
                try
                {
                    var Sectors = from d in db.TrnSectors
                                  orderby d.Sector 
                                  select new Models.Sector
                                  {
                                     Id = d.Id,
                                     SectorCode = d.Sector,
                                     SectorDefinition = d.Definition
                                  };
                    if (Sectors.Count() > 0)
                    {
                        values = Sectors.ToList();
                    }
                    else
                    {
                        values = new List<Models.Sector>();
                    }
                    break;
                }
                catch
                {
                    if (retryCounter == 3)
                    {
                        values = new List<Models.Sector>();
                        break;
                    }

                    System.Threading.Thread.Sleep(1000);
                    retryCounter++;
                }
            }
            return values;
        }

        [Authorize]
        [HttpGet]
        [Route("api/SectorPrice/{Sector}")]
        public Models.SectorPriceWrapper GetSectorPrice(String Sector)
        {
            List<Models.SectorPrice> prices = new List<Models.SectorPrice>();

            string sectorDescription = "";
            int symbolCounter;

            String[] QuoteDate = new String[3000];
            decimal[] OpenPrice = new decimal[3000];
            decimal[] HighPrice = new decimal[3000];
            decimal[] LowPrice = new decimal[3000];
            decimal[] ClosePrice = new decimal[3000];
            decimal[] Volume = new decimal[3000];

            try
            {
                var sectors = from d in db.TrnSectors where d.Sector == Sector select d;

                if (sectors.Any())
                {
                    sectorDescription = sectors.First().Definition;
                    var symbols = from d in sectors.First().TrnSectorSymbols select d;
                    if(symbols.Any()) {
                        DateTime date2 = symbols.First().MstSymbol.LatestQuoteDate.Value;
                        DateTime date1 = ((date2.AddDays(1)).AddYears(-10)).AddMonths(-5);
                        symbolCounter = 0;
                        Models.SectorPrice price = new Models.SectorPrice();
                        foreach (var s in symbols)
                        {
                            var symbolPrices = (from d in db.TrnStockPrices
                                               where (d.Symbol == s.Symbol) &&
                                                   (d.QuoteDate >= date1 && d.QuoteDate <= date2)
                                               orderby d.QuoteDate descending
                                               select new Models.StockPrice
                                               {
                                                   QuoteDate = Convert.ToString(d.QuoteDate.Year) + "-" + Convert.ToString(d.QuoteDate.Month + 100).Substring(1, 2) + "-" + Convert.ToString(d.QuoteDate.Day + 100).Substring(1, 2),
                                                   OpenPrice = d.OpenPrice,
                                                   HighPrice = d.HighPrice,
                                                   LowPrice = d.LowPrice,
                                                   ClosePrice = d.ClosePrice,
                                                   Volume = d.Volume
                                               }).ToList();

                            int i = 0;
                            foreach (var p in symbolPrices)
                            {
                                QuoteDate[i] = p.QuoteDate;
                                OpenPrice[i] = (OpenPrice[i] + p.OpenPrice);
                                HighPrice[i] = (HighPrice[i] + p.HighPrice);
                                LowPrice[i] = (LowPrice[i] + p.LowPrice);
                                ClosePrice[i] = (ClosePrice[i] + p.ClosePrice);
                                Volume[i] = (Volume[i] + p.Volume);

                                i++;
                            }

                            symbolCounter++;
                        }

                        for (int i = 0; i < 3000; i++)
                        {
                            OpenPrice[i] = OpenPrice[i] / symbolCounter;
                            HighPrice[i] = HighPrice[i] / symbolCounter;
                            LowPrice[i] = LowPrice[i] / symbolCounter;
                            ClosePrice[i] = ClosePrice[i] / symbolCounter;
                            Volume[i] = Volume[i] / symbolCounter;

                            if (ClosePrice[i] > 0 && Volume[i] > 0 && !string.IsNullOrEmpty(QuoteDate[i]))
                            {
                                Models.SectorPrice p = new Models.SectorPrice();
                                p.QuoteDate = QuoteDate[i];
                                p.OpenPrice = decimal.Round(OpenPrice[i],2);
                                p.HighPrice = decimal.Round(HighPrice[i],2);
                                p.LowPrice = decimal.Round(LowPrice[i],2);
                                p.ClosePrice = decimal.Round(ClosePrice[i],2);
                                p.Volume = decimal.Round(Volume[i],2);

                                prices.Add(p);
                            }
                        }
                    }
                    else
                    {
                        prices = new List<Models.SectorPrice>();
                    }
                }
                else
                {
                    prices = new List<Models.SectorPrice>();
                }
            }
            catch
            {
                prices = new List<Models.SectorPrice>();
            }

            var sectorPriceWrapper = new Models.SectorPriceWrapper();

            sectorPriceWrapper.Sector = Sector.ToUpper();
            sectorPriceWrapper.SectorDescription = sectorDescription;
            sectorPriceWrapper.SectorPrices = prices.ToList();

            return sectorPriceWrapper;
        }

        [Authorize]
        [HttpGet]
        [Route("api/SectorSymbols/{Id}")]
        public List<Models.SectorSymbol> GetSectorSymbols(String Id)
        {
            Id = Id.Replace(",", "");
            int id = Convert.ToInt32(Id);

            var retryCounter = 0;
            List<Models.SectorSymbol> values;

            while (true)
            {
                try
                {
                    var SectorSymbols = from d in db.TrnSectorSymbols
                                        where d.SectorId == id
                                        orderby d.Symbol
                                        select new Models.SectorSymbol
                                        {
                                            Id = d.Id,
                                            SectorId = d.SectorId,
                                            Symbol = d.Symbol,
                                            SymbolId = d.SymbolId,
                                            Description = d.MstSymbol.Description
                                        };
                    if (SectorSymbols.Count() > 0)
                    {
                        values = SectorSymbols.ToList();
                    }
                    else
                    {
                        values = new List<Models.SectorSymbol>();
                    }
                    break;
                }
                catch
                {
                    if (retryCounter == 3)
                    {
                        values = new List<Models.SectorSymbol>();
                        break;
                    }

                    System.Threading.Thread.Sleep(1000);
                    retryCounter++;
                }
            }
            return values;
        }

        [Authorize]
        [HttpGet]
        [Route("api/PickSectorSymbols/{Sector}")]
        public List<Models.SectorSymbol> GetPickSectorSymbols(String Sector)
        {
            var retryCounter = 0;
            List<Models.SectorSymbol> values;

            while (true)
            {
                try
                {
                    var SectorSymbols = from d in db.TrnSectorSymbols
                                        where d.TrnSector.Sector == Sector
                                        orderby d.Symbol
                                        select new Models.SectorSymbol
                                        {
                                            Id = d.Id,
                                            SectorId = d.SectorId,
                                            Symbol = d.Symbol,
                                            SymbolId = d.SymbolId,
                                            Description = d.MstSymbol.Description
                                        };
                    if (SectorSymbols.Count() > 0)
                    {
                        values = SectorSymbols.ToList();
                    }
                    else
                    {
                        values = new List<Models.SectorSymbol>();
                    }
                    break;
                }
                catch
                {
                    if (retryCounter == 3)
                    {
                        values = new List<Models.SectorSymbol>();
                        break;
                    }

                    System.Threading.Thread.Sleep(1000);
                    retryCounter++;
                }
            }
            return values;
        }

        [Authorize]
        [HttpPost]
        [Route("api/AddSector")]
        public int Post(Models.Sector Value)
        {
            try
            {

                Data.TrnSector NewSector = new Data.TrnSector();

                NewSector.Sector = Value.SectorCode;
                NewSector.Definition = Value.SectorDefinition;

                db.TrnSectors.InsertOnSubmit(NewSector);
                db.SubmitChanges();

                return NewSector.Id;
            }
            catch
            {
                return 0;
            }
        }

        [Authorize]
        [HttpPut]
        [Route("api/UpdateSector/{Id}")]
        public int Put(String Id, Models.Sector Value)
        {
            Id = Id.Replace(",", "");
            int id = Convert.ToInt32(Id);

            try
            {
                var Sectors = from d in db.TrnSectors where d.Id == id select d;

                if (Sectors.Any())
                {
                    var UpdatedSector = Sectors.FirstOrDefault();

                    UpdatedSector.Sector = Value.SectorCode;
                    UpdatedSector.Definition = Value.SectorDefinition;

                    db.SubmitChanges();

                    return UpdatedSector.Id;
                }
                else
                {
                    return 0;
                }
            }
            catch (NullReferenceException)
            {
                return 0;
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("api/DeleteSector/{Id}")]
        public HttpResponseMessage Delete(int Id)
        {
            Data.TrnSector DeleteSector = db.TrnSectors.Where(d => d.Id == Id).First();

            if (DeleteSector != null)
            {
                db.TrnSectors.DeleteOnSubmit(DeleteSector);
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

        // Sector Symbol

        [Authorize]
        [HttpGet]
        [Route("api/SectorSymbol/{Id}")]
        public Models.SectorSymbol GetSectorSymbol(String Id)
        {
            Id = Id.Replace(",", "");
            int id = Convert.ToInt32(Id);

            var retryCounter = 0;
            Models.SectorSymbol value;

            while (true)
            {
                try
                {
                    var SectorSymbol = from d in db.TrnSectorSymbols
                                       where d.Id == id
                                       select new Models.SectorSymbol
                                       {
                                            Id = d.Id,
                                            SectorId = d.SectorId,
                                            Symbol = d.Symbol,
                                            SymbolId = d.SymbolId,
                                            Description = d.MstSymbol.Description
                                       };
                    if (SectorSymbol.Count() > 0)
                    {
                        value = SectorSymbol.First();
                    }
                    else
                    {
                        value = new Models.SectorSymbol();
                    }
                    break;
                }
                catch
                {
                    if (retryCounter == 3)
                    {
                        value = new Models.SectorSymbol();
                        break;
                    }

                    System.Threading.Thread.Sleep(1000);
                    retryCounter++;
                }
            }
            return value;
        }

        [Authorize]
        [HttpPost]
        [Route("api/AddSectorSymbol")]
        public int PostSymbol(Models.SectorSymbol Value)
        {
            try
            {
                var Symbol = from d in db.MstSymbols where d.Symbol == Value.Symbol select d;

                if (Symbol.Any())
                {
                    Data.TrnSectorSymbol NewSectorSymbol = new Data.TrnSectorSymbol();

                    NewSectorSymbol.SectorId = Value.SectorId;
                    NewSectorSymbol.SymbolId = Symbol.First().Id;
                    NewSectorSymbol.Symbol = Symbol.First().Symbol;

                    db.TrnSectorSymbols.InsertOnSubmit(NewSectorSymbol);
                    db.SubmitChanges();

                    return NewSectorSymbol.Id;
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }    
    
        [Authorize]
        [HttpPut]
        [Route("api/UpdateSectorSymbol/{Id}")]
        public int PutSymbol(String Id, Models.SectorSymbol Value)
        {
            Id = Id.Replace(",", "");
            int id = Convert.ToInt32(Id);

            try
            {
                var SectorSymbols = from d in db.TrnSectorSymbols where d.Id == id select d;

                if (SectorSymbols.Any())
                {
                    var Symbol = from d in db.MstSymbols where d.Symbol == Value.Symbol select d;

                    if (Symbol.Any())
                    {
                        var UpdatedSectorSymbol = SectorSymbols.FirstOrDefault();

                        UpdatedSectorSymbol.SymbolId = Symbol.First().Id;
                        UpdatedSectorSymbol.Symbol = Symbol.First().Symbol;

                        db.SubmitChanges();

                        return UpdatedSectorSymbol.Id;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch (NullReferenceException)
            {
                return 0;
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("api/DeleteSectorSymbol/{Id}")]
        public HttpResponseMessage DeleteSymbol(int Id)
        {
            Data.TrnSectorSymbol DeleteSectorSymbol = db.TrnSectorSymbols.Where(d => d.Id == Id).First();

            if (DeleteSectorSymbol != null)
            {
                db.TrnSectorSymbols.DeleteOnSubmit(DeleteSectorSymbol);
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
