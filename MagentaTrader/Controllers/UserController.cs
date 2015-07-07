﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Security;
using System.Net.Http;
using System.Web.Http;

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
                    var query1 = from sale in db.TrnSales
                                 group sale by sale.UserId into g
                                 let maxDate = g.Max(s => s.SalesDate)
                                 select new { UserId = g.Key, maxDate };

                    var query2 = from user in db.MstUsers
                                 from q1 in query1.Where(q => q.UserId == user.Id)
                                 select new {
                                     Id = user.Id,
                                     UserName = user.UserName,
                                     FirstName = user.FirstName,
                                     LastName = user.LastName,
                                     EmailAddress = user.EmailAddress,
                                     PhoneNumber = user.PhoneNumber,
                                     q1.maxDate
                                 };

                    var result = query2.ToList();

                    var Users = from d in db.MstUsers
                                join s in
                                    (from sale in db.TrnSales
                                     group sale by sale.Id into grp
                                     select new
                                     {
                                         Id = grp.Key,
                                         LastPurchase = grp.Max(item => item.SalesDate)
                                     })
                                on d.Id equals s.Id
                                select new Models.User
                                {
                                    Id = d.Id,
                                    UserName = d.UserName,
                                    FirstName = d.FirstName,
                                    LastName = d.LastName,
                                    EmailAddress = d.EmailAddress,
                                    PhoneNumber = d.PhoneNumber,
                                    LastPurchase = Convert.ToString(s.LastPurchase.Year) + "-" + Convert.ToString(s.LastPurchase.Month + 100).Substring(1, 2) + "-" + Convert.ToString(s.LastPurchase.Day + 100).Substring(1, 2)
                                };
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
        public List<Models.User> GetUserInfo( String UserName )
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
                                   PhoneNumber = m.PhoneNumber
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
