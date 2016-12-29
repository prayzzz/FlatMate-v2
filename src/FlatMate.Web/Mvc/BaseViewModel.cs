using FlatMate.Api.Areas.Account.User;
using prayzzz.Common.Result;

namespace FlatMate.Web.Mvc
{
    public class EmptyViewModel : BaseViewModel
    {
    }

    public abstract class BaseViewModel
    {
        public UserJso CurrentUser { get; set; }

        /// <summary>
        ///     Set by <see cref="ErrorResult" /> if <see cref="MvcResultFilter" /> is filled
        /// </summary>
        public string ErrorMessage { get; set; }

        public Result ErrorResult { get; set; }

        public bool HasError => ErrorResult != null || !string.IsNullOrEmpty(ErrorMessage);
    }
}