using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MagentaTrader.Controllers
{
    public class MoxieController : Controller
    {
        // GET: Moxie
        [Authorize(Roles = "Moxie")]
        public ActionResult Index()
        {
            return View();
        }
    }
}