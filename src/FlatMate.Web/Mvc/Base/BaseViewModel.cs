using FlatMate.Api.Areas.Account.User;
using prayzzz.Common.Results;

namespace FlatMate.Web.Mvc.Base
{
    public class EmptyViewModel : BaseViewModel
    {
    }

    public abstract class BaseViewModel
    {
        public UserJso CurrentUser { get; set; }

        public bool IsError => Result != null && Result.IsError;

        public bool IsSuccess => Result != null && Result.IsSuccess;

        /// <summary>
        ///     Set by <see cref="Result" /> if <see cref="MvcResultFilter" /> is filled
        /// </summary>
        //public string ErrorMessage { get; set; }

        public Result Result { get; set; }
    }
}