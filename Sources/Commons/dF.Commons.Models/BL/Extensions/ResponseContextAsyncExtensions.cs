using System;
using System.Threading.Tasks;

using dF.Commons.Models.Global.Enums;
using dF.Commons.Models.Globals;

namespace dF.Commons.Models.BL.Extensions
{
    public static class ResponseContextAsyncExtensions
    {
        #region ToAsync
        public static Task<ResponseContext> ToAsync(this ResponseContext response)
        {
            return Task.FromResult(response);
        }

        public static Task<ResponseContext<T>> ToAsync<T>(this ResponseContext<T> response)
        {
            return Task.FromResult(response);
        }
        #endregion

        #region Then » Compositions
        #region Compose From Result
        public static async Task<ResponseContext<T>> ThenAsync<T>(this Task<ResponseContext<T>> response, Func<ResponseContext<T>, Task<ResponseContext<T>>> func)
        {
            var @this = await response;

            if (@this.IsSuccess)
                return await func(@this);

            return @this;
        }
        #endregion

        #region Compose New Function
        public static async Task<ResponseContext<T>> ThenAsync<T>(this Task<ResponseContext<T>> response, Func<Task<ResponseContext<T>>> func)
        {
            var @this = await response;

            if (@this.IsSuccess)
                return await func();

            return @this;
        }
        #endregion
        #endregion

        #region OnSuccess
        public static async Task<ResponseContext<T>> OnSuccessAsync<T>(this Task<ResponseContext<T>> response, Action<T> action) //where T : class
        {
            var @this = await response;

            if (@this.IsSuccess)
                action(@this.Result);

            return @this;
        }
        #endregion

        #region OnFailure
        public static async Task<ResponseContext> OnFailureAsync(this Task<ResponseContext> response, Action<ResultBase> action)
        {
            var @this = await response;

            action(@this);

            return @this;
        }

        public static async Task<ResponseContext<T>> OnFailureAsync<T>(this Task<ResponseContext<T>> response, Action<ResponseContext<T>> action)
        {
            var @this = await response;

            if (@this.IsFailure)
                action(@this);

            return @this;
        }

        public static async Task<ResponseContext> OnFailureAsync(this Task<ResponseContext> response, ErrorType errorType, string errorMessage, params string[] errorMessageParameters)
        {
            var @this = await response;

            if (@this.ErrorType == errorType)
                return ResponseContext.Fail(string.Format(errorMessage, errorMessageParameters), errorType)
                    .withInnerResponse(@this);

            return @this;
        }

        public static async Task<ResponseContext<T>> OnFailureAsync<T>(this Task<ResponseContext<T>> response, ErrorType errorType, string errorMessage, params string[] errorMessageParameters)
        {
            var @this = await response;

            if (@this.IsFailure && @this.ErrorType == errorType)
                return ResponseContext.Fail<T>(string.Format(errorMessage, errorMessageParameters), errorType)
                    .withInnerResponse(@this);

            return @this;
        }
        #endregion

        #region Flow
        public static async Task<ResponseContext<T>> EnsureAsync<T>(this Task<ResponseContext<T>> response, Func<ResponseContext<T>, bool> predicate, ErrorType errorType, string errorMessage, params string[] errorMessageParameters)
        {
            var @this = await response;

            if (@this.IsSuccess && !predicate(@this))
                return ResponseContext.Fail<T>(string.Format(errorMessage, errorMessageParameters), errorType)
                    .withInnerResponse(@this);

            return @this;
        }

        #region For Functions
        public static async Task<ResponseContext<T>> ThenIfAsync<T>(this Task<ResponseContext<T>> response, bool predicate, Func<ResponseContext<T>, Task<ResponseContext<T>>> then)
        {
            var @this = await response;

            if (@this.IsSuccess && predicate)
                return await then(@this);

            return @this;
        }

        public static Task<ResponseContext<T>> ThenIfAsync<T>(this Task<ResponseContext<T>> response, Func<bool> predicate, Func<ResponseContext<T>, Task<ResponseContext<T>>> then)
        {
            return ThenIfAsync(response, predicate(), then);
        }

        public static Task<ResponseContext<T>> ThenIfAsync<T>(this Task<ResponseContext<T>> response, ResultBase predicate, Func<ResponseContext<T>, Task<ResponseContext<T>>> then)
        {
            return ThenIfAsync(response, predicate.IsSuccess, then);
        }

        public static async Task<ResponseContext<T>> ThenIfAsync<T>(this Task<ResponseContext<T>> response, Func<ResponseContext<T>, bool> predicate, Func<ResponseContext<T>, Task<ResponseContext<T>>> then)
        {
            var @this = await response;

            if (@this.IsSuccess && predicate(@this))
                return await then(@this);

            return @this;
        }
        #endregion

        #region For Actions
        public static async Task<ResponseContext<T>> ThenIfAsync<T>(this Task<ResponseContext<T>> response, bool predicate, Action then)
        {
            var @this = await response;

            if (@this.IsSuccess && predicate)
                then();

            return @this;
        }
        #endregion
        #endregion

        #region Transformations
        public static async Task<ResponseContext<K>> MapAsync<T, K>(this Task<ResponseContext<T>> response, Func<ResponseContext<T>, ResponseContext<K>> func)
        {
            var @this = await response;

            if (@this.IsFailure)
                return @this.HasException ? ResponseContext.Fail<K>(@this.InnerException) : ResponseContext.Fail<K>(@this.Error, @this.ErrorType);

            return ResponseContext.Ok(func(@this));
        }

        public static async Task<ResponseContext<K>> MapAsync<T, K>(this Task<ResponseContext<T>> response, Func<T, K> func)
        {
            var @this = await response;

            if (@this.IsFailure)
                return @this.HasException ? ResponseContext.Fail<K>(@this.InnerException) : ResponseContext.Fail<K>(@this.Error, @this.ErrorType);

            return ResponseContext.Ok(func(@this.Result));
        }

        public static async Task<ResponseContext<K>> ReturnAsync<T, K>(this Task<ResponseContext<T>> response, Func<ResponseContext<K>> func)
        {
            var @this = await response;

            if (@this.IsFailure)
                return @this.HasException ? ResponseContext.Fail<K>(@this.InnerException) : ResponseContext.Fail<K>(@this.Error, @this.ErrorType);

            return ResponseContext.Ok(func());
        }

        public static async Task<ResponseContext<K>> ReturnAsync<T, K>(this Task<ResponseContext<T>> response, Func<K> func)
        {
            var @this = await response;

            if (@this.IsFailure)
                return @this.HasException ? ResponseContext.Fail<K>(@this.InnerException) : ResponseContext.Fail<K>(@this.Error, @this.ErrorType);

            return ResponseContext.Ok(func());
        }
        #endregion

        #region WithResponseDo
        #region Actions
        public static async Task<ResponseContext<T>> WithResponseDoAsync<T>(this Task<ResponseContext<T>> response, Action<ResponseContext<T>> action)
        {
            var @this = await response;

            if (@this.IsSuccess)
                action(@this);

            return @this;
        }
        #endregion

        #region Functions
        public static async Task<T> WithResponseDoAsync<T>(this Task<ResponseContext<T>> response, Func<ResponseContext<T>, T> func)
        {
            var @this = await response;

            if (@this.IsSuccess)
                return func(@this);

            return default(T);
        }
        #endregion
        #endregion

        #region Conversion
        public static async Task<ResponseContext> ToResponseContextAsync<T>(this Task<ResponseContext<T>> response)
        {
            return await response;
        }

        public static async Task<ResponseContext<T>> ToResponseContextAsync<T>(this Task<ResponseContext> response)
        {
            return (ResponseContext<T>)await response;
        }

        public static async Task<Result> ToResultAsync(this Task<ResponseContext> response)
        {
            return (Result)await response;
        }

        public static async Task<Result<T>> ToResultAsync<T>(this Task<ResponseContext> response)
        {
            return (Result<T>)await response;
        }

        public static async Task<Result<T>> ToResultAsync<T>(this Task<ResponseContext<T>> response)
        {
            return (Result<T>)await response;
        }
        #endregion
    }
}
