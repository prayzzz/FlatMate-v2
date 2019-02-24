using System.Collections.Generic;
using FlatMate.Module.Offers.Api.Jso;
using FlatMate.Web.Mvc.Base;

namespace FlatMate.Web.Areas.Offers.Data
{
    public class ProductFavoriteManageVm : MvcViewModel
    {
        public List<CompanyJso> Companies { get; set; }

        public int? CurrentCompany { get; set; }
    }
}