using FlatMate.Module.Account.DataAccess.Users;
using FlatMate.Module.Account.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using prayzzz.Common.Attributes;
using prayzzz.Common.Enums;

namespace FlatMate.Web.Mvc.Authorization
{
    [Inject(DependencyLifetime.Scoped, typeof(AreaRestrictionFilter))]
    public class AreaRestrictionFilter : IAuthorizationFilter
    {
        private readonly IUserRestrictedAreaRepository _areaRepository;
        private readonly ICurrentSession _session;

        public AreaRestrictionFilter(ICurrentSession session, IUserRestrictedAreaRepository areaRepository)
        {
            _session = session;
            _areaRepository = areaRepository;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (_session.CurrentUserId.HasValue && context.RouteData.Values.TryGetValue("area", out var area))
            {
                var result = _areaRepository.GetRestrictedAreas(_session.CurrentUserId.Value);
                if (result.Contains(area.ToString().ToLower()))
                {
                    context.Result = new UnauthorizedResult();
                }
            }
        }
    }
}