using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Authentication;

using dF.Commons.Core.Exceptions;
using dF.Commons.Models.Global.Enums;
using dF.Commons.Models.Globals;

namespace dF.Commons.Exceptions.Extensions
{
    public static class ResultBaseExtensions
    {
        #region Extensions
        public static ResultBase OnFailureThrowException(this ResultBase result)
        {
            if (result.IsFailure)
                throw ToException(result);

            return result;
        }
        #endregion

        public static ErrorType GetErrorType(this ResultBase result)
        {
            if (result.HasException)
            {
                var e = result.InnerException;

                // Frequent Use
                if (e is ArgumentException)
                    return ErrorType.Argument;
                else if (e is ArgumentNullException)
                    return ErrorType.ArgumentNull;
                else if (e is NullReferenceException)
                    return ErrorType.NullReference;
                else if (e is DuplicateEntryException)
                    return ErrorType.DuplicateEntry;
                else if (e is EntityCreatedWithErrorsException)
                    return ErrorType.EntityCreatedWithErrors;
                else if (e is EntityNotFoundException)
                    return ErrorType.EntityNotFound;
                else if (e is KeyNotFoundException)
                    return ErrorType.KeyNotFound;
                else if (e is InvalidOperationException)
                    return ErrorType.InvalidOperation;
                else if (e is UnauthorizedAccessException)
                    return ErrorType.UnauthorizedAccess;
                else if (e is CommitFailureException)
                    return ErrorType.CommitFailure;

                // Medium Use
                else if (e is EntityLockedExpection)
                    return ErrorType.EntityLocked;
                else if (e is TimeoutException)
                    return ErrorType.Timeout;
                else if (e is FormatException)
                    return ErrorType.Format;
                else if (e is IndexOutOfRangeException)
                    return ErrorType.IndexOutOfRange;

                // Low Use
                else if (e is AccessViolationException)
                    return ErrorType.AccessViolation;
                else if (e is ArgumentOutOfRangeException)
                    return ErrorType.ArgumentOutOfRange;
                else if (e is ArithmeticException)
                    return ErrorType.Arithmetic;
                else if (e is ArrayTypeMismatchException)
                    return ErrorType.ArrayTypeMismatch;
                else if (e is AuthenticationException)
                    return ErrorType.Authentication;
                else if (e is DivideByZeroException)
                    return ErrorType.DivideByZero;
                else if (e is SerializationException)
                    return ErrorType.Serialization;

                // Rare Use
                else if (e is NotImplementedException)
                    return ErrorType.NotImplemented;
                else if (e is NotSupportedException)
                    return ErrorType.NotSupported;
                else if (e is OperationCanceledException)
                    return ErrorType.OperationCanceled;
                else if (e is BadImageFormatException)
                    return ErrorType.BadImageFormat;
                else if (e is OutOfMemoryException)
                    return ErrorType.OutOfMemory;
                else if (e is SecurityException)
                    return ErrorType.Security;
            }

            return result.ErrorType;
        }

        public static Exception ToException(this ResultBase result)
        {
            if (result.IsSuccess)
                throw new InvalidOperationException("Successful Result cannot be converted to Exception");

            switch (result.ErrorType)
            {
                // Frequent Use
                case ErrorType.Argument:
                    return new ArgumentException(result.Error, result.InnerException);
                case ErrorType.ArgumentNull:
                    return new ArgumentNullException(result.Error, result.InnerException);
                case ErrorType.NullReference:
                    return new NullReferenceException(result.Error, result.InnerException);
                case ErrorType.DuplicateEntry:
                    return new DuplicateEntryException(result.Error, result.InnerException);
                case ErrorType.EntityCreatedWithErrors:
                    return new EntityCreatedWithErrorsException(result.Error, result.InnerException);
                case ErrorType.EntityNotFound:
                    return new EntityNotFoundException(result.Error, result.InnerException);
                case ErrorType.KeyNotFound:
                    return new KeyNotFoundException(result.Error, result.InnerException);
                case ErrorType.InvalidOperation:
                    return new InvalidOperationException(result.Error, result.InnerException);
                case ErrorType.UnauthorizedAccess:
                    return new UnauthorizedAccessException(result.Error, result.InnerException);
                case ErrorType.CommitFailure:
                    return new CommitFailureException(result.Error, result.InnerException);

                // Medium Use
                case ErrorType.EntityLocked:
                    return new EntityLockedExpection(result.Error, result.InnerException);
                case ErrorType.Timeout:
                    return new TimeoutException(result.Error, result.InnerException);
                case ErrorType.Format:
                    return new FormatException(result.Error, result.InnerException);
                case ErrorType.IndexOutOfRange:
                    return new IndexOutOfRangeException(result.Error, result.InnerException);

                // Low Use
                case ErrorType.AccessViolation:
                    return new AccessViolationException(result.Error, result.InnerException);
                case ErrorType.ArgumentOutOfRange:
                    return new ArgumentOutOfRangeException(result.Error, result.InnerException);
                case ErrorType.Arithmetic:
                    return new ArithmeticException(result.Error, result.InnerException);
                case ErrorType.ArrayTypeMismatch:
                    return new ArrayTypeMismatchException(result.Error, result.InnerException);
                case ErrorType.Authentication:
                    return new AuthenticationException(result.Error, result.InnerException);
                case ErrorType.DivideByZero:
                    return new DivideByZeroException(result.Error, result.InnerException);
                case ErrorType.Serialization:
                    return new SerializationException(result.Error, result.InnerException);

                // Rare Use
                case ErrorType.NotImplemented:
                    return new NotImplementedException(result.Error, result.InnerException);
                case ErrorType.NotSupported:
                    return new NotSupportedException(result.Error, result.InnerException);
                case ErrorType.OperationCanceled:
                    return new OperationCanceledException(result.Error, result.InnerException);
                case ErrorType.BadImageFormat:
                    return new BadImageFormatException(result.Error, result.InnerException);
                case ErrorType.OutOfMemory:
                    return new OutOfMemoryException(result.Error, result.InnerException);
                case ErrorType.Security:
                    return new SecurityException(result.Error, result.InnerException);

                case ErrorType.Verification:
                case ErrorType.StackOverflow:
                case ErrorType.QuotaExceeded:
                case ErrorType.Communication:
                case ErrorType.Empty:
                default:
                    return new Exception(result.Error, result.InnerException);
            }
        }
    }

}
