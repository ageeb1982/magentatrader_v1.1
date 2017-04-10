using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MagentaTrader.Controllers
{
    public class VelocityController : Controller
    {
        //
        // GET: /Velocity/
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
	}
}