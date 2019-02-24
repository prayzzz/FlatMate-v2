using System;
using System.Security.Claims;
using App.Metrics;
using FlatMate.Module.Common.Api.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

namespace FlatMate.Module.Common.Api
{
    [Authorize]
    [EnsurePostBodyFilter]
    public class ApiController : Controller
    {
        private readonly IApiControllerServices _services;
        protected readonly MappingContext MappingContext;

        public ApiController(IApiControllerServices services)
        {
            _services = services;

            MappingContext = new MappingContext();
        }

        protected int CurrentUserId
        {
            get
            {
                var userId = User?.FindFirst(ClaimTypes.Sid)?.Value;
                return userId == null ? 0 : Convert.ToInt32(userId);
            }
        }

        protected IMapper Mapper => _services.Mapper;

        protected IMetricsRoot MetricsRoot => _services.MetricsRoot;

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