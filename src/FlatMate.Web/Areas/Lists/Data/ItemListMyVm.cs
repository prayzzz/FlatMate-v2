using System.Collections.Generic;
using System.Linq;
using FlatMate.Module.Lists.Api.Jso;
using FlatMate.Web.Mvc.Base;

namespace FlatMate.Web.Areas.Lists.Data
{
    public class ItemListMyVm : MvcViewModel
    {
        public IEnumerable<ItemListJso> Favorites { get; set; } = Enumerable.Empty<ItemListJso>();

        public IEnumerable<ItemListJso> MyLists { get; set; } = Enumerable.Empty<ItemListJso>();
    }
}