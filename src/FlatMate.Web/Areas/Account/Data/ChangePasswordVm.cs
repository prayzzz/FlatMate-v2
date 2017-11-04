using System.ComponentModel.DataAnnotations;
using FlatMate.Web.Mvc.Base;

namespace FlatMate.Web.Areas.Account.Data
{
    public class ChangePasswordVm : MvcViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Neues Passwort")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Neues Passwort bestätigen")]
        public string NewPasswordConfirmation { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Aktuelles Passwort")]
        public string OldPassword { get; set; }
    }
}