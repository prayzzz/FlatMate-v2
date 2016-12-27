using FlatMate.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using prayzzz.Common.Attributes;
using prayzzz.Common.Enums;
using prayzzz.Common.Result;

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

            if (objectResult == null)
            {
                return;
            }

            // check for generic result
            var genericResult = objectResult.Value as IResult<object>;

            if (genericResult != null)
            {
                context.Result = _resultService.Get(genericResult);
                return;
            }

            // check for non-generic result
            var result = objectResult.Value as Result;

            if (result != null)
            {
                context.Result = _resultService.Get(result);
            }
        }
    }
}