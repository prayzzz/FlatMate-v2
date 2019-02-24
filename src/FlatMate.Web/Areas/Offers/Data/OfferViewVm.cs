using System;
using System.Collections.Generic;
using System.Linq;
using FlatMate.Module.Offers.Api;
using FlatMate.Module.Offers.Api.Jso;
using FlatMate.Web.Mvc.Base;

namespace FlatMate.Web.Areas.Offers.Data
{
    public class OfferViewVm : MvcViewModel
    {
        public IEnumerable<OfferViewJso.ProductCategoriesWithOffers> Categories { get; set; } = Enumerable.Empty<OfferViewJso.ProductCategoriesWithOffers>();

        public CompanyJso Company { get; set; }

        public IEnumerable<OfferViewJso.OfferedProductJso> FavoriteProducts { get; set; } = Enumerable.Empty<OfferViewJso.OfferedProductJso>();

        public IEnumerable<MarketJso> Markets { get; set; } = Enumerable.Empty<MarketJso>();

        public int OfferCount { get; set; }

        public DateTime OffersFrom { get; set; }

        public DateTime OffersTo { get; set; }
    }
}