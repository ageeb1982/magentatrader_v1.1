using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MagentaTrader.Controllers
{
    public class UserFavoriteAPIController : ApiController
    {
        private Data.MagentaTradersDBDataContext db = new Data.MagentaTradersDBDataContext();

        // GET api/UserFavoriteList/dpilger
        [Authorize]
        [Route("api/UserFavoriteList/{UserName}")]
        public List<Models.UserFavorite> GetUserFavoriteList(String UserName)
        {
            List<Models.UserFavorite> userFavorites = new List<Models.UserFavorite>();

            try
            {
                var data = from d in db.TrnUserFavorites
                           where d.MstUser.UserName == UserName
                           orderby d.Id descending
                           select new Models.UserFavorite
                           {
                               Id = d.Id,
                               UserId = d.UserId,
                               User = d.MstUser.UserName,
                               Description = d.Description,
                               IsShared = d.IsShared,
                               EncodedDate = d.EncodedDate.HasValue ? Convert.ToString(d.EncodedDate.Value.Year) + "-" + Convert.ToString(d.EncodedDate.Value.Month + 100).Substring(1, 2) + "-" + Convert.ToString(d.EncodedDate.Value.Day + 100).Substring(1, 2) : "NA",
                               NoOfSymbols = d.TrnUserFavoritesSymbols.Count()
                           };

                userFavorites = data.ToList();
            }
            catch
            {
                userFavorites = new List<Models.UserFavorite>();
            }

            return userFavorites.ToList();
        }

        // GET api/UserFavoriteSharedList
        [Authorize]
        [Route("api/UserFavoriteSharedList")]
        public List<Models.UserFavorite> GetUserFavoriteSharedList()
        {
            List<Models.UserFavorite> userFavorites = new List<Models.UserFavorite>();

            try
            {
                var data = from d in db.TrnUserFavorites
                           where d.IsShared == true
                           orderby d.Description ascending
                           select new Models.UserFavorite
                           {
                               Id = d.Id,
                               UserId = d.UserId,
                               User = d.MstUser.UserName,
                               Description = d.Description,
                               IsShared = d.IsShared,
                               EncodedDate = d.EncodedDate.HasValue ? Convert.ToString(d.EncodedDate.Value.Year) + "-" + Convert.ToString(d.EncodedDate.Value.Month + 100).Substring(1, 2) + "-" + Convert.ToString(d.EncodedDate.Value.Day + 100).Substring(1, 2) : "NA",
                               NoOfSymbols = d.TrnUserFavoritesSymbols.Count()
                           };

                userFavorites = data.ToList();
            }
            catch
            {
                userFavorites = new List<Models.UserFavorite>();
            }

            return userFavorites.ToList();
        }

        // GET api/UserFavorite/1
        [Authorize]
        [Route("api/UserFavorite/{Id}")]
        public Models.UserFavorite GetUserFavorite(String Id)
        {
            Models.UserFavorite userFavorites = new Models.UserFavorite();

            try
            {
                var data = from d in db.TrnUserFavorites
                           where d.Id == Convert.ToInt32(Id)
                           select new Models.UserFavorite
                           {
                               Id = d.Id,
                               UserId = d.UserId,
                               User = d.MstUser.UserName,
                               Description = d.Description,
                               IsShared = d.IsShared,
                               EncodedDate = d.EncodedDate.HasValue ? Convert.ToString(d.EncodedDate.Value.Year) + "-" + Convert.ToString(d.EncodedDate.Value.Month + 100).Substring(1, 2) + "-" + Convert.ToString(d.EncodedDate.Value.Day + 100).Substring(1, 2) : "NA"
                           };

                userFavorites = data.FirstOrDefault();
            }
            catch
            {
                userFavorites = new Models.UserFavorite();
            }

            return userFavorites;
        }

        // POST api/UserFavoriteAdd
        [Authorize]
        [Route("api/UserFavoriteAdd")]
        public int Post(Models.UserFavorite value)
        {
            try
            {
                Data.TrnUserFavorite newUserFavorite = new Data.TrnUserFavorite();

                var userId = (from d in db.MstUsers where d.UserName.Equals(value.User) select d).FirstOrDefault().Id;
                bool isShared = false;

                DateTime dt = Convert.ToDateTime(value.EncodedDate);
                SqlDateTime EncodedDate = new SqlDateTime(new DateTime(dt.Year, dt.Month, dt.Day));

                newUserFavorite.UserId = userId;
                newUserFavorite.Description = value.Description;
                newUserFavorite.IsShared = isShared;
                newUserFavorite.EncodedDate = EncodedDate.Value;

                db.TrnUserFavorites.InsertOnSubmit(newUserFavorite);
                db.SubmitChanges();

                return newUserFavorite.Id;
            }
            catch
            {
                return 0;
            }
        }

        // POST api/UserFavoriteUpdate/1
        [Authorize]
        [Route("api/UserFavoriteUpdate/{Id}")]
        public HttpResponseMessage Put(String Id, Models.UserFavorite value)
        {
            Id = Id.Replace(",", "");
            int id = Convert.ToInt32(Id);

            try
            {
                var userFavorites = from d in db.TrnUserFavorites where d.Id == id select d;

                if (userFavorites.Any())
                {
                    var updatedUserFavorites = userFavorites.FirstOrDefault();

                    var userId = (from d in db.MstUsers where d.UserName.Equals(value.User) select d).FirstOrDefault().Id;

                    DateTime dt = Convert.ToDateTime(value.EncodedDate);
                    SqlDateTime EncodedDate = new SqlDateTime(new DateTime(dt.Year, dt.Month, dt.Day));

                    updatedUserFavorites.UserId = userId;
                    updatedUserFavorites.Description = value.Description;
                    updatedUserFavorites.IsShared = value.IsShared;
                    updatedUserFavorites.EncodedDate = EncodedDate.Value;

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

        // DELETE api/UserFavoriteDelete/1
        [Authorize]
        [Route("api/UserFavoriteDelete/{Id}")]
        public HttpResponseMessage Delete(int Id)
        {
            Data.TrnUserFavorite deleteUserFavorite = db.TrnUserFavorites.Where(d => d.Id == Id).First();

            if (deleteUserFavorite != null)
            {
                var userFavoritesSymbols = from d in db.TrnUserFavoritesSymbols where d.UserFavoritesId == deleteUserFavorite.Id select d;
                if (userFavoritesSymbols.Any())
                {
                    foreach(Data.TrnUserFavoritesSymbol s in userFavoritesSymbols) {
                        db.TrnUserFavoritesSymbols.DeleteOnSubmit(s);
                    }
                    try
                    {
                        db.SubmitChanges();
                    }
                    catch
                    {
                        // Do nothing.
                    }
                }

                db.TrnUserFavorites.DeleteOnSubmit(deleteUserFavorite);
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
