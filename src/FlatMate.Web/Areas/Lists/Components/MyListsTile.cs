using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Lists.Api;
using FlatMate.Web.Mvc;
using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Mvc;

namespace FlatMate.Web.Areas.Lists.Components
{
    [ViewComponent(Name = ComponentName)]
    public class MyListsTile : MvcViewComponent, IDashboardTile
    {
        private const string ComponentName = "MyListsTile";
        private readonly ItemListApiController _itemListApi;

        public MyListsTile(ItemListApiController itemListApi)
        {
            _itemListApi = itemListApi;
        }

        public string Name => ComponentName;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var allLists = await _itemListApi.GetAllLists(new GetAllListsQuery { OwnerId = CurrentUserId });

            var model = new MyListsTileVm();
            model.Lists = allLists.Select(x => x.Name).ToList();

            return View(model);
        }
    }

    public class MyListsTileVm : MvcViewModel
    {
        public List<string> Lists { get; set; }
    }
}