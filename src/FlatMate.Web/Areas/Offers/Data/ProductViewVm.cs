using System.Collections.Generic;
using FlatMate.Module.Offers.Api.Jso;
using FlatMate.Web.Mvc.Base;

namespace FlatMate.Web.Areas.Offers.Data
{
    public class ProductViewVm : MvcViewModel
    {
        public bool IsFavorite { get; set; }

        public List<OfferJso> Offers { get; set; } = new List<OfferJso>();

        public List<PriceHistoryJso> PriceHistory { get; set; } = new List<PriceHistoryJso>();

        public ProductJso Product { get; set; }
    }
}