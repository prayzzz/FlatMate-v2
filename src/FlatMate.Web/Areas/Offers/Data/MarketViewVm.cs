using FlatMate.Module.Offers.Api.Jso;
using FlatMate.Web.Mvc.Base;
using System.Collections.Generic;
using System.Linq;

namespace FlatMate.Web.Areas.Offers.Data
{
    public class MarketViewVm : BaseViewModel
    {
        public IEnumerable<OfferJso> Offers { get; set; } = Enumerable.Empty<OfferJso>();

        public MarketJso Market { get; set; }

        public Dictionary<int, ProductCategoryJso> ProductCategories { get; set; } = new Dictionary<int, ProductCategoryJso>();
    }
}
