﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MagentaTrader.Controllers
{
    public class PurchaseController : Controller
    {
        //
        // GET: /Purchase/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Purchase/Affiliate
        public ActionResult Affiliate()
        {
            return View();
        }
	}
}