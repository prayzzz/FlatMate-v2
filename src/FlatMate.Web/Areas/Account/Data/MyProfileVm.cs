using FlatMate.Api.Areas.Account.User;
using FlatMate.Web.Mvc.Base;

namespace FlatMate.Web.Areas.Account.Data
{
    public class MyProfileVm : BaseViewModel
    {
        public UserJso UserJso { get; set; }
    }
}