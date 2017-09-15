using FlatMate.Module.Account.Api.Jso;
using prayzzz.Common.Results;

namespace FlatMate.Web.Mvc.Base
{
    public class EmptyViewModel : BaseViewModel
    {
    }

    public abstract class BaseViewModel
    {
        public UserInfoJso CurrentUser { get; set; }

        public bool IsError => Result?.IsError == true;

        public bool IsSuccess => Result?.IsSuccess == true;

        public Result Result { get; set; }
    }
}