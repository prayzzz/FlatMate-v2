using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using prayzzz.Common.Attributes;
using prayzzz.Common.Enums;
using System;

namespace FlatMate.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Class)]
    [Inject(DependencyLifetime.Singleton, typeof(MvcResultFilter))]
    public class MvcResultFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var viewResult = context.Result as ViewResult;

            if (viewResult == null)
            {
                return;
            }

            var model = viewResult.Model as MvcViewModel;

            if (model == null || !model.IsError || model.Result == null)
            {
            }
        }
    }
}