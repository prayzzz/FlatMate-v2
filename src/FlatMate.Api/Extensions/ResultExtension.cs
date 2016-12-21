using System;
using prayzzz.Common.Result;

namespace FlatMate.Api.Extensions
{
    public static class ResultExtension
    {
        public static Result<TOut> WithData<TOut>(this Result result, TOut data)
        {
            if (!result.IsSuccess)
            {
                return new ErrorResult<TOut>(result);
            }

            return new SuccessResult<TOut>(data);
        }

        public static Result<TOut> WithDataAs<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> func)
        {
            if (!result.IsSuccess)
            {
                return new ErrorResult<TOut>(result);
            }

            return new SuccessResult<TOut>(func(result.Data));
        }
    }
}