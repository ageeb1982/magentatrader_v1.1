using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MagentaTrader.Controllers
{
    public class CalendarController : Controller
    {
        //
        // GET: /Calendar/
        [Authorize(Roles = "Chart")]
        public ActionResult Index()
        {
            return View();
        }
	}
}