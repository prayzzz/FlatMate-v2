using System.Collections.Generic;
using System.Linq;
using FlatMate.Api.Areas.Lists.Jso;
using FlatMate.Web.Mvc.Base;

namespace FlatMate.Web.Areas.Lists.Data
{
    public class ItemListBrowseVm : BaseViewModel
    {
        public IEnumerable<ItemListJso> Lists { get; set; } = Enumerable.Empty<ItemListJso>();

        public IEnumerable<ItemListJso> Favorites { get; set; }
    }
}