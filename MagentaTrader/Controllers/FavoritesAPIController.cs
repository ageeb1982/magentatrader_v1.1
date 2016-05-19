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
                           orderby d.Id descending
                           select new Models.Favorite
                           {
                               Id = d.Id,
                               SymbolId = d.SymbolId,
                               Symbol = d.Symbol,
                               UserId = d.UserId,
                               Remarks = d.Remarks,
                               EncodedDate = d.EncodedDate.HasValue ? Convert.ToString(d.EncodedDate.Value.Year) + "-" + Convert.ToString(d.EncodedDate.Value.Month + 100).Substring(1, 2) + "-" + Convert.ToString(d.EncodedDate.Value.Day + 100).Substring(1, 2) : "NA",
                               Trend = d.Trend == null ? "SIDEWAYS" : d.Trend
                           };

                favorites = data.ToList();
            }
            catch
            {
                favorites = new List<Models.Favorite>();
            }

            return favorites.ToList();
        }

        // GET api/UserFavorites/Web99
        [Authorize]
        [Route("api/UserFavorites/{FavoriteName}")]
        public List<Models.Favorite> GetUserFavorites(String FavoriteName)
        {
            List<Models.Favorite> favorites = new List<Models.Favorite>();
            string currentUserName = User.Identity.Name;
            long userId = 0;
            try
            {
                userId = (from d in db.MstUsers where d.UserName.Equals(currentUserName) select d).FirstOrDefault().Id;
                //userId = 82;

                var data = from d in db.TrnFavorites
                           where d.UserId == userId && d.Remarks.Equals(FavoriteName)
                           orderby d.Id descending
                           select new Models.Favorite
                           {
                               Id = d.Id,
                               SymbolId = d.SymbolId,
                               Symbol = d.Symbol,
                               SymbolDescription = d.MstSymbol.Description,
                               Exchange = d.MstSymbol.Exchange,
                               UserId = d.UserId,
                               Remarks = d.Remarks,
                               IsShared = d.IsShared,
                               EncodedDate = d.EncodedDate.HasValue ? Convert.ToString(d.EncodedDate.Value.Year) + "-" + Convert.ToString(d.EncodedDate.Value.Month + 100).Substring(1, 2) + "-" + Convert.ToString(d.EncodedDate.Value.Day + 100).Substring(1, 2) : "NA",
                               Trend = d.Trend == null ? "SIDEWAYS" : d.Trend
                           };

                favorites = data.ToList();
            }
            catch
            {
                favorites = new List<Models.Favorite>();
            }

            return favorites.ToList();
        }

        // GET api/UserFavorites/Web99/dpilger
        [Authorize]
        [Route("api/UserFavoritesByUser/{FavoriteName}/{UserName}")]
        public List<Models.Favorite> GetUserFavoritesByUser(String FavoriteName, String UserName)
        {
            List<Models.Favorite> favorites = new List<Models.Favorite>();
            long userId = 0;
            try
            {
                userId = (from d in db.MstUsers where d.UserName.Equals(UserName) select d).FirstOrDefault().Id;

                var data = from d in db.TrnFavorites
                           where d.UserId == userId && d.Remarks.Equals(FavoriteName)
                           orderby d.Id descending
                           select new Models.Favorite
                           {
                               Id = d.Id,
                               SymbolId = d.SymbolId,
                               Symbol = d.Symbol,
                               SymbolDescription = d.MstSymbol.Description,
                               Exchange = d.MstSymbol.Exchange,
                               UserId = d.UserId,
                               Remarks = d.Remarks,
                               IsShared = d.IsShared,
                               EncodedDate = d.EncodedDate.HasValue ? Convert.ToString(d.EncodedDate.Value.Year) + "-" + Convert.ToString(d.EncodedDate.Value.Month + 100).Substring(1, 2) + "-" + Convert.ToString(d.EncodedDate.Value.Day + 100).Substring(1, 2) : "NA",
                               Trend = d.Trend == null ? "SIDEWAYS" : d.Trend
                           };

                favorites = data.ToList();
            }
            catch
            {
                favorites = new List<Models.Favorite>();
            }

            return favorites.ToList();
        }

        // GET api/DeleteUserFavorites/Web99
        [Authorize]
        [Route("api/DeleteUserFavorites/{FavoriteName}")]
        public HttpResponseMessage GetDeleteUserFavorites(String FavoriteName)
        {
            List<Models.Favorite> favorites = new List<Models.Favorite>();
            string currentUserName = User.Identity.Name;
            long userId = 0;
            try
            {
                userId = (from d in db.MstUsers where d.UserName.Equals(currentUserName) select d).FirstOrDefault().Id;

                var data = db.TrnFavorites.Where(d=>d.UserId == userId && d.Remarks.Equals(FavoriteName));
                foreach (Data.TrnFavorite d in data)
                {
                    db.TrnFavorites.DeleteOnSubmit(d);
                    db.SubmitChanges();
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // GET api/SharedFavorites
        [Authorize]
        [Route("api/SharedFavorites")]
        public List<Models.FavoriteName> GetSharedFavorites()
        {
            List<Models.FavoriteName> sharedFavorites = new List<Models.FavoriteName>();
            try
            {
                var data = from d in db.TrnFavorites
                           where d.IsShared == true
                           orderby d.Remarks
                           group d by new
                           {
                               d.UserId,
                               d.MstUser.UserName,
                               d.Remarks
                           } into g
                           select new Models.FavoriteName
                           {
                               Remarks = g.Key.Remarks,
                               User = g.Key.UserName,
                               UserId = g.Key.UserId,
                               NoOfSymbols = g.Count()
                           };

                sharedFavorites = data.ToList();
            }
            catch
            {
                sharedFavorites = new List<Models.FavoriteName>();
            }

            return sharedFavorites.ToList();
        }

        // GET api/UserFavoriteNames
        [Authorize]
        [Route("api/UserFavoriteNames")]
        public List<Models.FavoriteName> GetUserFavoriteNames()
        {
            List<Models.FavoriteName> favoriteNames = new List<Models.FavoriteName>();
            string currentUserName = User.Identity.Name;
            long userId = 0;
            try
            {
                userId = (from d in db.MstUsers where d.UserName.Equals(currentUserName) select d).FirstOrDefault().Id;
                //userId = 82;

                var data = from d in db.TrnFavorites
                           where d.UserId == userId
                           orderby d.Remarks
                           group d by new
                           {
                               d.UserId,
                               d.Remarks
                           } into g
                           select new Models.FavoriteName
                           {
                               Remarks = g.Key.Remarks
                           };

                favoriteNames = data.ToList();
            }
            catch
            {
                favoriteNames = new List<Models.FavoriteName>();
            }

            return favoriteNames.ToList();
        }

        // GET api/ChangeFavoriteName
        [Authorize]
        [Route("api/ChangeFavoriteName")]
        public HttpResponseMessage GetChangeFavoriteName([FromUri] List<String> FavoriteNames)
        {
            string currentUserName = User.Identity.Name;
            int userId = 0;
            try
            {
                userId = (from d in db.MstUsers where d.UserName.Equals(currentUserName) select d).FirstOrDefault().Id;
                var oldRemarks = FavoriteNames[0];
                var newRemarks = FavoriteNames[1];

                var data = from d in db.TrnFavorites
                           where d.UserId == userId && d.Remarks.Equals(oldRemarks)
                           select d;

                foreach (var d in data) {
                    d.Remarks = newRemarks;
                    db.SubmitChanges();
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // GET api/ChangeFavoriteShareStatus
        [Authorize]
        [Route("api/ChangeFavoriteShareStatus")]
        public HttpResponseMessage GetChangeFavoriteShareStatus([FromUri] List<String> Status)
        {
            string currentUserName = User.Identity.Name;
            int userId = 0;
            try
            {
                userId = (from d in db.MstUsers where d.UserName.Equals(currentUserName) select d).FirstOrDefault().Id;
                
                var newIsShared = Status[0];
                var favoriteName = Status[1];

                var data = from d in db.TrnFavorites
                           where d.UserId == userId && d.Remarks.Equals(favoriteName)
                           select d;

                foreach (var d in data)
                {
                    d.IsShared = newIsShared.Equals("true") ? true : false;
                    db.SubmitChanges();
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
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

                DateTime now = DateTime.Now;
                string uploadDate = now.ToString("yyyy-MM-dd");

                bool isShared = false;
                var data = from d in db.TrnFavorites
                           where d.UserId == userId && d.Remarks.Equals("UPLOADED-" + uploadDate)
                           select d;
                if (data.Any())
                {
                    isShared = data.First().IsShared;
                }

                for (int i = 0; i < Symbols.Count; i++)
                {
                    Data.TrnFavorite NewFavorite = new Data.TrnFavorite();

                    var symbolId = from d in db.MstSymbols where d.Symbol.Equals(Symbols[i].Trim()) select d;
                    if (symbolId.Any())
                    {
                        DateTime d = DateTime.Now;
                        SqlDateTime EncodedDate = new SqlDateTime(new DateTime(d.Year, d.Month, d.Day));

                        NewFavorite.SymbolId = symbolId.FirstOrDefault().Id;
                        NewFavorite.Symbol = Symbols[i].ToUpper();
                        NewFavorite.Remarks = "UPLOADED-" + uploadDate;
                        NewFavorite.EncodedDate = EncodedDate.Value;
                        NewFavorite.Trend = "SIDEWAYS";
                        NewFavorite.UserId = userId;
                        NewFavorite.IsShared = isShared;

                        db.TrnFavorites.InsertOnSubmit(NewFavorite);
                        db.SubmitChanges();

                        returnId = NewFavorite.Id;
                    }

                    if (currentUserName.ToUpper()!="DPILGER") {
                        if (i > 450 - countFavorites) break;
                    }
                    
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
                
                bool isShared = false;
                var data = from d in db.TrnFavorites
                           where d.UserId == userId && d.Remarks.Equals(value.Remarks)
                           select d;
                if (data.Any())
                {
                    isShared = data.First().IsShared;
                }

                DateTime dt = Convert.ToDateTime(value.EncodedDate);
                SqlDateTime EncodedDate = new SqlDateTime(new DateTime(dt.Year, dt.Month, dt.Day));

                NewFavorite.SymbolId = symbolId;
                NewFavorite.Symbol = value.Symbol;
                NewFavorite.Remarks = value.Remarks;
                NewFavorite.EncodedDate = EncodedDate.Value;
                NewFavorite.Trend = value.Trend;
                NewFavorite.UserId = userId;

                NewFavorite.IsShared = isShared;

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

                    DateTime dt = Convert.ToDateTime(value.EncodedDate);
                    SqlDateTime EncodedDate = new SqlDateTime(new DateTime(dt.Year, dt.Month, dt.Day));

                    UpdatedFavorites.SymbolId = symbolId;
                    UpdatedFavorites.Symbol = value.Symbol;
                    UpdatedFavorites.Remarks = value.Remarks;
                    UpdatedFavorites.EncodedDate = EncodedDate.Value;
                    UpdatedFavorites.Trend = value.Trend;
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