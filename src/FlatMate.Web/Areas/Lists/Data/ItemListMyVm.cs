using System.Collections.Generic;
using System.Linq;
using FlatMate.Web.Mvc.Base;
using FlatMate.Module.Lists.Api.Jso;

namespace FlatMate.Web.Areas.Lists.Data
{
    public class ItemListMyVm : MvcViewModel
    {
        public IEnumerable<ItemListJso> Favorites { get; set; } = Enumerable.Empty<ItemListJso>();

        public IEnumerable<ItemListJso> MyLists { get; set; } = Enumerable.Empty<ItemListJso>();
    }
}