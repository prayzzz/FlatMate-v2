using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using prayzzz.Common.Results;
using System;

namespace FlatMate.Module.Common.Api
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EnsurePostBodyFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!HttpMethods.IsPost(context.HttpContext.Request.Method)
                && !HttpMethods.IsPut(context.HttpContext.Request.Method))
            {
                return;
            }

            // get name of all body parameters
            var bodyParameterNames = context.ActionDescriptor
                                            .Parameters
                                            .Where(p => p.BindingInfo != null && p.BindingInfo.BindingSource == BindingSource.Body)
                                            .Select(p => p.Name);

            foreach (var name in bodyParameterNames)
            {
                if (context.ActionArguments.TryGetValue(name, out var argument))
                {
                    if (argument == null)
                    {
                        // argument null
                        context.Result = new BadRequestObjectResult(new ErrorResult(ErrorType.ValidationError, "Body should not empty"));
                    }
                }
                else
                {
                    // argument not found
                    context.Result = new BadRequestObjectResult(new ErrorResult(ErrorType.ValidationError, "Body should not empty"));
                }
            }
        }
    }
}