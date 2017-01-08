using FlatMate.Api.Areas.Lists.ItemList;
using FlatMate.Web.Mvc.Base;

namespace FlatMate.Web.Areas.Lists.Data
{
    public class ItemListViewVm : BaseViewModel
    {
        public ItemListJso List { get; set; }
    }
}