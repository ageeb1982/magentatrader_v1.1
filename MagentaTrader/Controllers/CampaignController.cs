using MagentaTrader.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MagentaTrader.Controllers
{
    public class CampaignController : Controller
    {
        public CampaignController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public CampaignController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        //
        // GET: /Campaign/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Campaign/Anniversary5
        public ActionResult Anniversary5()
        {
            return View();
        }

        //
        // GET: /Campaign/AlaskaCruise2016
        public ActionResult AlaskaCruise2016()
        {
            return View();
        }

        //
        // GET: /Campaign/SeasonalityWorkshop01
        public ActionResult SeasonalityWorkshop01()
        {
            return View();
        }

        //
        // GET: /Campaign/Tradier
        public ActionResult Tradier()
        {
            return View();
        }

        //
        // GET: /Campaign/Questrade
        public ActionResult Questrade()
        {
            return View();
        }

        //
        // GET: /Campaign/BetaTesters
        public ActionResult BetaTesters()
        {
            return View();
        }

        //
        // GET: /Campaign/Referral
        [Authorize]
        public ActionResult Referral()
        {
            return View();
        }

        //
        // GET: /Campaign/LV20161113
        public ActionResult LV20161113()
        {
            return View();
        }

        //
        // GET: /Campaign/IW20160619
        public ActionResult IW20160619()
        {
            return View();
        }

        //
        // GET: /Campaign/Web99
        public ActionResult Web99()
        {
            return View();
        }
        //
        // GET: /Campaign/ViewVideo
        public ActionResult ViewVideo()
        {
            return View();
        }

        //
        // GET: /Campaign/MagentaTour2017
        public ActionResult MagentaTour2017()
        {
            return View();
        }

        // Account Registration Pre-Requisite modules
        public UserManager<ApplicationUser> UserManager { get; private set; }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        //
        // GET: /Campaign/BetaTesters2017
        public async Task<ActionResult> BetaTesters2017(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user, model.Password);

                var response = HttpContext.Request.Form["g-recaptcha-response"] == null ? "" : HttpContext.Request.Form["g-recaptcha-response"];
                string secretKey = "6Lc5GBoTAAAAAOQFNfUBzRtzN_I-vmyJzGugEx65";
                var client = new System.Net.WebClient();
                var verificationResultJson = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
                var verificationResult = JsonConvert.DeserializeObject<CaptchaVerificationResult>(verificationResultJson);
                if (!verificationResult.Success)
                {
                    ModelState.AddModelError("", "ERROR: Invalid recaptcha challenge.");
                }
                else
                {
                    if (result.Succeeded)
                    {
                        // Add or update MstUser table
                        try
                        {
                            await SignInAsync(user, isPersistent: false);

                            Data.MagentaTradersDBDataContext db = new Data.MagentaTradersDBDataContext();

                            var Users = from d in db.MstUsers where d.UserName == model.UserName select d;

                            if (Users.Any())
                            {
                                var UpdatedUser = Users.FirstOrDefault();

                                UpdatedUser.AspNetUserId = db.AspNetUsers.Where(d => d.UserName == model.UserName).FirstOrDefault().Id;

                                db.SubmitChanges();
                            }
                            else
                            {
                                Data.MstUser NewUser = new Data.MstUser();

                                NewUser.UserName = model.UserName;
                                NewUser.FirstName = model.FirstName == null || model.FirstName.Length == 0 ? "NA" : model.FirstName;
                                NewUser.LastName = model.LastName == null || model.LastName.Length == 0 ? "NA" : model.LastName;
                                NewUser.EmailAddress = model.EmailAddress == null || model.EmailAddress.Length == 0 ? "NA" : model.EmailAddress;
                                NewUser.PhoneNumber = model.PhoneNumber == null || model.PhoneNumber.Length == 0 ? "NA" : model.PhoneNumber;
                                NewUser.Address = model.Address == null || model.Address.Length == 0 ? "" : model.Address;
                                NewUser.ReferralUserName = model.ReferralUserName == null || model.ReferralUserName.Length == 0 ? "" : model.ReferralUserName;
                                NewUser.AspNetUserId = db.AspNetUsers.Where(d => d.UserName == model.UserName).FirstOrDefault().Id;

                                DateTime dateCreated = DateTime.Now;
                                SqlDateTime dateCreatedSQL = new SqlDateTime(new DateTime(dateCreated.Year, +
                                                                                          dateCreated.Month, +
                                                                                          dateCreated.Day));
                                NewUser.DateCreated = dateCreatedSQL.Value;

                                db.MstUsers.InsertOnSubmit(NewUser);
                                db.SubmitChanges();

                                Data.AspNetUserRole NewRole1 = new Data.AspNetUserRole();

                                NewRole1.AspNetUser = db.AspNetUsers.Where(d => d.UserName == model.UserName).FirstOrDefault();
                                NewRole1.AspNetRole = db.AspNetRoles.Where(d => d.Name == "Quest").FirstOrDefault();

                                db.AspNetUserRoles.InsertOnSubmit(NewRole1);
                                db.SubmitChanges();

                                Data.AspNetUserRole NewRole2 = new Data.AspNetUserRole();

                                NewRole2.AspNetUser = db.AspNetUsers.Where(d => d.UserName == model.UserName).FirstOrDefault();
                                NewRole2.AspNetRole = db.AspNetRoles.Where(d => d.Name == "Chart").FirstOrDefault();

                                db.AspNetUserRoles.InsertOnSubmit(NewRole2);
                                db.SubmitChanges();

                                Data.AspNetUserRole NewRole3 = new Data.AspNetUserRole();

                                NewRole3.AspNetUser = db.AspNetUsers.Where(d => d.UserName == model.UserName).FirstOrDefault();
                                NewRole3.AspNetRole = db.AspNetRoles.Where(d => d.Name == "Web99").FirstOrDefault();

                                db.AspNetUserRoles.InsertOnSubmit(NewRole3);
                                db.SubmitChanges();

                            }
                            return RedirectToAction("Index", "Help");
                            //return RedirectToAction("Index", "Home");
                        }
                        catch (Exception e)
                        {
                            ModelState.AddModelError("", "ERROR: Try again. " + e.ToString());
                        }
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            return View(model);
        }

        //
        // GET: /Campaign/BetaTesters2017Video
        public ActionResult BetaTesters2017Video()
        {
            return View();
        }

        //
        // GET: /Campaign/IW20170305
        public ActionResult IW20170305()
        {
            return View();
        }

        //
        // GET: /Campaign/LV20171030
        public ActionResult LV20171030()
        {
            return View();
        }

        //
        // GET: /Campaign/BC20170508
        public ActionResult BC20170508()
        {
            return View();
        }

        //
        // GET: /Campaign/TradersWorld2017
        public ActionResult TradersWorld2017()
        {
            return View();
        }

        //
        // GET: /Campaign/CarnivalCruise2017
        public ActionResult CarnivalCruise2017()
        {
            return View();
        }
	}
}