using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MagentaTrader.Controllers
{
    public class FavoritesController : Controller
    {
        //
        // GET: /Favorites/
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
	}
}