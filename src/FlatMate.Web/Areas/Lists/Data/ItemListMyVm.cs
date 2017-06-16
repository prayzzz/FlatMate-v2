using System.Collections.Generic;
using System.Linq;
using FlatMate.Api.Areas.Lists.Jso;
using FlatMate.Web.Mvc.Base;

namespace FlatMate.Web.Areas.Lists.Data
{
    public class ItemListMyVm : BaseViewModel
    {
        public IEnumerable<ItemListJso> Favorites { get; set; } = Enumerable.Empty<ItemListJso>();

        public IEnumerable<ItemListJso> MyLists { get; set; } = Enumerable.Empty<ItemListJso>();
    }
}