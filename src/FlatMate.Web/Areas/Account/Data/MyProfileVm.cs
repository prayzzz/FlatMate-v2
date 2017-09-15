using FlatMate.Module.Account.Api.Jso;
using FlatMate.Web.Mvc.Base;

namespace FlatMate.Web.Areas.Account.Data
{
    public class MyProfileVm : BaseViewModel
    {
        public UserInfoJso UserJso { get; set; }
    }
}