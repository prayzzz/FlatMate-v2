using System;
using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FlatMate.Web.Mvc;

namespace FlatMate.Web.Controllers
{
    public class ErrorController : MvcController
    {
        public ErrorController(ILogger<ErrorController> logger, IMvcControllerService controllerService)
            : base(logger, controllerService)
        {
        }

        [HttpGet]
        public IActionResult ForceException()
        {
            throw new ArgumentNullException();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            return View();
        }
    }
}