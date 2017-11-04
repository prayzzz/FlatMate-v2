using System.Collections.Generic;
using System.Linq;
using FlatMate.Module.Offers.Api;
using FlatMate.Web.Mvc.Base;

namespace FlatMate.Web.Areas.Offers.Data
{
    public class AdminManageProductDuplicatesVm : MvcViewModel
    {
        public List<IGrouping<(string Name, string SizeInfo, int MarketId), ProductJso>> GroupedProducts { get; set; }

        public List<MarketJso> Markets { get; set; }
    }
}