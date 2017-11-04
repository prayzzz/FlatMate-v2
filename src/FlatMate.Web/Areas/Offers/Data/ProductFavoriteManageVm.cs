using FlatMate.Module.Offers.Api;
using FlatMate.Web.Mvc.Base;
using System.Collections.Generic;

namespace FlatMate.Web.Areas.Offers.Data
{
    public class ProductFavoriteManageVm : MvcViewModel
    {
        public int? CurrentMarket { get; set; }

        public List<MarketJso> Markets { get; set; }
    }
}
