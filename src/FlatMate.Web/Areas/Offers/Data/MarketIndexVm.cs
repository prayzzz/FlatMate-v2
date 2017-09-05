using FlatMate.Module.Offers.Api.Jso;
using FlatMate.Web.Mvc.Base;
using System.Collections.Generic;
using System.Linq;

namespace FlatMate.Web.Areas.Offers.Data
{
    public class MarketIndexVm : BaseViewModel
    {
        public IEnumerable<MarketJso> Markets { get; set; } = Enumerable.Empty<MarketJso>();
    }
}
