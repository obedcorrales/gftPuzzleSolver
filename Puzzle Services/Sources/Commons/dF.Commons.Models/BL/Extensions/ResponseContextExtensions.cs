using System;

using dF.Commons.Models.Global.Enums;
using dF.Commons.Models.Globals;

namespace dF.Commons.Models.BL.Extensions
{
    public static class ResponseContextExtensions
    {
        #region Then » Compositions
        #region Compose From Response
        public static ResponseContext<T> Then<T>(this ResponseContext<T> response, Func<ResponseContext<T>, ResponseContext<T>> func)
        {
            if (response.IsSuccess)
                return func(response);

            return response;
        }
        #endregion

        #region Compose New Function
        public static ResponseContext<T> Then<T>(this ResponseContext<T> response, Func<ResponseContext<T>> func)
        {
            if (response.IsSuccess)
                return func();

            return response;
        }
        #endregion
        #endregion

        #region OnSuccess
        public static ResponseContext<T> OnSuccess<T>(this ResponseContext<T> response, Action<T> action)
        {
            if (response.IsSuccess)
                action(response.Result);

            return response;
        }
        #endregion

        #region OnFailure
        public static ResponseContext OnFailure(this ResponseContext response, Action<ResultBase> action)
        {
            /// this will always be true since you cannot form a successful ResponseContext without a value or result, 
            /// thus all ResponseContexts without a T are Failures
            //if (response.IsFailure)
            action(response);

            return response;
        }

        public static ResponseContext<T> OnFailure<T>(this ResponseContext<T> response, Action<ResponseContext<T>> action)
        {
            if (response.IsFailure)
                action(response);

            return response;
        }

        public static ResponseContext OnFailure(this ResponseContext response, ErrorType errorType, string errorMessage, params string[] errorMessageParameters)
        {
            //if (response.IsFailure && response.ErrorType == errorType)
            if (response.ErrorType == errorType)
                return ResponseContext.Fail(string.Format(errorMessage, errorMessageParameters), errorType)
                    .withInnerResponse(response);

            return response;
        }

        public static ResponseContext<T> OnFailure<T>(this ResponseContext<T> response, ErrorType errorType, string errorMessage, params string[] errorMessageParameters)
        {
            if (response.IsFailure && response.ErrorType == errorType)
                return ResponseContext.Fail<T>(string.Format(errorMessage, errorMessageParameters), errorType)
                    .withInnerResponse(response);

            return response;
        }
        #endregion

        #region Flow
        public static ResponseContext<T> Ensure<T>(this ResponseContext<T> response, Func<ResponseContext<T>, bool> predicate, ErrorType errorType, string errorMessage, params string[] errorMessageParameters)
        {
            if (response.IsSuccess && !predicate(response))
                return ResponseContext.Fail<T>(string.Format(errorMessage, errorMessageParameters), errorType);

            return response;
        }

        #region For Functions
        public static ResponseContext<T> ThenIf<T>(this ResponseContext<T> response, bool predicate, Func<ResponseContext<T>, ResponseContext<T>> then)
        {
            if (response.IsSuccess && predicate)
                return then(response);

            return response;
        }

        public static ResponseContext<T> ThenIf<T>(this ResponseContext<T> response, Func<bool> predicate, Func<ResponseContext<T>, ResponseContext<T>> then)
        {
            return ThenIf(response, predicate(), then);
        }

        public static ResponseContext<T> ThenIf<T>(this ResponseContext<T> response, ResultBase predicate, Func<ResponseContext<T>, ResponseContext<T>> then)
        {
            return ThenIf(response, predicate.IsSuccess, then);
        }

        public static ResponseContext<T> ThenIf<T>(this ResponseContext<T> response, Func<ResponseContext<T>, bool> predicate, Func<ResponseContext<T>, ResponseContext<T>> then)
        {
            if (response.IsSuccess && predicate(response))
                return then(response);

            return response;
        }
        #endregion

        #region For Actions
        public static ResponseContext<T> ThenIf<T>(this ResponseContext<T> response, bool predicate, Action then)
        {
            if (response.IsSuccess && predicate)
                then();

            return response;
        }
        #endregion
        #endregion

        #region Transformations
        public static ResponseContext<K> Map<T, K>(this ResponseContext<T> response, Func<ResponseContext<T>, ResponseContext<K>> func)
        {
            if (response.IsFailure)
                return response.HasException ? ResponseContext.Fail<K>(response.InnerException) : ResponseContext.Fail<K>(response.Error, response.ErrorType);

            return ResponseContext.Ok(func(response));
        }

        public static ResponseContext<K> Map<T, K>(this ResponseContext<T> response, Func<T, K> func)
        {
            if (response.IsFailure)
                return response.HasException ? ResponseContext.Fail<K>(response.InnerException) : ResponseContext.Fail<K>(response.Error, response.ErrorType);

            return ResponseContext.Ok(func(response.Result));
        }

        public static ResponseContext<K> Return<T, K>(this ResponseContext<T> result, Func<ResponseContext<K>> func)
        {
            if (result.IsFailure)
                return result.HasException ? ResponseContext.Fail<K>(result.InnerException) : ResponseContext.Fail<K>(result.Error, result.ErrorType);

            return ResponseContext.Ok(func());
        }

        public static ResponseContext<K> Return<T, K>(this ResponseContext<T> result, Func<K> func)
        {
            if (result.IsFailure)
                return result.HasException ? ResponseContext.Fail<K>(result.InnerException) : ResponseContext.Fail<K>(result.Error, result.ErrorType);

            return ResponseContext.Ok(func());
        }
        #endregion

        #region WithResponseDo
        #region Actions
        public static ResponseContext<T> WithResponseDo<T>(this ResponseContext<T> response, Action<ResponseContext<T>> action)
        {
            if (response.IsSuccess)
                action(response);

            return response;
        }
        #endregion

        #region Functions
        public static T WithResponseDo<T>(this ResponseContext<T> response, Func<ResponseContext<T>, T> func)
        {
            if (response.IsSuccess)
                return func(response);

            return default(T);
        }
        #endregion
        #endregion

        #region Conversion
        public static Result ToResult(this ResponseContext response)
        {
            return (Result)response;
        }

        public static Result<T> ToResult<T>(this ResponseContext response)
        {
            return (Result<T>)response;
        }

        public static Result<T> ToResult<T>(this ResponseContext<T> response)
        {
            return (Result<T>)response;
        }
        #endregion
    }
}
