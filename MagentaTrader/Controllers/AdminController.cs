using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MagentaTrader.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Admin/Event
        [Authorize]
        public ActionResult Event()
        {
            return View();
        }

        //
        // GET: /Admin/User
        [Authorize(Users = "dpilger")]
        public ActionResult User()
        {
            return View();
        }

        //
        // GET: /Admin/UserLedger
        [Authorize(Users = "dpilger")]
        public ActionResult UserLedger()
        {
            return View();
        }

        //
        // GET: /Admin/News
        [Authorize(Users = "dpilger")]
        public ActionResult News()
        {
            return View();
        }


        //
        // GET: /Admin/Product
        [Authorize(Users = "dpilger")]
        public ActionResult Product()
        {
            return View();
        }

        //
        // GET: /Admin/Package
        [Authorize(Users = "dpilger")]
        public ActionResult Package()
        {
            return View();
        }

        //
        // GET: /Admin/PackageLedger
        [Authorize(Users = "dpilger")]
        public ActionResult PackageLedger()
        {
            return View();
        }

        //
        // GET: /Admin/Sales
        [Authorize(Users = "dpilger")]
        public ActionResult Sales()
        {
            return View();
        }

        //
        // GET: /Admin/Roles
        [Authorize(Users = "dpilger")]
        public ActionResult Roles()
        {
            return View();
        }
    }
}