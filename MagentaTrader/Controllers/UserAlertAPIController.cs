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

        private void GetResultSymbols() {

        }
    }
}
