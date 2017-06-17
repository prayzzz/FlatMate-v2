﻿using System;
using System.Security.Claims;
using FlatMate.Api.Areas.Account.User;
using FlatMate.Web.Mvc.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Results;

namespace FlatMate.Web.Mvc.Base
{
    [Authorize]
    [ServiceFilter(typeof(MvcResultFilter))]
    public abstract class MvcController : Controller
    {
        private readonly IJsonService _jsonService;

        protected MvcController(IJsonService jsonService)
        {
            _jsonService = jsonService;
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

        public abstract ILogger Logger { get; }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);

            var viewResult = context.Result as ViewResult;
            if (viewResult == null)
            {
                return;
            }

            var model = viewResult.Model as BaseViewModel;
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

        protected T ApplyTempResult<T>(T model) where T : BaseViewModel
        {
            // check for passed result from redirect
            if (TempData.TryGetValue(Constants.TempData.Result, out var data))
            {
                try
                {
                    model.Result = _jsonService.Deserialize<Result>(data as string);
                }
                catch (Exception e)
                {
                    Logger.LogError(0, e, data as string);
                }
                
                TempData.Remove(Constants.TempData.Result);
            }

            return model;
        }
    }
}