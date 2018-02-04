using FlatMate.Module.Account.Api.Jso;
using FlatMate.Module.Common;
using prayzzz.Common.Results;

namespace FlatMate.Web.Mvc.Base
{
    public class EmptyViewModel : MvcViewModel
    {
    }

    public abstract class MvcViewModel
    {
        public UserInfoJso CurrentUser { get; set; }

        public bool IsError => Result.HasValue && Result.Value.IsError;

        public bool IsSuccess => Result.HasValue && Result.Value.IsSuccess;

        public Result? Result { get; set; }
    }
}