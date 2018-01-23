using System;
using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FlatMate.Web.Mvc;

namespace FlatMate.Web.Controllers
{
    public class ErrorController : MvcController
    {
        public ErrorController(ILogger<ErrorController> logger, IMvcControllerServices controllerService)
            : base(logger, controllerService)
        {
        }

        [HttpGet]
        public IActionResult ForceException()
        {
            throw new ArgumentNullException();
        }

        [HttpGet]
        public IActionResult Index(int? statusCode)
        {
            statusCode = statusCode ?? HttpContext.Response.StatusCode;

            switch (statusCode)
            {
                case 401:
                    return View("Unauthorized");
                case 404:
                    return View("PageNotFound");
            }

            return View();
        }

        [HttpGet]
        public IActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            return View("PageNotFound");
        }
    }
}