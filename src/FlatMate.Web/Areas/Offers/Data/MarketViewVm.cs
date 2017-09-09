using FlatMate.Module.Offers.Api.Jso;
using FlatMate.Web.Mvc.Base;
using System.Collections.Generic;

namespace FlatMate.Web.Areas.Offers.Data
{
    public class MarketViewVm : BaseViewModel
    {
        public List<OfferJso> Offers { get; set; } = new List<OfferJso>();

        public MarketJso Market { get; set; }

        public Dictionary<int, ProductCategoryJso> ProductCategories { get; set; } = new Dictionary<int, ProductCategoryJso>();
    }
}
