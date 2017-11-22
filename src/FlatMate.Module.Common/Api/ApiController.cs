using App.Metrics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;
using System;
using System.Security.Claims;

namespace FlatMate.Module.Common.Api
{
    [Authorize]
    [EnsurePostBodyFilter]
    public class ApiController : Controller
    {
        protected readonly MappingContext MappingContext;
        private readonly IApiControllerServices _services;

        public ApiController(IApiControllerServices services)
        {
            _services = services;

            MappingContext = new MappingContext();
        }

        protected IMapper Mapper => _services.Mapper;

        protected IMetricsRoot MetricsRoot => _services.MetricsRoot;

        protected int CurrentUserId
        {
            get
            {
                var userId = User?.FindFirst(ClaimTypes.Sid)?.Value;
                return userId == null ? 0 : Convert.ToInt32(userId);
            }
        }

        protected T Map<T>(object data) where T : class
        {
            return Mapper.Map<T>(data, MappingContext);
        }

        protected static (Result, TOut) MapResultTuple<TIn, TOut>((Result, TIn) result, Func<TIn, TOut> mappingFunc)
        {
            return (result.Item1, mappingFunc(result.Item2));
        }
    }
}