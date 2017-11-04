using System.ComponentModel.DataAnnotations;
using FlatMate.Web.Mvc.Base;

namespace FlatMate.Web.Areas.Lists.Data
{
    public class ItemListCreateVm : MvcViewModel
    {
        [Display(Name = "Beschreibung")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Öffentlich")]
        public bool IsPublic { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
    }
}