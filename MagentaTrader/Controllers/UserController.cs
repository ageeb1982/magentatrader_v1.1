using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Security;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;

namespace MagentaTrader.Controllers
{
    public class UserController : ApiController
    {
        private Data.MagentaTradersDBDataContext db = new Data.MagentaTradersDBDataContext();

        // GET api/User
        [Authorize]
        public List<Models.User> Get()
        {
            var retryCounter = 0;
            List<Models.User> values;

            while (true)
            {
                try
                {
                    var Userss = (from u in db.MstUsers.Where(x => x.AspNetUserId != null)
                                 select new Models.User
                                 {
                                     Id = u.Id,
                                     UserName = u.UserName,
                                     FirstName = u.FirstName,
                                     LastName = u.LastName,
                                     EmailAddress = u.EmailAddress,
                                     PhoneNumber = u.PhoneNumber,
                                     Address = u.Address
                                 }).ToList();

                    var MaxLastPurchase = (from u in db.MstUsers.Where(x => x.AspNetUserId != null)
                                           from s in db.TrnSales.Where(x => x.UserId == u.Id).GroupBy(x => x.UserId)
                                           select new Models.User
                                           {
                                               Id = u.Id,
                                               AspNetUserId = u.AspNetUserId,
                                               dateLP = s.Max(x => x.SalesDate)
                                           }).ToArray();
                    
                    for( var x = 0; x < MaxLastPurchase.Count()-1; x++ )
                    {
                        if (Userss.Any(n => n.Id == MaxLastPurchase[x].Id))
                            MaxLastPurchase[x].LastPurchase = Convert.ToString(MaxLastPurchase[x].dateLP.Year) + "-" + Convert.ToString(MaxLastPurchase[x].dateLP.Month + 100).Substring(1, 2) + "-" + Convert.ToString(MaxLastPurchase[x].dateLP.Day + 100).Substring(1, 2);
                            Userss.Where(m => m.Id == MaxLastPurchase[x].Id).ToList().ForEach(c => c.LastPurchase = MaxLastPurchase[x].LastPurchase);
                    }

                    var Users = Userss.ToList();

                    if (Users.Count() > 0)
                    {
                        values = Users.ToList();
                    }
                    else
                    {
                        values = new List<Models.User>();
                    }
                    break;
                }
                catch
                {
                    if (retryCounter == 3)
                    {
                        values = new List<Models.User>();
                        break;
                    }

                    System.Threading.Thread.Sleep(1000);
                    retryCounter++;
                }
            }
            return values;
        }

        // POST api/AddUser
        [Authorize]
        [Route("api/AddUser")]
        public int Post(Models.User value)
        {
            try
            {

                Data.MstUser NewUser = new Data.MstUser();

                NewUser.UserName = value.UserName;
                NewUser.FirstName = value.FirstName;
                NewUser.LastName = value.LastName;
                NewUser.EmailAddress = value.EmailAddress;
                NewUser.PhoneNumber = value.PhoneNumber;
                NewUser.Address = value.Address;

                db.MstUsers.InsertOnSubmit(NewUser);
                db.SubmitChanges();

                return NewUser.Id;
            }
            catch
            {
                return 0;
            }
        }

        // PUT /api/UpdateUser/5
        [Authorize]
        [Route("api/UpdateUser/{Id}")]
        public HttpResponseMessage Put(String Id, Models.User value)
        {
            Id = Id.Replace(",", "");
            int id = Convert.ToInt32(Id);

            try
            {
                var Users = from d in db.MstUsers where d.Id == id select d;
                
                if (Users.Any())
                {
                    var UpdatedUser = Users.FirstOrDefault();

                    UpdatedUser.UserName = value.UserName;
                    UpdatedUser.FirstName = value.FirstName;
                    UpdatedUser.LastName = value.LastName;
                    UpdatedUser.EmailAddress = value.EmailAddress;
                    UpdatedUser.PhoneNumber = value.PhoneNumber;
                    UpdatedUser.Address = value.Address;

                    db.SubmitChanges();
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (NullReferenceException)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // DELETE api/DeleteUser/5
        [Authorize]
        [Route("api/DeleteUser/{Id}")]
        public HttpResponseMessage Delete(int Id)
        {
            Data.MstUser DeleteUser = db.MstUsers.Where(d => d.Id == Id).First();

            if (DeleteUser != null)
            {
                db.MstUsers.DeleteOnSubmit(DeleteUser);
                try
                {
                    db.SubmitChanges(); // Delete MSTUSER

                    var aspNetUser = from d in db.AspNetUsers where d.UserName == DeleteUser.UserName select d;

                    if (aspNetUser.Any()) {
                        var aspNetUserRoles = from d in db.AspNetUserRoles where d.UserId == aspNetUser.First().Id select d;
                        foreach (Data.AspNetUserRole role in aspNetUserRoles)
                        {
                            db.AspNetUserRoles.DeleteOnSubmit(role); // Delete ASPNET User Roles
                        }
                        db.SubmitChanges();
                    }

                    db.AspNetUsers.DeleteOnSubmit(aspNetUser.First()); // Delete ASPNET User
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

        // GET api/GetUser/dpilger
        [Authorize]
        [Route("api/GetUser/{username}")]
        public List<Models.User> GetUserInfo(String UserName)
        {
            //List<Models.User> UserInfo = null;
            //var Info = from m in db.MstUsers
            //           where m.UserName == username
            //           select new Models.User
            //           {
            //               UserName = m.UserName,
            //               FirstName = m.FirstName,
            //               LastName = m.LastName,
            //               EmailAddress = m.EmailAddress,
            //               PhoneNumber = m.PhoneNumber
            //           };

            //UserInfo = Info.ToList();
            //return UserInfo;
            var retryCounter = 0;
            List<Models.User> values;

            while (true)
            {
                try
                {
                    var Info = from m in db.MstUsers
                               where m.UserName == UserName
                               select new Models.User
                               {
                                   Id = m.Id,
                                   UserName = m.UserName,
                                   FirstName = m.FirstName,
                                   LastName = m.LastName,
                                   EmailAddress = m.EmailAddress,
                                   PhoneNumber = m.PhoneNumber,
                                   Address = m.Address
                               };
                    if (Info.Count() > 0)
                    {
                        values = Info.ToList();
                    }
                    else
                    {
                        values = new List<Models.User>();
                    }
                    break;
                }
                catch
                {
                    if (retryCounter == 3)
                    {
                        values = new List<Models.User>();
                        break;
                    }

                    System.Threading.Thread.Sleep(1000);
                    retryCounter++;
                }
            }

            return values;
        }

        // GET api/GetRoleUsers/Web99
        [Authorize]
        [Route("api/GetRoleUsers/{role}")]
        public List<Models.User> GetRoleUsers(String Role)
        {
            var retryCounter = 0;
            List<Models.User> values;

            while (true)
            {
                try
                {
                    var Info = from m in db.MstUsers
                               where m.AspNetUser.AspNetUserRoles.Count(d=>d.AspNetRole.Name==Role) > 0
                               select new Models.User
                               {
                                   Id = m.Id,
                                   UserName = m.UserName,
                                   FirstName = m.FirstName,
                                   LastName = m.LastName,
                                   EmailAddress = m.EmailAddress,
                                   PhoneNumber = m.PhoneNumber,
                                   Address = m.Address,
                                   ReferralUserName = m.ReferralUserName
                               };
                    if (Info.Count() > 0)
                    {
                        values = Info.ToList();
                    }
                    else
                    {
                        values = new List<Models.User>();
                    }
                    break;
                }
                catch
                {
                    if (retryCounter == 3)
                    {
                        values = new List<Models.User>();
                        break;
                    }

                    System.Threading.Thread.Sleep(1000);
                    retryCounter++;
                }
            }

            return values;
        }

        //// GET api/AddUserRole/dpilger/1/1
        //[Authorize]
        //[Route("api/ModifyUserRole/{username}/{role}/{change}")]
        //public int AddUserRole(string Username, string Role, bool Change)
        //{
        //    try
        //    {
        //        var id = (from m in db.MstUsers
        //                  join n in db.AspNetUsers on m.AspNetUserId equals n.Id
        //                  where m.UserName == Username
        //                  select new Models.User
        //                  {
        //                      AspNetUserId = m.AspNetUserId
        //                  }).ToList();

        //        var AspNetUserId = id[0].AspNetUserId;
                
        //        Data.AspNetUserRole NewRole = new Data.AspNetUserRole();

        //        NewRole.UserId = AspNetUserId;
        //        NewRole.RoleId = Role;
                
        //        if(Change)
        //            db.AspNetUserRoles.InsertOnSubmit(NewRole);                
        //        try
        //        {
        //            db.SubmitChanges();
        //            return 1;
        //        }
        //        catch
        //        {
        //            return 0;
        //        }
        //    }
        //    catch
        //    {
        //        return 0;
        //    }
        //}

        //[Authorize]
        //[Route("api/GetRoles/{username}")]
        //public List<Models.User> GetRoles(string userName)
        //{
        //    try
        //    {
        //        try
        //        {
        //            var id = (from m in db.MstUsers
        //                      join n in db.AspNetUsers on m.AspNetUserId equals n.Id
        //                      where m.UserName == userName
        //                      select new Models.User
        //                      {
        //                          AspNetUserId = m.AspNetUserId
        //                      }).ToList();

        //            var AspNetUserId = id[0].AspNetUserId;

        //            try
        //            {
        //                var x = (from m in db.AspNetUserRoles
        //                         where m.UserId == AspNetUserId
        //                         select new Models.User
        //                         {
        //                            Roles = m.RoleId
        //                         }).Distinct().ToList();

        //                var y = x;
        //                return y;
        //            }
        //            catch
        //            {
        //                return null;
        //            }
        //        }
        //        catch
        //        {
        //            return null;
        //        }
                                        
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}


        
        //private string getAspNetUserId( string username )
        //{
        //    try
        //    {
        //       var x = (from m in db.MstUsers
        //                join n in db.AspNetUsers on m.AspNetUserId equals n.Id
        //                where m.UserName == username
        //                select new Models.User
        //                {
        //                    AspNetUserId = m.AspNetUserId
        //                }).ToList();

        //       return x[0].AspNetUserId;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}


        //// DELETE api/DeleteUser/5
        //[Authorize]
        //[Route("api/DeleteUserRole/{username}/{role}")]
        //public HttpResponseMessage Delete(string Username, string Role)
        //{
        //    var UserId = getAspNetUserId(Username);
        //    Data.AspNetUserRole DeleteRole = db.AspNetUserRoles.Where(d => d.RoleId == Role && d.UserId == UserId).First();
            
        //    if (DeleteRole != null)
        //    {
        //        db.AspNetUserRoles.DeleteOnSubmit(DeleteRole);
        //        try
        //        {
        //            db.SubmitChanges();
        //            return Request.CreateResponse(HttpStatusCode.OK);
        //        }
        //        catch
        //        {
        //            return Request.CreateResponse(HttpStatusCode.BadRequest);
        //        }
        //    }
        //    else
        //    {
        //        return Request.CreateResponse(HttpStatusCode.NotFound);
        //    }
        //}
    }
}
