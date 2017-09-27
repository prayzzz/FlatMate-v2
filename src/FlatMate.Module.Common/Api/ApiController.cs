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
        private readonly IMapper _mapper;
        protected readonly MappingContext MappingContext;

        public ApiController(IMapper mapper)
        {
            _mapper = mapper;

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

        protected T Map<T>(object data) where T : class
        {
            return _mapper.Map<T>(data, MappingContext);
        }

        protected Result<TResult> FromTuple<TData, TResult>((Result Result, TData Value) tuple, Func<TData, TResult> map)
        {
            return FromTuple(tuple.Result, map(tuple.Value));
        }

        protected Result<TResult> FromTuple<TResult>((Result Result, TResult Value) tuple)
        {
            return FromTuple(tuple.Result, tuple.Value);
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