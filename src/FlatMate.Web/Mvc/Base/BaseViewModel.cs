using FlatMate.Api.Areas.Account.User;
using prayzzz.Common.Result;

namespace FlatMate.Web.Mvc.Base
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

        public bool IsError => ErrorResult != null || !string.IsNullOrEmpty(ErrorMessage);

        public bool IsSuccess => SuccessMessage != null || !string.IsNullOrEmpty(SuccessMessage);

        public string SuccessMessage { get; set; }
    }
}