using FlatMate.Module.Offers.Api;
using FlatMate.Web.Mvc.Base;
using System;
using System.Collections.Generic;

namespace FlatMate.Web.Areas.Offers.Data
{
    public class CompanyViewVm : MvcViewModel
    {
        public CompanyJso Company { get; set; }

        public List<OfferJso> Offers { get; set; } = new List<OfferJso>();

        public DateTime OffersFrom { get; set; }

        public DateTime OffersTo { get; set; }

        public List<ProductCategoryJso> ProductCategories { get; set; } = new List<ProductCategoryJso>();

        public List<OfferJso> Favorites { get; set; } = new List<OfferJso>();

        public List<MarketJso> Markets { get; set; } = new List<MarketJso>();
    }
}
