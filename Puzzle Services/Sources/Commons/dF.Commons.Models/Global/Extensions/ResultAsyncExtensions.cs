using System;
using System.Threading.Tasks;

using dF.Commons.Models.BL;
using dF.Commons.Models.Global.Enums;

namespace dF.Commons.Models.Globals.Extensions
{
    public static class ResultAsyncExtensions
    {
        #region ToAsync
        public static Task<Result> ToAsync(this Result result)
        {
            return Task.FromResult(result);
        }

        public static Task<Result<T>> ToAsync<T>(this Result<T> result)
        {
            return Task.FromResult(result);
        }
        #endregion

        #region Async
        #region Returning Actions
        public static async Task<Result> Async(this Result result, Task<Action> action)
        {
            try
            {
                if (result.IsSuccess)
                    await action;

                return result;
            }
            catch (Exception e)
            {
                return Result.Fail(e).withInnerResult(result);
            }
        }

        public static async Task<Result<T>> Async<T>(this Result<T> result, Task<Action<T>> action)
        {
            try
            {
                if (result.IsSuccess)
                    await action;

                return result;
            }
            catch (Exception e)
            {
                return Result.Fail<T>(e).withInnerResult(result);
            }
        }
        #endregion

        #region Returning Functions
        public static async Task<Result> Async(this Result result, Func<Result, Task<Result>> func)
        {
            if (result.IsSuccess)
                return await func(result);

            return result;
        }

        public static async Task<Result<T>> Async<T>(this Result result, Func<Result, Task<Result<T>>> func)
        {
            if (result.IsSuccess)
                return await func(result);

            return result.ToTypedResult<T>();
        }

        public static async Task<Result<T>> Async<T>(this Result<T> result, Func<Result<T>, Task<Result<T>>> func)
        {
            if (result.IsSuccess)
                return await func(result);

            return result;
        }

        public static async Task<Result> Async<T>(this Result<T> result, Func<Result<T>, Task<Result>> func)
        {
            if (result.IsSuccess)
                return await func(result);

            return result;
        }

        public static async Task<Result<K>> Async<T, K>(this Result<T> result, Func<Result<T>, Task<Result<K>>> func)
        {
            if (result.IsSuccess)
                return await func(result);

            return result.ToTypedResult<K>();
        }
        #endregion
        #endregion

        #region Then » Compositions
        #region Compose From Result
        public static async Task<Result<T>> ThenAsync<T>(this Task<Result<T>> result, Func<Result<T>, Task<Result<T>>> func)
        {
            var @this = await result;

            if (@this.IsSuccess)
                return await func(@this);

            return @this;
        }
        #endregion

        #region Compose New Function
        public static async Task<Result> ThenAsync(this Task<Result> result, Func<Task<Result>> func)
        {
            var @this = await result;

            if (@this.IsSuccess)
                return await func();

            return @this;
        }

        public static async Task<Result<T>> ThenAsync<T>(this Task<Result> result, Func<Task<Result<T>>> func)
        {
            var @this = await result;

            if (@this.IsSuccess)
                return await func();

            if (@this.HasException)
                return Result.Fail<T>(@this.InnerException);
            else
                return Result.Fail<T>(@this.Error, @this.ErrorType);
        }

        public static async Task<Result<T>> ThenAsync<T>(this Task<Result<T>> result, Func<Task<Result<T>>> func)
        {
            var @this = await result;

            if (@this.IsSuccess)
                return await func();

            return @this;
        }
        #endregion
        #endregion

        #region OnSuccess
        #region Actions
        public static async Task<Result> OnSuccessAsync(this Task<Result> result, Action action)
        {
            var @this = await result;

            if (@this.IsSuccess)
                action();

            return @this;
        }

        public static async Task<Result<T>> OnSuccessAsync<T>(this Task<Result<T>> result, Action<T> action)
        {
            var @this = await result;

            if (@this.IsSuccess)
                action(@this.Value);

            return @this;
        }

        public static async Task<Result> WithSuccessDoAsync(this Task<Result> result, Action<ResultBase> action)
        {
            var @this = await result;

            if (@this.IsSuccess)
                action(@this);

            return @this;
        }

        public static async Task<Result<T>> WithSuccessDoAsync<T>(this Task<Result<T>> result, Action<Result<T>> action)
        {
            var @this = await result;

            if (@this.IsSuccess)
                action(@this);

            return @this;
        }
        #endregion

        #region Functions
        public static async Task<T> WithSuccessDoAsync<T>(this Task<Result> result, Func<ResultBase, T> func)
        {
            var @this = await result;

            if (@this.IsSuccess)
                return func(@this);

            return default(T);
        }

        public static async Task<T> WithSuccessDoAsync<T>(this Task<Result<T>> result, Func<Result<T>, T> func)
        {
            var @this = await result;

            if (@this.IsSuccess)
                return func(@this);

            return default(T);
        }
        #endregion
        #endregion

        #region OnFailure
        public static async Task<Result> OnFailureAsync(this Task<Result> result, Action<ResultBase> action)
        {
            var @this = await result;

            if (@this.IsFailure)
                action(@this);

            return @this;
        }

        public static async Task<Result<T>> OnFailureAsync<T>(this Task<Result<T>> result, Action<Result<T>> action)
        {
            var @this = await result;

            if (@this.IsFailure)
                action(@this);

            return @this;
        }

        public static async Task<Result> OnFailureAsync(this Task<Result> result, ErrorType errorType, string errorMessage, params string[] errorMessageParameters)
        {
            var @this = await result;

            if (@this.IsFailure && @this.ErrorType == errorType)
                return Result.Fail(string.Format(errorMessage, errorMessageParameters), errorType)
                    .withInnerResult(@this);

            return @this;
        }

        public static async Task<Result<T>> OnFailureAsync<T>(this Task<Result<T>> result, ErrorType errorType, string errorMessage, params string[] errorMessageParameters)
        {
            var @this = await result;

            if (@this.IsFailure && @this.ErrorType == errorType)
                return Result.Fail<T>(string.Format(errorMessage, errorMessageParameters), errorType)
                    .withInnerResult(@this);

            return @this;
        }
        #endregion

        #region Flow
        public static async Task<Result> EnsureAsync(this Task<Result> result, Func<ResultBase, bool> predicate, ErrorType errorType, string errorMessage, params string[] errorMessageParameters)
        {
            var @this = await result;

            if (@this.IsSuccess && !predicate(@this))
                return Result.Fail(string.Format(errorMessage, errorMessageParameters), errorType)
                    .withInnerResult(@this);

            return @this;
        }

        public static async Task<Result<T>> EnsureAsync<T>(this Task<Result<T>> result, Func<Result<T>, bool> predicate, ErrorType errorType, string errorMessage, params string[] errorMessageParameters)
        {
            var @this = await result;

            if (@this.IsSuccess && !predicate(@this))
                return Result.Fail<T>(string.Format(errorMessage, errorMessageParameters), errorType)
                    .withInnerResult(@this);

            return @this;
        }

        #region For Functions
        public static async Task<Result> ThenIfAsync(this Task<Result> result, bool predicate, Func<ResultBase, Task<Result>> then)
        {
            var @this = await result;

            if (@this.IsSuccess && predicate)
                return await then(@this);

            return @this;
        }

        public static async Task<Result<T>> ThenIfAsync<T>(this Task<Result<T>> result, bool predicate, Func<Result<T>, Task<Result<T>>> then)
        {
            var @this = await result;

            if (@this.IsSuccess && predicate)
                return await then(@this);

            return @this;
        }

        public static Task<Result> ThenIfAsync(this Task<Result> result, Func<bool> predicate, Func<ResultBase, Task<Result>> then)
        {
            return ThenIfAsync(result, predicate(), then);
        }

        public static Task<Result<T>> ThenIfAsync<T>(this Task<Result<T>> result, Func<bool> predicate, Func<Result<T>, Task<Result<T>>> then)
        {
            return ThenIfAsync(result, predicate(), then);
        }

        public static Task<Result> ThenIfAsync(this Task<Result> result, ResultBase predicate, Func<ResultBase, Task<Result>> then)
        {
            return ThenIfAsync(result, predicate.IsSuccess, then);
        }

        public static Task<Result<T>> ThenIfAsync<T>(this Task<Result<T>> result, ResultBase predicate, Func<Result<T>, Task<Result<T>>> then)
        {
            return ThenIfAsync(result, predicate.IsSuccess, then);
        }

        public static async Task<Result> ThenIfAsync(this Task<Result> result, Func<ResultBase, bool> predicate, Func<ResultBase, Task<Result>> then)
        {
            var @this = await result;

            if (@this.IsSuccess && predicate(@this))
                return await then(@this);

            return @this;
        }

        public static async Task<Result<T>> ThenIfAsync<T>(this Task<Result<T>> result, Func<Result<T>, bool> predicate, Func<Result<T>, Task<Result<T>>> then)
        {
            var @this = await result;

            if (@this.IsSuccess && predicate(@this))
                return await then(@this);

            return @this;
        }
        #endregion

        #region For Actions
        public static async Task<Result> ThenIfAsync(this Task<Result> result, bool predicate, Action then)
        {
            var @this = await result;

            if (predicate)
                await OnSuccessAsync(result, then);

            return @this;
        }

        public static async Task<Result<T>> ThenIfAsync<T>(this Task<Result<T>> result, bool predicate, Action<T> then)
        {
            var @this = await result;

            if (predicate)
                await OnSuccessAsync(result, then);

            return @this;
        }
        #endregion
        #endregion

        #region Transformations
        public static async Task<Result<K>> MapAsync<T, K>(this Task<Result<T>> result, Func<Result<T>, Result<K>> func)
        {
            var @this = await result;

            if (@this.IsFailure)
                return @this.HasException ? Result.Fail<K>(@this.InnerException) : Result.Fail<K>(@this.Error, @this.ErrorType);

            return Result.Ok(func(@this));
        }

        public static async Task<Result<K>> MapAsync<T, K>(this Task<Result<T>> result, Func<T, K> func)
        {
            var @this = await result;

            if (@this.IsFailure)
                return @this.HasException ? Result.Fail<K>(@this.InnerException) : Result.Fail<K>(@this.Error, @this.ErrorType);

            return Result.Ok(func(@this.Value));
        }

        public static async Task<Result<K>> ReturnAsync<T, K>(this Task<Result<T>> result, Func<Result<K>> func)
        {
            var @this = await result;

            if (@this.IsFailure)
                return @this.HasException ? Result.Fail<K>(@this.InnerException) : Result.Fail<K>(@this.Error, @this.ErrorType);

            return Result.Ok(func());
        }

        public static async Task<Result<K>> ReturnAsync<T, K>(this Task<Result<T>> result, Func<K> func)
        {
            var @this = await result;

            if (@this.IsFailure)
                return @this.HasException ? Result.Fail<K>(@this.InnerException) : Result.Fail<K>(@this.Error, @this.ErrorType);

            return Result.Ok(func());
        }
        #endregion

        #region WithResultDo
        #region Actions
        public static async Task<Result> WithResultDoAsync(this Task<Result> result, Action<ResultBase> action)
        {
            var @this = await result;

            action(@this);

            return @this;
        }

        public static async Task<Result<T>> WithResultDoAsync<T>(this Task<Result<T>> result, Action<Result<T>> action)
        {
            var @this = await result;

            action(@this);

            return @this;
        }
        #endregion

        #region Functions
        public static async Task<T> WithResultDoAsync<T>(this Task<Result> result, Func<ResultBase, T> func)
        {
            var @this = await result;

            return func(@this);
        }

        public static async Task<T> WithResultDoAsync<T>(this Task<Result<T>> result, Func<Result<T>, T> func)
        {
            var @this = await result;

            return func(@this);
        }
        #endregion        
        #endregion

        #region Conversion
        public static async Task<Result> ToResultAsync<T>(this Task<Result<T>> result)
        {
            return await result;
        }

        public static async Task<Result<T>> ToResultAsync<T>(this Task<Result> result)
        {
            return Result<T>.FromResult(await result);
        }

        public static async Task<ResponseContext> ToResponseContextAsync(this Task<Result> result)
        {
            return (ResponseContext)await result;
        }

        public static async Task<ResponseContext<T>> ToResponseContextAsync<T>(this Task<Result> result)
        {
            return (ResponseContext<T>)await result;
        }

        public static async Task<ResponseContext<T>> ToResponseContextAsync<T>(this Task<Result<T>> result)
        {
            return (ResponseContext<T>)await result;
        }
        #endregion
    }
}
