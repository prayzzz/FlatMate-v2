using FlatMate.Module.Offers.Api;
using FlatMate.Web.Mvc.Base;
using System.Collections.Generic;

namespace FlatMate.Web.Areas.Offers.Data
{
    public class MarketIndexVm : BaseViewModel
    {
        public List<MarketJso> Markets { get; set; } = new List<MarketJso>();
    }
}
