using System.Collections.Generic;
using FlatMate.Module.Account.Api.Jso;
using FlatMate.Web.Mvc.Base;

namespace FlatMate.Web.Areas.Home.Data
{
    public class DashboardIndexVm : MvcViewModel
    {
        public List<UserDashboardTileJso> Tiles { get; set; }
    }
}