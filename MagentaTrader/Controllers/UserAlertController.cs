using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MagentaTrader.Controllers
{
    public class UserAlertController : Controller
    {
        //
        // GET: /UserAlert/
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
	}
}