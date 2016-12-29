using FlatMate.Web.Mvc;

namespace FlatMate.Web.Areas.Account.Data
{
    public class RegisterVm : BaseViewModel
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string PasswordConfirmation { get; set; }
    }
}