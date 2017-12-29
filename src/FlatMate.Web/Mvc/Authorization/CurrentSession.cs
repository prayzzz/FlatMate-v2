using System;
using System.Security.Claims;
using FlatMate.Module.Account.Shared.Interfaces;
using Microsoft.AspNetCore.Http;
using prayzzz.Common.Attributes;

namespace FlatMate.Web.Mvc.Authorization
{
    [Inject]
    public class CurrentSession : ICurrentSession
    {
        public CurrentSession(IHttpContextAccessor httpContextAccessor)
        {
            var httpContext = httpContextAccessor.HttpContext;

            var userClaim = httpContext?.User?.FindFirst(ClaimTypes.Sid);
            if (userClaim != null)
            {
                CurrentUserId = Convert.ToInt32(userClaim.Value);
            }
        }

        public int? CurrentUserId { get; }
    }
}