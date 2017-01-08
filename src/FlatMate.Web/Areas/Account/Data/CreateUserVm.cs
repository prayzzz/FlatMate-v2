using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FlatMate.Web.Mvc.Base;

namespace FlatMate.Web.Areas.Account.Data
{
    public class CreateUserVm : BaseViewModel
    {
        [Required]
        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Passwort")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Passwort bestätigen")]
        public string PasswordConfirmation { get; set; }

        [Required]
        [DisplayName("Benutzername")]
        public string UserName { get; set; }
    }
}