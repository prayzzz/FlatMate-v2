using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using prayzzz.Common.Attributes;
using prayzzz.Common.Enums;

namespace FlatMate.Web.Mvc
{
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

            var model = viewResult.Model as BaseViewModel;

            if (model == null || !model.IsError || model.ErrorResult == null)
            {
                return;
            }

            model.ErrorMessage = string.Format(model.ErrorResult.ErrorMessage, model.ErrorResult.ErrorMessageArgs);
        }
    }
}