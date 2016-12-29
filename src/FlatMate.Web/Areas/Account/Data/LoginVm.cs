using System.ComponentModel;
using FlatMate.Web.Mvc;

namespace FlatMate.Web.Areas.Account.Data
{
    public class LoginVm : BaseViewModel
    {
        [DisplayName("Benutzername")]
        public string UserName { get; set; }

        [DisplayName("Passwort")]
        public string Password { get; set; }
    }
}