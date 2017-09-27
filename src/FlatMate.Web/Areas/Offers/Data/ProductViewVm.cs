using FlatMate.Module.Offers.Api;
using FlatMate.Web.Mvc.Base;
using System.Collections.Generic;

namespace FlatMate.Web.Areas.Offers.Data
{
    public class ProductViewVm : BaseViewModel
    {
        public List<OfferJso> Offers { get; set; } = new List<OfferJso>();

        public List<PriceHistoryJso> PriceHistory { get; set; } = new List<PriceHistoryJso>();

        public ProductJso Product { get; set; }
    }
}
