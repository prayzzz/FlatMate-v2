using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace FlatMate.Web.Mvc.Base
{
    public abstract class MvcViewComponent : ViewComponent
    {
        protected int CurrentUserId
        {
            get
            {
                var userId = HttpContext.User?.FindFirst(ClaimTypes.Sid)?.Value;
                return userId == null ? 0 : Convert.ToInt32(userId);
            }
        }
    }
}