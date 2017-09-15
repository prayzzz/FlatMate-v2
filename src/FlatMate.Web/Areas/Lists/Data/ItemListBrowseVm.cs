using System.Collections.Generic;
using System.Linq;
using FlatMate.Web.Mvc.Base;
using FlatMate.Module.Lists.Api.Jso;

namespace FlatMate.Web.Areas.Lists.Data
{
    public class ItemListBrowseVm : BaseViewModel
    {
        public IEnumerable<ItemListJso> Favorites { get; set; }

        public IEnumerable<ItemListJso> Lists { get; set; } = Enumerable.Empty<ItemListJso>();
    }
}