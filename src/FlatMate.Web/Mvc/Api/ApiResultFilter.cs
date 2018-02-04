using System;
using System.Runtime.CompilerServices;
using FlatMate.Module.Common;
using FlatMate.Module.Common.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Attributes;
using prayzzz.Common.Enums;
using prayzzz.Common.Results;

namespace FlatMate.Web.Mvc.Api
{
    [AttributeUsage(AttributeTargets.Class)]
    [Inject(DependencyLifetime.Singleton, typeof(ApiResultFilter))]
    public class ApiResultFilter : ActionFilterAttribute
    {
        private readonly ILogger<ApiResultFilter> _logger;
        private readonly IRequestResultService _resultService;

        public ApiResultFilter(IRequestResultService resultService, ILogger<ApiResultFilter> logger)
        {
            _resultService = resultService;
            _logger = logger;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (!(context.Result is ObjectResult actionResult) || actionResult.Value == null)
            {
                return;
            }

            switch (actionResult.Value)
            {
                case ITuple tuple:
                    context.Result = CreateResultFromTuple(context, tuple);
                    return;
//                case IResult<object> result:
//                    context.Result = _resultService.Get(result, result.Data, context.HttpContext);
//                    return;
//                case Result result:
//                    context.Result = _resultService.Get(result, context.HttpContext);
//                    break;
            }
        }

        private IActionResult CreateResultFromTuple(ActionExecutedContext context, ITuple tupleResult)
        {
            switch (tupleResult.Length)
            {
                case 1 when tupleResult[0] is Result result:
                    return _resultService.Get(result, context.HttpContext);
                case 2 when tupleResult[0] is Result result:
                    return _resultService.Get(result, tupleResult[1], context.HttpContext);
                default:
                    _logger.LogWarning("Cannot't handle ITuple with {} parameters", tupleResult.Length);
                    return context.Result;
            }
        }
    }
}