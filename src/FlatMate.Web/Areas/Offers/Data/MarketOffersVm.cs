using FlatMate.Module.Offers.Api;
using FlatMate.Web.Mvc.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FlatMate.Web.Areas.Offers.Data
{
    public class MarketOffersVm : MvcViewModel
    {
        public CompanyJso Company { get; set; }

        public IEnumerable<OfferViewJso.ProductCategoriesWithOffers> Categories { get; set; } = Enumerable.Empty<OfferViewJso.ProductCategoriesWithOffers>();

        public DateTime OffersFrom { get; set; }

        public DateTime OffersTo { get; set; }

        public IEnumerable<OfferViewJso.OfferedProductJso> FavoriteProducts { get; set; } = Enumerable.Empty<OfferViewJso.OfferedProductJso>();

        public Dictionary<int, MarketJso> Markets { get; set; } = new Dictionary<int, MarketJso>();

        public int OfferCount { get; set; }
    }
}