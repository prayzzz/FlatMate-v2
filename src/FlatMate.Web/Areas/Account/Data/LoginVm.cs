using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FlatMate.Web.Mvc.Base;

namespace FlatMate.Web.Areas.Account.Data
{
    public class LoginVm : BaseViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Passwort")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        [Required]
        [DisplayName("Benutzername")]
        public string UserName { get; set; }
    }
}