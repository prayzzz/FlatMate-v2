using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<RequestResultService> _logger;

        public RequestResultService(ILogger<RequestResultService> logger)
        {
            _logger = logger;
        }

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

        private IActionResult GetErrorResult(Result result)
        {
            switch (result.ErrorType)
            {
                case ErrorType.Unknown:
                    return new ObjectResult(new ErrorResult(ErrorType.Unknown, "Unbekannter Fehler.")) { StatusCode = 500 };
                case ErrorType.InternalError:
                    return new ObjectResult(new ErrorResult(ErrorType.InternalError, result.ToMessageString())) { StatusCode = 500 };
                case ErrorType.NotFound:
                    return new NotFoundObjectResult(new ErrorResult(ErrorType.NotFound, result.ToMessageString()));
                case ErrorType.ValidationError:
                    return new BadRequestObjectResult(new ErrorResult(ErrorType.ValidationError, result.ToMessageString()));
                case ErrorType.Unauthorized:
                    return new ObjectResult(new ErrorResult(ErrorType.Unauthorized, result.ToMessageString())) { StatusCode = 401 };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}