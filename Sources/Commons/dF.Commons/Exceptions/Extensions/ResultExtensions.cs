using dF.Commons.Models.Globals;
using dF.Commons.Models.Global.Enums;

namespace dF.Commons.Exceptions.Extensions
{
    public static class ResultExtensions
    {
        public static Result<T> OnFailureThrowException<T>(this Result<T> result)
        {
            if (result.IsFailure)
                throw Result.Fail(result.ExtractErrorMessages(), result.ExtractInnermostErrorType())
                                     .withInnerResult(result)
                                     .ToException();

            return result;
        }

        public static Result OnFailureThrowException(this Result result)
        {
            if (result.IsFailure)
                throw Result.Fail(result.ExtractErrorMessages(), result.ExtractInnermostErrorType())
                                     .withInnerResult(result)
                                     .ToException();

            return result;
        }

        public static string ExtractErrorMessages(this Result result)
        {
            if (result.IsSuccess)
                return string.Empty;

            var errorMessages = string.Empty;

            if (result.InnerException != null)
            {
                var innerErrorMessage = ExceptionHandler.ExtractExceptionMessages(result.InnerException);

                if (!string.IsNullOrWhiteSpace(innerErrorMessage))
                    errorMessages += "  »»  " + innerErrorMessage;
            }
            else
                errorMessages = result.Error;

            if (result.InnerResult != null)
            {
                var innerErrorMessage = ExtractErrorMessages(result.InnerResult);

                if (!string.IsNullOrWhiteSpace(innerErrorMessage))
                    errorMessages += "  |  " + innerErrorMessage;
            }

            return errorMessages;
        }

        public static ErrorType ExtractInnermostErrorType(this Result result)
        {
            if (result.IsFailure)
            {
                var errorType = result.GetErrorType();

                if (result.InnerResult == null)
                    return errorType;
                else if (result.InnerResult.IsSuccess)
                    return errorType;

                var innerErrorType = ExtractInnermostErrorType(result.InnerResult);

                switch (errorType)
                {
                    case ErrorType.DuplicateEntry:
                    case ErrorType.EntityCreatedWithErrors:
                    case ErrorType.EntityLocked:
                    case ErrorType.EntityNotFound:
                    case ErrorType.Argument:
                    case ErrorType.ArgumentNull:
                    case ErrorType.KeyNotFound:
                    case ErrorType.UnauthorizedAccess:
                    case ErrorType.CommitFailure:
                        switch (innerErrorType)
                        {
                            case ErrorType.DuplicateEntry:
                            case ErrorType.EntityCreatedWithErrors:
                            case ErrorType.EntityLocked:
                            case ErrorType.EntityNotFound:
                            case ErrorType.Argument:
                            case ErrorType.ArgumentNull:
                            case ErrorType.KeyNotFound:
                            case ErrorType.UnauthorizedAccess:
                            case ErrorType.CommitFailure:
                                break;
                            default:
                                innerErrorType = errorType;
                                break;
                        }
                        break;
                    default:
                        break;
                }

                return innerErrorType;
            }

            return ErrorType.Empty;
        }
    }
}
