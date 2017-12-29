using FlatMate.Module.Offers.Api;
using FlatMate.Web.Mvc.Base;
using System.Collections.Generic;

namespace FlatMate.Web.Areas.Offers.Data
{
    public class CompanyIndexVm : MvcViewModel
    {
        public List<CompanyJso> Companies { get; set; } = new List<CompanyJso>();
    }
}
