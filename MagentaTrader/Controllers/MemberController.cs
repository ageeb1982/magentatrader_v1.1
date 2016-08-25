using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MagentaTrader.Controllers
{
    public class MemberController : Controller
    {
        //
        // GET: /Member/
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
	}
}