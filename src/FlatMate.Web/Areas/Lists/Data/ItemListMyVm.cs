using System.Collections.Generic;
using FlatMate.Api.Areas.Lists.ItemList;
using FlatMate.Web.Mvc.Base;

namespace FlatMate.Web.Areas.Lists.Data
{
    public class ItemListMyVm : BaseViewModel
    {
        public IEnumerable<ItemListJso> Lists { get; set; }
    }
}