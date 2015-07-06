using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MagentaTrader.Controllers
{
    public class RolesController : ApiController
    {
        private Data.MagentaTradersDBDataContext db = new Data.MagentaTradersDBDataContext();


        [Authorize]
        [Route("api/GetRoles")]
        public List<Models.Roles> GetRoles()
        {
            var retryCounter = 0;
            List<Models.Roles> values;
            
            while(true)
            {
                try
                {
                    var Roles = from r in db.AspNetRoles
                                select new Models.Roles
                                {
                                    Id = r.Id,
                                    Name = r.Name
                                };

                    if (Roles.Count() > 0)
                    {
                        values = Roles.ToList();
                    }
                    else
                    {
                        values = new List<Models.Roles>();
                    }
                    break;
                }
                catch (Exception)
                {
                    if (retryCounter == 3)
                    {
                        values = new List<Models.Roles>();
                        break;
                    }

                    System.Threading.Thread.Sleep(1000);
                    retryCounter++;
                }
            }

            return values;
        }

        // GET api/AddUserRole/dpilger/1/1
        [Authorize]
        [Route("api/ModifyUserRole/{username}/{role}/{change}")]
        public int AddUserRole(string Username, string Role, bool Change)
        {
            try
            {
                var id = (from m in db.MstUsers
                          join n in db.AspNetUsers on m.AspNetUserId equals n.Id
                          where m.UserName == Username
                          select new Models.User
                          {
                              AspNetUserId = m.AspNetUserId
                          }).ToList();

                var AspNetUserId = id[0].AspNetUserId;

                Data.AspNetUserRole NewRole = new Data.AspNetUserRole();

                NewRole.UserId = AspNetUserId;
                NewRole.RoleId = Role;

                if (Change)
                    db.AspNetUserRoles.InsertOnSubmit(NewRole);
                try
                {
                    db.SubmitChanges();
                    return 1;
                }
                catch
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
        [Route("api/GetUserRoles/{username}")]
        public List<Models.Roles> GetRoles(string userName)
        {
            try
            {
                try
                {
                    // get roles from user by username
                    var id = (from m in db.MstUsers
                              join n in db.AspNetUsers on m.AspNetUserId equals n.Id
                              where m.UserName == userName
                              select new Models.User
                              {
                                  AspNetUserId = m.AspNetUserId
                              }).ToList();

                    var AspNetUserId = id[0].AspNetUserId;

                    try
                    {
                        var x = (from m in db.AspNetUserRoles
                                 where m.UserId == AspNetUserId
                                 select new Models.Roles
                                 {
                                     Id = m.RoleId,
                                     Name = m.AspNetRole.Name
                                 }).Distinct().ToList();

                        var y = x;
                        return y;
                    }
                    catch
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }

            }
            catch
            {
                return null;
            }
        }



        private string getAspNetUserId(string username)
        {
            try
            {
                var x = (from m in db.MstUsers
                         join n in db.AspNetUsers on m.AspNetUserId equals n.Id
                         where m.UserName == username
                         select new Models.User
                         {
                             AspNetUserId = m.AspNetUserId
                         }).ToList();

                return x[0].AspNetUserId;
            }
            catch
            {
                return null;
            }
        }

        // DELETE api/DeleteUser/5
        [Authorize]
        [Route("api/DeleteUserRole/{username}/{role}")]
        public HttpResponseMessage Delete(string Username, string Role)
        {
            var UserId = getAspNetUserId(Username);
            Data.AspNetUserRole DeleteRole = db.AspNetUserRoles.Where(d => d.RoleId == Role && d.UserId == UserId).First();

            if (DeleteRole != null)
            {
                db.AspNetUserRoles.DeleteOnSubmit(DeleteRole);
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
