using System.Collections.Generic;
using System.Linq;
using FlatMate.Module.Offers.Api.Jso;
using FlatMate.Module.Offers.Domain.Companies;
using FlatMate.Web.Mvc.Base;

namespace FlatMate.Web.Areas.Offers.Data
{
    public class AdminManageProductDuplicatesVm : MvcViewModel
    {
        public List<CompanyJso> Companies { get; set; }

        public List<IGrouping<(string Name, Company CompanyId), ProductJso>> GroupedProducts { get; set; }
    }
}