using System;
using System.Threading.Tasks;

using dF.Commons.Models.BL;
using dF.Commons.Models.Global.Enums;

namespace dF.Commons.Models.Globals
{
    public class Result : ResultBase
    {
        #region Fields
        protected Result _innerResult = null;
        #endregion

        #region Properties
        public Result InnerResult
        {
            get
            {
                return _innerResult;
            }
        }
        #endregion

        #region Constructors
        protected Result() : base() { }

        protected Result(Result result) : base(result) { }

        protected Result(ResponseContext response) : base(response)
        {
            if (response.InnerResponse != null)
                _innerResult = new Result(response.InnerResponse);
        }

        protected Result(ErrorType errorType = ErrorType.Empty, string error = "", Exception e = null)
            : base(errorType, error, e) { }
        #endregion

        #region Builders
        public Result withInnerResult(Result result)
        {
            _innerResult = result;

            return this;
        }
        #endregion

        #region Static Constructors
        #region Synch
        #region Failure
        public static Result Fail(string message, ErrorType errorType = ErrorType.Empty)
        {
            return new Result(errorType, message);
        }

        public static Result<T> Fail<T>(string message, ErrorType errorType = ErrorType.Empty)
        {
            return new Result<T>(errorType, message);
        }

        public static Result Fail(Exception e)
        {
            return new Result(ErrorType.Empty, e.Message, e);
        }

        public static Result<T> Fail<T>(Exception e)
        {
            return new Result<T>(ErrorType.Empty, e.Message, e);
        }
        #endregion

        #region Success
        public static Result Ok()
        {
            return new Result();
        }

        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value);
        }
        #endregion
        #endregion

        #region Asynch
        #region Failure
        public static Task<Result> FailAsync(string message, ErrorType errorType = ErrorType.Empty)
        {
            return Task.FromResult(new Result(errorType, message));
        }

        public static Task<Result<T>> FailAsync<T>(string message, ErrorType errorType = ErrorType.Empty)
        {
            return Task.FromResult(new Result<T>(errorType, message));
        }

        public static Task<Result> FailAsync(Exception e)
        {
            return Task.FromResult(new Result(ErrorType.Empty, e.Message, e));
        }

        public static Task<Result<T>> FailAsync<T>(Exception e)
        {
            return Task.FromResult(new Result<T>(ErrorType.Empty, e.Message, e));
        }
        #endregion

        #region Success
        public static Task<Result> OkAsync()
        {
            return Task.FromResult(new Result());
        }

        public static Task<Result<T>> OkAsync<T>(T value)
        {
            return Task.FromResult(new Result<T>(value));
        }
        #endregion
        #endregion
        #endregion

        #region Operators
        public static explicit operator Result(ResponseContext response)
        {
            return new Result(response);
        }
        #endregion
    }

    public class Result<T> : Result
    {
        #region Fields
        private readonly T _value;
        #endregion

        #region Properties
        public DateTime? ExpirationTime { get; private set; } = null;

        public bool IsExpired => ExpirationTime.HasValue && ExpirationTime.Value < DateTime.UtcNow;

        public T Value
        {
            get
            {
                if (!IsSuccess || IsExpired)
                    return default(T);

                return _value;
            }
        }

        public new Result<T> InnerResult => (Result<T>)_innerResult;
        #endregion

        #region Constructors
        protected internal Result() : base() { }

        protected Result(Result result) : base(result) { }

        protected Result(ResponseContext response) : base(response) { }

        protected Result(ResponseContext<T> response) : base(response)
        {
            if (response.IsSuccess)
                _value = response.Result;
        }

        protected internal Result(ErrorType errorType = ErrorType.Empty, string error = "", Exception e = null)
            : base(errorType, error, e) { }

        protected internal Result(T value) : base()
        {
            _value = value;
        }
        #endregion

        #region Builders
        public Result<T> withInnerResult(Result<T> result)
        {
            _innerResult = result;

            return this;
        }

        public new Result<T> withInnerResult(Result result)
        {
            _innerResult = result;

            return this;
        }

        public Result<T> ExpiresIn(int minutes)
        {
            if (IsSuccess && !ExpirationTime.HasValue)
                ExpirationTime = DateTime.UtcNow.AddMinutes(minutes);

            return this;
        }

        public Result<T> ExpiresOn(DateTime expirationTime)
        {
            if (IsSuccess && !ExpirationTime.HasValue)
                ExpirationTime = expirationTime;

            return this;
        }
        #endregion

        #region Operators
        public static implicit operator T(Result<T> result)
        {
            return result.Value;
        }

        public static Result<T> FromResult(Result result)
        {
            return new Result<T>(result);
        }

        public static explicit operator Result<T>(ResponseContext response)
        {
            return new Result<T>(response);
        }

        public static explicit operator Result<T>(ResponseContext<T> response)
        {
            return new Result<T>(response);
        }
        #endregion
    }

}
