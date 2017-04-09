﻿using FlatMate.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using prayzzz.Common.Attributes;
using prayzzz.Common.Enums;
using prayzzz.Common.Results;

namespace FlatMate.Api.Filter
{
    [Inject(DependencyLifetime.Singleton, typeof(ApiResultFilter))]
    public class ApiResultFilter : ActionFilterAttribute
    {
        private readonly IRequestResultService _resultService;

        public ApiResultFilter(IRequestResultService resultService)
        {
            _resultService = resultService;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var objectResult = context.Result as ObjectResult;
            var requestMethod = context.HttpContext.Request.Method;

            if (objectResult == null)
            {
                return;
            }

            // check for generic result
            if (objectResult.Value is IResult<object> genericResult)
            {
                context.Result = _resultService.Get(genericResult);
                return;
            }

            // check for non-generic result
            if (objectResult.Value is Result result)
            {
                context.Result = _resultService.Get(result);
            }
        }
    }
}