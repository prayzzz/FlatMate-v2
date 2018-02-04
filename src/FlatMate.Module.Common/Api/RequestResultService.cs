using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Attributes;
using prayzzz.Common.Enums;
using prayzzz.Common.Results;

namespace FlatMate.Module.Common.Api
{
    public interface IRequestResultService
    {
        /// <summary>
        ///     Creates a new <see cref="IActionResult" /> from the given <see cref="Result" />
        /// </summary>
        IActionResult Get(Result result, HttpContext context);

        /// <summary>
        ///     Creates a new <see cref="IActionResult" /> from the given <see cref="Result" />
        /// </summary>
        IActionResult Get(Result result, object obj, HttpContext context);
    }

    /// <summary>
    ///     This service creates <see cref="IActionResult" /> from <see cref="Result" />
    ///     by converting the <see cref="ErrorType" /> into a http statuscode
    /// </summary>
    [Inject(DependencyLifetime.Singleton)]
    public class RequestResultService : IRequestResultService
    {
        public IActionResult Get(Result result, HttpContext context)
        {
            if (result.IsError)
            {
                return GetErrorResult(result);
            }

            if (HttpMethods.IsPost(context.Request.Method))
            {
                return new CreatedResult(string.Empty, null);
            }

            return new OkResult();
        }

        public IActionResult Get(Result result, object obj, HttpContext context)
        {
            if (result.IsError)
            {
                return GetErrorResult(result);
            }

            if (HttpMethods.IsPost(context.Request.Method))
            {
                return new CreatedResult(string.Empty, obj);
            }

            return new OkObjectResult(obj);
        }

        private static IActionResult GetErrorResult(Result result)
        {
            switch (result.ErrorType)
            {
                case ErrorType.Unknown:
                    return new ObjectResult(new Result(ErrorType.Unknown, "Unbekannter Fehler.")) { StatusCode = 500 };
                case ErrorType.InternalError:
                    return new ObjectResult(new Result(ErrorType.InternalError, result.Message, result.MessageArgs)) { StatusCode = 500 };
                case ErrorType.NotFound:
                    return new NotFoundObjectResult(new Result(ErrorType.NotFound, result.Message, result.MessageArgs));
                case ErrorType.ValidationError:
                    return new BadRequestObjectResult(new Result(ErrorType.ValidationError, result.Message, result.MessageArgs));
                case ErrorType.Unauthorized:
                    return new ObjectResult(new Result(ErrorType.Unauthorized, result.Message, result.MessageArgs)) { StatusCode = 401 };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}