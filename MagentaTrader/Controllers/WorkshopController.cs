using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MagentaTrader.Controllers
{
    public class WorkshopController : Controller
    {
        //
        // GET: /Workshop/
        [Authorize(Roles = "Workshop")]
        public ActionResult Index()
        {
            return View();
        }
	}
}