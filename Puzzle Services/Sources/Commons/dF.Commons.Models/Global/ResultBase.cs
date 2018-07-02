using System;

using dF.Commons.Models.Global.Enums;

namespace dF.Commons.Models.Globals
{
    public abstract class ResultBase
    {
        #region Properties
        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;
        public string Error { get; private set; }
        public ErrorType ErrorType { get; private set; }

        public bool HasException { get; private set; }
        public Exception InnerException { get; private set; }
        #endregion

        #region Constructors
        protected ResultBase(ErrorType errorType = ErrorType.Empty, string error = "", Exception e = null)
        {
            IsSuccess = false;
            ErrorType = errorType;
            Error = error;
            InnerException = e;
            if (e != null)
                HasException = true;
        }

        protected ResultBase(ResultBase result)
        {
            IsSuccess = result.IsSuccess;
            Error = result.Error;
            ErrorType = result.ErrorType;
            HasException = result.HasException;
            InnerException = result.InnerException;
        }

        protected ResultBase()
        {
            IsSuccess = true;
        }
        #endregion
    }
}
