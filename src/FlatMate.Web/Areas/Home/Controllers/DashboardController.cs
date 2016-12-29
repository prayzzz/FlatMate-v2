﻿using FlatMate.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace FlatMate.Web.Areas.Home.Controllers
{
    [Area("Home")]
    public class DashboardController : MvcController
    {
        public IActionResult Index()
        {
            return RedirectPermanent("/account/login");
        } 
    }
}