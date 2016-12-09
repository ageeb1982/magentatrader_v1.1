using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MagentaTrader.Controllers
{
    public class BookClubController : Controller
    {
        //
        // GET: /BookClub/
        [Authorize(Roles = "Book Club")]
        public ActionResult Index()
        {
            return View();
        }
	}
}