using System;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Attributes;
using prayzzz.Common.Enums;
using prayzzz.Common.Result;

namespace FlatMate.Api.Services
{
    public interface IRequestResultService
    {
        /// <summary>
        ///     Creates a new <see cref="IActionResult" /> from the given <see cref="Result" />
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        IActionResult Get(Result result);

        /// <summary>
        ///     Creates a new <see cref="IActionResult" /> from the given <see cref="Result" />
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        IActionResult Get<T>(IResult<T> result);
    }

    /// <summary>
    ///     This service creates <see cref="IActionResult" /> from <see cref="Result" /> by converting the
    ///     <see cref="ErrorType" /> into a http statuscode
    /// </summary>
    [Inject(DependencyLifetime.Singleton)]
    public class RequestResultService : IRequestResultService
    {
        /// <summary>
        ///     Creates a new <see cref="IActionResult" /> from the given <see cref="Result" />
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public IActionResult Get(Result result)
        {
            if (!result.IsSuccess)
            {
                return GetErrorResult(result);
            }

            return new OkResult();
        }

        /// <summary>
        ///     Creates a new <see cref="IActionResult" /> from the given <see cref="Result" />
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public IActionResult Get<T>(IResult<T> result)
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(result.Data);
            }

            return GetErrorResult(result as Result);
        }

        private IActionResult GetErrorResult(Result result)
        {
            switch (result.ErrorType)
            {
                case ErrorType.Unknown:
                case ErrorType.InternalError:
                case ErrorType.SqlError:
                    return new StatusCodeResult(500);
                case ErrorType.NotFound:
                    return new NotFoundObjectResult(string.Format(result.ErrorMessage, result.ErrorMessageArgs));
                case ErrorType.ValidationError:
                    return new BadRequestObjectResult(string.Format(result.ErrorMessage, result.ErrorMessageArgs));
                case ErrorType.Unauthorized:
                    return new UnauthorizedResult();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}