using System.Collections.Generic;
using System.Linq;
using FlatMate.Module.Offers.Api;
using FlatMate.Module.Offers.Domain;
using FlatMate.Web.Mvc.Base;

namespace FlatMate.Web.Areas.Offers.Data
{
    public class AdminManageProductDuplicatesVm : MvcViewModel
    {
        public List<IGrouping<(string Name, string SizeInfo, Company CompanyId), ProductJso>> GroupedProducts { get; set; }

        public List<CompanyJso> Companies { get; set; }
    }
}