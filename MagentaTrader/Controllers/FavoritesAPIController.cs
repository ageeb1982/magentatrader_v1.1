using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MagentaTrader.Controllers
{
    public class FavoritesAPIController : ApiController
    {
        private Data.MagentaTradersDBDataContext db = new Data.MagentaTradersDBDataContext();
        
        // GET api/Favorite
        [Authorize]
        [Route("api/Favorite")]
        public List<Models.Favorite> Get()
        {
            List<Models.Favorite> favorites = new List<Models.Favorite>();

            try
            {
                var data = from d in db.TrnFavorites
                           select new Models.Favorite
                           {
                               Id = d.Id,
                               SymbolId = d.SymbolId,
                               Symbol = d.Symbol,
                               UserId = d.UserId,
                               Remarks = d.Remarks
                           };

                favorites = data.ToList();
            }
            catch
            {
                favorites = new List<Models.Favorite>();
            }

            return favorites.ToList();
        }

        // GET api/UserFavorites
        [Authorize]
        [Route("api/UserFavorites")]
        public List<Models.Favorite> GetUserFavorites()
        {
            List<Models.Favorite> favorites = new List<Models.Favorite>();
            string currentUserName = User.Identity.Name;
            long userId = 0;
            try
            {
                userId = (from d in db.MstUsers where d.UserName.Equals(currentUserName) select d).FirstOrDefault().Id;

                var data = from d in db.TrnFavorites
                           where d.UserId == userId
                           select new Models.Favorite
                           {
                               Id = d.Id,
                               SymbolId = d.SymbolId,
                               Symbol = d.Symbol,
                               SymbolDescription = d.MstSymbol.Description,
                               Exchange = d.MstSymbol.Exchange,
                               UserId = d.UserId,
                               Remarks = d.Remarks
                           };

                favorites = data.ToList();
            }
            catch
            {
                favorites = new List<Models.Favorite>();
            }

            return favorites.ToList();
        }

        // POST api/UploadFavorites
        [Authorize]
        [Route("api/UploadFavorites")]
        public long GetUploadFavorites([FromUri] List<String> Symbols)
        {
            string currentUserName = User.Identity.Name;
            int userId = 0;
            long returnId = 0;
            try
            {
                userId = (from d in db.MstUsers where d.UserName.Equals(currentUserName) select d).FirstOrDefault().Id;
                var countFavorites = (from d in db.TrnFavorites where d.UserId == userId select d).Count();
                
                for (int i = 0; i < Symbols.Count; i++)
                {
                    Data.TrnFavorite NewFavorite = new Data.TrnFavorite();
                    var symbolId = (from d in db.MstSymbols where d.Symbol.Equals(Symbols[i]) select d).FirstOrDefault().Id;
                    DateTime now = DateTime.Now;
                    string uploadDate = now.ToString("yyyy-MM-dd");

                    if (symbolId > 0)
                    {
                        NewFavorite.SymbolId = symbolId;
                        NewFavorite.Symbol = Symbols[i].ToUpper();
                        NewFavorite.Remarks = "UPLOADED-" + uploadDate;
                        NewFavorite.UserId = userId;

                        db.TrnFavorites.InsertOnSubmit(NewFavorite);
                        db.SubmitChanges();

                        returnId = NewFavorite.Id;
                    }

                    if (i > 400 - countFavorites) break;
                }
            }
            catch
            {

            }
            return returnId;
        }

        // POST api/AddFavorite
        [Authorize]
        [Route("api/AddFavorite")]
        public int Post(Models.Favorite value)
        {
            try
            {
                Data.TrnFavorite NewFavorite = new Data.TrnFavorite();

                var userId = (from d in db.MstUsers where d.UserName.Equals(value.User) select d).FirstOrDefault().Id;
                var symbolId = (from d in db.MstSymbols where d.Symbol.Equals(value.Symbol) select d).FirstOrDefault().Id;

                NewFavorite.SymbolId = symbolId;
                NewFavorite.Symbol = value.Symbol;
                NewFavorite.Remarks = value.Remarks;
                NewFavorite.UserId = userId;

                db.TrnFavorites.InsertOnSubmit(NewFavorite);
                db.SubmitChanges();

                return NewFavorite.Id;
            }
            catch
            {
                return 0;
            }
        }

        // PUT /api/UpdateFavorite/5
        [Authorize]
        [Route("api/UpdateFavorite/{Id}")]
        public HttpResponseMessage Put(String Id, Models.Favorite value)
        {
            Id = Id.Replace(",", "");
            int id = Convert.ToInt32(Id);

            try
            {
                var Favorites = from d in db.TrnFavorites where d.Id == id select d;

                if (Favorites.Any())
                {
                    var UpdatedFavorites = Favorites.FirstOrDefault();
                    var userId = (from d in db.MstUsers where d.UserName.Equals(value.User) select d).FirstOrDefault().Id;
                    var symbolId = (from d in db.MstSymbols where d.Symbol.Equals(value.Symbol) select d).FirstOrDefault().Id;

                    UpdatedFavorites.SymbolId = symbolId;
                    UpdatedFavorites.Symbol = value.Symbol;
                    UpdatedFavorites.Remarks = value.Remarks;
                    UpdatedFavorites.UserId = userId;

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

        // DELETE api/DeleteFavorite/5
        [Authorize]
        [Route("api/DeleteFavorite/{Id}")]
        public HttpResponseMessage Delete(int Id)
        {
            Data.TrnFavorite DeleteFavorite = db.TrnFavorites.Where(d => d.Id == Id).First();

            if (DeleteFavorite != null)
            {
                db.TrnFavorites.DeleteOnSubmit(DeleteFavorite);
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