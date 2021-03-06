﻿using System;
using System.Security.Claims;
using FlatMate.Module.Account.Api.Jso;
using FlatMate.Web.Mvc.Authorization;
using FlatMate.Web.Mvc.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Results;

namespace FlatMate.Web.Mvc.Base
{
    [Authorize]
    [ServiceFilter(typeof(AreaRestrictionFilter))]
    public abstract class MvcController : Controller
    {
        protected readonly IJsonService JsonService;
        protected readonly ILogger Logger;

        protected MvcController(ILogger logger, IMvcControllerServices controllerService)
        {
            Logger = logger;
            JsonService = controllerService.JsonService;
        }

        protected int CurrentUserId
        {
            get
            {
                var userId = User?.FindFirst(ClaimTypes.Sid)?.Value;
                return userId == null ? 0 : Convert.ToInt32(userId);
            }
        }

        protected string CurrentUserName
        {
            get
            {
                var userId = User?.FindFirst(ClaimTypes.Name)?.Value;
                return userId ?? "";
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Sets the current user into the viewmodel after an action is executed.
        ///     Viewmodel must be of type <see cref="T:FlatMate.Web.Mvc.Base.BaseViewModel" />.
        /// </summary>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);

            var viewResult = context.Result as ViewResult;
            if (viewResult == null)
            {
                return;
            }

            if (viewResult.Model == null)
            {
                viewResult.ViewData.Model = new EmptyViewModel();
            }

            var model = viewResult.Model as MvcViewModel;
            if (model == null)
            {
                return;
            }

            model.CurrentUser = new UserInfoJso
            {
                Id = CurrentUserId,
                UserName = CurrentUserName
            };
        }

        /// <summary>
        ///     check for passed result from redirect and sets it to the given model.
        /// </summary>
        protected T ApplyTempResult<T>(T model) where T : MvcViewModel
        {
            if (TempData.TryGetValue(Constants.TempData.Result, out var data))
            {
                try
                {
                    model.Result = JsonService.Deserialize<Result>(data as string);
                }
                catch (Exception e)
                {
                    Logger.LogError(0, e, data as string);
                }

                TempData.Remove(Constants.TempData.Result);
            }

            return model;
        }

        protected void SetTempResult(Result result)
        {
            TempData[Constants.TempData.Result] = JsonService.Serialize(result);
        }

        protected IActionResult TryRedirectToReferer(IActionResult fallback)
        {
            var referer = HttpContext.Request.Headers["Referer"].ToString();

            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }

            return fallback;
        }
    }
}