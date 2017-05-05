using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Attributes;
using prayzzz.Common.Enums;
using prayzzz.Common.Results;

namespace FlatMate.Api.Services
{
    public interface IRequestResultService
    {
        /// <summary>
        ///     Creates a new <see cref="IActionResult" /> from the given <see cref="Result" />
        /// </summary>
        IActionResult Get(Result result, string httpMethod);

        /// <summary>
        ///     Creates a new <see cref="IActionResult" /> from the given <see cref="Result" />
        /// </summary>
        IActionResult Get<T>(IResult<T> result, string httpMethod);
    }

    /// <summary>
    ///     This service creates <see cref="IActionResult" /> from <see cref="Result" /> 
    ///     by converting the <see cref="ErrorType" /> into a http statuscode
    /// </summary>
    [Inject(DependencyLifetime.Singleton)]
    public class RequestResultService : IRequestResultService
    {
        public IActionResult Get(Result result, string httpMethod)
        {
            if (result.IsError)
            {
                return GetErrorResult(result);
            }

            return HttpMethods.IsPost(httpMethod) ? new StatusCodeResult(201) : new OkResult();
        }

        public IActionResult Get<T>(IResult<T> result, string httpMethod)
        {
            if (result.IsError)
            {
                return GetErrorResult(result as Result);
            }

            return HttpMethods.IsPost(httpMethod) ? new ObjectResult(result.Data) { StatusCode = 201 } : new OkObjectResult(result.Data);
        }

        private static IActionResult GetErrorResult(Result result)
        {
            switch (result.ErrorType)
            {
                case ErrorType.None:
                case ErrorType.Unknown:
                case ErrorType.InternalError:
                    return new ObjectResult(new Dictionary<string, string> { { "error", result.ToMessageString() } }) { StatusCode = 500 };
                case ErrorType.NotFound:
                    return new NotFoundObjectResult(new Dictionary<string, string> { { "error", result.ToMessageString() } });
                case ErrorType.ValidationError:
                    return new BadRequestObjectResult(new Dictionary<string, string> { { "error", result.ToMessageString() } });
                case ErrorType.Unauthorized:
                    return new ObjectResult(new Dictionary<string, string> { { "error", result.ToMessageString() } }) { StatusCode = 401 };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}