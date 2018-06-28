using dF.Commons.Models.BL;
using dF.Commons.Models.Global.Enums;
using System;

namespace dF.Commons.Models.Globals.Extensions
{
    public static class ResultExtensions
    {
        #region Then » Compositions
        #region Compose From Result
        public static Result<T> Then<T>(this Result<T> result, Func<Result<T>, Result<T>> func)
        {
            if (result.IsSuccess)
                return func(result);

            return result;
        }
        #endregion

        #region Compose New Function
        public static Result Then(this Result result, Func<Result> func)
        {
            if (result.IsSuccess)
                return func();

            return result;
        }

        public static Result<T> Then<T>(this Result result, Func<Result<T>> func)
        {
            if (result.IsSuccess)
                return func();

            return result.ToTypedResult<T>();
        }

        public static Result<T> Then<T>(this Result<T> result, Func<Result<T>> func)
        {
            if (result.IsSuccess)
                return func();

            return result;
        }
        #endregion
        #endregion

        #region OnSuccess
        #region Actions
        public static Result OnSuccess(this Result result, Action action)
        {
            if (result.IsSuccess)
                action();

            return result;
        }

        public static Result<T> OnSuccess<T>(this Result<T> result, Action<T> action)
        {
            if (result.IsSuccess)
                action(result.Value);

            return result;
        }

        public static Result WithSuccessDo(this Result result, Action<ResultBase> action)
        {
            if (result.IsSuccess)
                action(result);

            return result;
        }

        public static Result<T> WithSuccessDo<T>(this Result<T> result, Action<Result<T>> action)
        {
            if (result.IsSuccess)
                action(result);

            return result;
        }
        #endregion

        #region Functions
        public static T WithSuccessDo<T>(this Result result, Func<ResultBase, T> func)
        {
            if (result.IsSuccess)
                return func(result);

            return default(T);
        }

        public static T WithSuccessDo<T>(this Result<T> result, Func<Result<T>, T> func)
        {
            if (result.IsSuccess)
                return func(result);

            return default(T);
        }
        #endregion
        #endregion

        #region OnFailure
        public static Result OnFailure(this Result result, Action<ResultBase> action)
        {
            if (result.IsFailure)
                action(result);

            return result;
        }

        public static Result<T> OnFailure<T>(this Result<T> result, Action<Result<T>> action)
        {
            if (result.IsFailure)
                action(result);

            return result;
        }

        public static Result OnFailure(this Result result, ErrorType errorType, string errorMessage, params string[] errorMessageParameters)
        {
            if (result.IsFailure && result.ErrorType == errorType)
                return Result.Fail(string.Format(errorMessage, errorMessageParameters), errorType)
                    .withInnerResult(result);

            return result;
        }

        public static Result<T> OnFailure<T>(this Result<T> result, ErrorType errorType, string errorMessage, params string[] errorMessageParameters)
        {
            if (result.IsFailure && result.ErrorType == errorType)
                return Result.Fail<T>(string.Format(errorMessage, errorMessageParameters), errorType)
                    .withInnerResult(result);

            return result;
        }
        #endregion

        #region Flow
        public static Result Ensure(this Result result, Func<ResultBase, bool> predicate, ErrorType errorType, string errorMessage, params string[] errorMessageParameters)
        {
            if (result.IsSuccess && !predicate(result))
                return Result.Fail(string.Format(errorMessage, errorMessageParameters), errorType)
                    .withInnerResult(result);

            return result;
        }

        public static Result<T> Ensure<T>(this Result<T> result, Func<Result<T>, bool> predicate, ErrorType errorType, string errorMessage, params string[] errorMessageParameters)
        {
            if (result.IsSuccess && !predicate(result))
                return Result.Fail<T>(string.Format(errorMessage, errorMessageParameters), errorType)
                    .withInnerResult(result);

            return result;
        }

        #region For Functions
        public static Result ThenIf(this Result result, bool predicate, Func<ResultBase, Result> then)
        {
            if (result.IsSuccess && predicate)
                return then(result);

            return result;
        }

        public static Result<T> ThenIf<T>(this Result<T> result, bool predicate, Func<Result<T>, Result<T>> then)
        {
            if (result.IsSuccess && predicate)
                return then(result);

            return result;
        }

        public static Result ThenIf(this Result result, Func<bool> predicate, Func<ResultBase, Result> then)
        {
            return ThenIf(result, predicate(), then);
        }

        public static Result<T> ThenIf<T>(this Result<T> result, Func<bool> predicate, Func<Result<T>, Result<T>> then)
        {
            return ThenIf(result, predicate(), then);
        }

        public static Result ThenIf(this Result result, ResultBase predicate, Func<ResultBase, Result> then)
        {
            return ThenIf(result, predicate.IsSuccess, then);
        }

        public static Result<T> ThenIf<T>(this Result<T> result, ResultBase predicate, Func<Result<T>, Result<T>> then)
        {
            return ThenIf(result, predicate.IsSuccess, then);
        }

        public static Result ThenIf(this Result result, Func<ResultBase, bool> predicate, Func<ResultBase, Result> then)
        {
            if (result.IsSuccess && predicate(result))
                return then(result);

            return result;
        }

        public static Result<T> ThenIf<T>(this Result<T> result, Func<Result<T>, bool> predicate, Func<Result<T>, Result<T>> then)
        {
            if (result.IsSuccess && predicate(result))
                return then(result);

            return result;
        }
        #endregion

        #region For Actions
        public static Result ThenIf(this Result result, bool predicate, Action then)
        {
            if (predicate)
                OnSuccess(result, then);

            return result;
        }

        public static Result<T> ThenIf<T>(this Result<T> result, bool predicate, Action then)
        {
            if (predicate)
                OnSuccess(result, then);

            return result;
        }
        #endregion
        #endregion

        #region Transformations
        public static Result<K> Map<T, K>(this Result<T> result, Func<Result<T>, Result<K>> func)
        {
            if (result.IsFailure)
                return result.HasException ? Result.Fail<K>(result.InnerException) : Result.Fail<K>(result.Error, result.ErrorType);

            return Result.Ok(func(result));
        }

        public static Result<K> Map<T, K>(this Result<T> result, Func<T, K> func)
        {
            if (result.IsFailure)
                return result.HasException ? Result.Fail<K>(result.InnerException) : Result.Fail<K>(result.Error, result.ErrorType);

            return Result.Ok(func(result.Value));
        }

        public static Result<K> Return<T, K>(this Result<T> result, Func<Result<K>> func)
        {
            if (result.IsFailure)
                return result.HasException ? Result.Fail<K>(result.InnerException) : Result.Fail<K>(result.Error, result.ErrorType);

            return Result.Ok(func());
        }

        public static Result<K> Return<T, K>(this Result<T> result, Func<K> func)
        {
            if (result.IsFailure)
                return result.HasException ? Result.Fail<K>(result.InnerException) : Result.Fail<K>(result.Error, result.ErrorType);

            return Result.Ok(func());
        }
        #endregion

        #region WithResultDo
        #region Actions
        public static Result WithResultDo(this Result result, Action<ResultBase> action)
        {
            action(result);

            return result;
        }

        public static Result<T> WithResultDo<T>(this Result<T> result, Action<Result<T>> action)
        {
            action(result);

            return result;
        }
        #endregion

        #region Functions
        public static T WithResultDo<T>(this Result result, Func<ResultBase, T> func)
        {
            return func(result);
        }

        public static T WithResultDo<T>(this Result<T> result, Func<Result<T>, T> func)
        {
            return func(result);
        }
        #endregion
        #endregion

        #region Conversion
        public static Result<T> ToTypedResult<T>(this Result result)
        {
            return Result<T>.FromResult(result).withInnerResult(result);
        }

        public static ResponseContext ToResponseContext(this Result result)
        {
            return (ResponseContext)result;
        }

        public static ResponseContext<T> ToResponseContext<T>(this Result result)
        {
            return (ResponseContext<T>)result;
        }

        public static ResponseContext<T> ToResponseContext<T>(this Result<T> result)
        {
            return (ResponseContext<T>)result;
        }
        #endregion
    }
}
