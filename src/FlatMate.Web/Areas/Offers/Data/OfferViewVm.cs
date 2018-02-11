using FlatMate.Module.Offers.Api;
using FlatMate.Web.Mvc.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlatMate.Web.Areas.Offers.Data
{
    public class OfferViewVm : MvcViewModel
    {
        public CompanyJso Company { get; set; }

        public IEnumerable<OfferViewJso.ProductCategoriesWithOffers> Categories { get; set; } = Enumerable.Empty<OfferViewJso.ProductCategoriesWithOffers>();

        public DateTime OffersFrom { get; set; }

        public DateTime OffersTo { get; set; }

        public IEnumerable<OfferViewJso.OfferedProductJso> FavoriteProducts { get; set; } = Enumerable.Empty<OfferViewJso.OfferedProductJso>();

        public IEnumerable<MarketJso> Markets { get; set; } = Enumerable.Empty<MarketJso>();

        public int OfferCount { get; set; }
    }
}