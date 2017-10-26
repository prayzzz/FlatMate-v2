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

        public IMapper Mapper => _services.Mapper;

        public IMetricsRoot MetricsRoot => _services.MetricsRoot;

        protected int CurrentUserId
        {
            get
            {
                var userId = User?.FindFirst(ClaimTypes.Sid)?.Value;
                return userId == null ? 0 : Convert.ToInt32(userId);
            }
        }

        protected Result<TResult> FromTuple<TData, TResult>((Result Result, TData Value) tuple, Func<TData, TResult> map)
        {
            return FromTuple(tuple.Result, map(tuple.Value));
        }

        protected Result<TResult> FromTuple<TResult>((Result Result, TResult Value) tuple)
        {
            return FromTuple(tuple.Result, tuple.Value);
        }

        protected T Map<T>(object data) where T : class
        {
            return Mapper.Map<T>(data, MappingContext);
        }

        private Result<TResult> FromTuple<TResult>(Result result, TResult value)
        {
            if (result.IsError)
            {
                return new ErrorResult<TResult>(result);
            }

            return new SuccessResult<TResult>(value);
        }
    }
}
