using FlatMate.Module.Offers.Api;
using FlatMate.Web.Mvc.Base;
using System;
using System.Collections.Generic;

namespace FlatMate.Web.Areas.Offers.Data
{
    public class MarketViewVm : MvcViewModel
    {
        public MarketJso Market { get; set; }

        public List<OfferJso> Offers { get; set; } = new List<OfferJso>();

        public DateTime OffersFrom { get; set; }

        public DateTime OffersTo { get; set; }

        public Dictionary<int, ProductCategoryJso> ProductCategories { get; set; } = new Dictionary<int, ProductCategoryJso>();

        public List<OfferJso> Favorites { get; set; } = new List<OfferJso>();
    }
}
