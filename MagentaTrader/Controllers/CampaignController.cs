using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MagentaTrader.Controllers
{
    public class CampaignController : Controller
    {
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
	}
}