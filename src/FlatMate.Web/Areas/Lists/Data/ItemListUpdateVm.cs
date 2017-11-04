using System.ComponentModel.DataAnnotations;
using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Mvc;

namespace FlatMate.Web.Areas.Lists.Data
{
    public class ItemListUpdateVm : MvcViewModel
    {
        [Display(Name = "Beschreibung")]
        public string Description { get; set; }

        [HiddenInput]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Öffentlich")]
        public bool IsPublic { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
    }
}