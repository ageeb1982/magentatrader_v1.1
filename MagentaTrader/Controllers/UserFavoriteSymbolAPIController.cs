using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MagentaTrader.Controllers
{
    public class UserFavoriteSymbolAPIController : ApiController
    {
        private Data.MagentaTradersDBDataContext db = new Data.MagentaTradersDBDataContext();

        // GET api/UserFavoriteSymbolList/1
        [Authorize]
        [Route("api/UserFavoriteSymbolList/{Id}")]
        public List<Models.UserFavoriteSymbol> GetUserFavoriteSymbolList(String Id)
        {
            List<Models.UserFavoriteSymbol> userFavoriteSymbols = new List<Models.UserFavoriteSymbol>();

            try
            {
                var data = from d in db.TrnUserFavoritesSymbols
                           where d.UserFavoritesId == Convert.ToInt32(Id)
                           orderby d.Symbol ascending
                           select new Models.UserFavoriteSymbol
                           {
                               Id = d.Id,
                               UserFavoritesId = d.UserFavoritesId,
                               SymbolId = d.SymbolId,
                               Symbol = d.MstSymbol.Symbol,
                               SymbolDescription = d.MstSymbol.Description,
                               Trend = d.Trend,
                               EncodedDate = d.EncodedDate.HasValue ? Convert.ToString(d.EncodedDate.Value.Year) + "-" + Convert.ToString(d.EncodedDate.Value.Month + 100).Substring(1, 2) + "-" + Convert.ToString(d.EncodedDate.Value.Day + 100).Substring(1, 2) : "NA",
                           };

                userFavoriteSymbols = data.ToList();
            }
            catch
            {
                userFavoriteSymbols = new List<Models.UserFavoriteSymbol>();
            }

            return userFavoriteSymbols.ToList();
        }

        // GET api/UserFavoriteSymbol/1
        [Authorize]
        [Route("api/UserFavoriteSymbol/{Id}")]
        public Models.UserFavoriteSymbol GetUserFavoriteSymbol(String Id)
        {
            Models.UserFavoriteSymbol userFavoriteSymbols = new Models.UserFavoriteSymbol();

            try
            {
                var data = from d in db.TrnUserFavoritesSymbols
                           where d.Id == Convert.ToInt32(Id)
                           select new Models.UserFavoriteSymbol
                           {
                               Id = d.Id,
                               UserFavoritesId = d.UserFavoritesId,
                               SymbolId = d.SymbolId,
                               Symbol = d.MstSymbol.Symbol,
                               SymbolDescription = d.MstSymbol.Description,
                               Trend = d.Trend == null ? "SIDEWAYS" : d.Trend,
                               EncodedDate = d.EncodedDate.HasValue ? Convert.ToString(d.EncodedDate.Value.Year) + "-" + Convert.ToString(d.EncodedDate.Value.Month + 100).Substring(1, 2) + "-" + Convert.ToString(d.EncodedDate.Value.Day + 100).Substring(1, 2) : "NA",
                           };

                userFavoriteSymbols = data.FirstOrDefault();
            }
            catch
            {
                userFavoriteSymbols = new Models.UserFavoriteSymbol();
            }

            return userFavoriteSymbols;
        }

        // GET api/UserFavoriteSymbolUpload/1
        [Authorize]
        [Route("api/UserFavoriteSymbolUpload/{Id}")]
        public HttpResponseMessage GetUserFavoriteSymbolUpload(String Id, [FromUri] List<String> Symbols)
        {
            Id = Id.Replace(",", "");
            int userFavoritesId = Convert.ToInt32(Id);

            try
            {
                if (userFavoritesId > 0)
                {
                    for (int i = 0; i < Symbols.Count; i++)
                    {
                        Data.TrnUserFavoritesSymbol newUserFavoriteSymbol = new Data.TrnUserFavoritesSymbol();

                        var symbols = from d in db.MstSymbols where d.Symbol == Symbols[i].ToUpper() select d;

                        if (symbols.Any())
                        {
                            newUserFavoriteSymbol.UserFavoritesId = userFavoritesId;
                            newUserFavoriteSymbol.SymbolId = symbols.First().Id;
                            newUserFavoriteSymbol.Symbol = symbols.First().Symbol;
                            newUserFavoriteSymbol.Trend = "SIDEWAYS";

                            //DateTime dt = new DateTime();
                            //SqlDateTime EncodedDate = new SqlDateTime(new DateTime(dt.Year, dt.Month, dt.Day));
                            //newUserFavoriteSymbol.EncodedDate = EncodedDate.Value;

                            db.TrnUserFavoritesSymbols.InsertOnSubmit(newUserFavoriteSymbol);
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
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // POST api/UserFavoriteSymbolAdd
        [Authorize]
        [Route("api/UserFavoriteSymbolAdd")]
        public long Post(Models.UserFavoriteSymbol value)
        {
            try
            {
                Data.TrnUserFavoritesSymbol newUserFavoriteSymbol = new Data.TrnUserFavoritesSymbol();

                var symbols = from d in db.MstSymbols where d.Symbol == value.Symbol.ToUpper() select d;

                if (symbols.Any()) {
                    newUserFavoriteSymbol.UserFavoritesId = value.UserFavoritesId;
                    newUserFavoriteSymbol.SymbolId = symbols.First().Id;
                    newUserFavoriteSymbol.Symbol = symbols.First().Symbol;
                    newUserFavoriteSymbol.Trend = value.Trend == null ? "SIDEWAYS" : value.Trend;
                    newUserFavoriteSymbol.EncodedDate = Convert.ToDateTime(value.EncodedDate);

                    db.TrnUserFavoritesSymbols.InsertOnSubmit(newUserFavoriteSymbol);
                    db.SubmitChanges();

                    return newUserFavoriteSymbol.Id;
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

        // POST api/UserFavoriteSymbolUpdate/1
        [Authorize]
        [Route("api/UserFavoriteSymbolUpdate/{Id}")]
        public HttpResponseMessage Put(String Id, Models.UserFavoriteSymbol value)
        {
            Id = Id.Replace(",", "");
            int id = Convert.ToInt32(Id);

            try
            {
                var userFavoritesSymbols = from d in db.TrnUserFavoritesSymbols where d.Id == id select d;

                if (userFavoritesSymbols.Any())
                {
                    var updatedUserFavoritesSymbols = userFavoritesSymbols.FirstOrDefault();

                    var symbols = from d in db.MstSymbols where d.Symbol == value.Symbol.ToUpper() select d;

                    if (symbols.Any())
                    {
                        updatedUserFavoritesSymbols.UserFavoritesId = value.UserFavoritesId;
                        updatedUserFavoritesSymbols.SymbolId = symbols.First().Id;
                        updatedUserFavoritesSymbols.Symbol = symbols.First().Symbol;
                        updatedUserFavoritesSymbols.Trend = value.Trend == null ? "SIDEWAYS" : value.Trend;
                        updatedUserFavoritesSymbols.EncodedDate = Convert.ToDateTime(value.EncodedDate);

                        db.SubmitChanges();

                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }
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

        // DELETE api/UserFavoriteSymbolDelete/1
        [Authorize]
        [Route("api/UserFavoriteSymbolDelete/{Id}")]
        public HttpResponseMessage Delete(int Id)
        {
            Data.TrnUserFavoritesSymbol deleteUserFavoriteSymbol = db.TrnUserFavoritesSymbols.Where(d => d.Id == Id).First();

            if (deleteUserFavoriteSymbol != null)
            {
                db.TrnUserFavoritesSymbols.DeleteOnSubmit(deleteUserFavoriteSymbol);
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