using System.Threading.Tasks;
using FlatMate.Module.Account.Api;
using FlatMate.Web.Mvc;
using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Mvc;

namespace FlatMate.Web.Areas.Account.Components
{
    [ViewComponent(Name = ComponentName)]
    public class MyProfileTile : MvcViewComponent, IDashboardTile
    {
        private const string ComponentName = "MyProfileTile";
        private readonly UserApiController _userApi;

        public MyProfileTile(UserApiController userApi)
        {
            _userApi = userApi;
        }

        public string Name => ComponentName;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new MyProfileTileVm();

            var (result, user) = await _userApi.GetAsync(CurrentUserId);
            if (result.IsError)
            {
                model.Result = result;
                return View(model);
            }

            model.UserName = user.UserName;

            return View(model);
        }
    }

    public class MyProfileTileVm : MvcViewModel
    {
        public string UserName { get; set; }
    }
}