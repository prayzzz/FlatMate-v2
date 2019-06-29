using System.Collections.Generic;
using System.Linq;
using FlatMate.Module.Lists.Api.Jso;
using FlatMate.Web.Mvc.Base;

namespace FlatMate.Web.Areas.Lists.Data
{
    public class ItemListBrowseVm : MvcViewModel
    {
        public IEnumerable<ItemListJso> Favorites { get; set; }

        public IEnumerable<ItemListJso> Lists { get; set; } = Enumerable.Empty<ItemListJso>();
    }
}