using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using dF.Commons.Core.Exceptions;

namespace dF.Commons.Exceptions
{
    public static class ExceptionHandler
    {
        const string ArgumentExceptionReasonPhrase = "InvalidArgument";
        const string EntityCreatedWithErrorsExceptionReasonPhrase = "EntityCreatedWithErrors";
        const string EntityIDHeader = "EntityID";


        public static HttpResponseMessage ErrorToHTTPResponse(this HttpRequestMessage request, Exception e)
        {
            var ex = ExtractInnermostException(e);

            if (ex is UnauthorizedAccessException)
                return request.CreateErrorResponse(HttpStatusCode.Unauthorized, ExtractExceptionMessages(e));
            if (ex is ArgumentException)
            {
                var response = request.CreateErrorResponse(HttpStatusCode.BadRequest, ExtractExceptionMessages(e));
                response.ReasonPhrase = ArgumentExceptionReasonPhrase;
                return response;
            }
            if (ex is DuplicateEntryException)
                return request.CreateErrorResponse(HttpStatusCode.Conflict, ExtractExceptionMessages(e));
            if (ex is EntityNotFoundException)
                return request.CreateErrorResponse(HttpStatusCode.NotFound, ExtractExceptionMessages(e));
            if (ex is EntityCreatedWithErrorsException)
            {
                var response = request.CreateErrorResponse(HttpStatusCode.BadRequest, ExtractExceptionMessages(e));
                response.ReasonPhrase = EntityCreatedWithErrorsExceptionReasonPhrase;
                response.Headers.Add(EntityIDHeader, (ex as EntityCreatedWithErrorsException).EntityID);
                return response;
            }
            if (ex is CommitFailureException)
                return request.CreateErrorResponse(HttpStatusCode.NotModified, ExtractExceptionMessages(e));
            if (ex is EntityLockedExpection)
                return request.CreateErrorResponse((HttpStatusCode)423, ExtractExceptionMessages(e));

            return request.CreateErrorResponse(HttpStatusCode.BadRequest, ExtractExceptionMessages(e), e);
        }

        public static string ExtractExceptionMessages(Exception e)
        {
            var errorMessages = e.Message;

            if (e.InnerException != null)
            {
                var innerErrorMessage = ExtractExceptionMessages(e.InnerException);

                if (!string.IsNullOrWhiteSpace(innerErrorMessage))
                    errorMessages += "  »»  " + innerErrorMessage;
            }

            return errorMessages;
        }

        private static Exception ExtractInnermostException(Exception e)
        {
            if (e.InnerException == null)
                return e;

            var retex = ExtractInnermostException(e.InnerException);

            if ((e is UnauthorizedAccessException ||
                    e is DuplicateEntryException ||
                    e is EntityNotFoundException ||
                    e is EntityCreatedWithErrorsException ||
                    e is ArgumentException ||
                    e is CommitFailureException) &&
                !(retex is UnauthorizedAccessException ||
                    retex is DuplicateEntryException ||
                    retex is EntityNotFoundException ||
                    retex is EntityCreatedWithErrorsException ||
                    retex is ArgumentException ||
                    retex is CommitFailureException))
                retex = e;

            return retex;
        }

        public static async Task<Exception> HTTPResponseToException(this HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new UnauthorizedAccessException(await response.Content.ReadAsStringAsync());
            if (response.StatusCode == HttpStatusCode.Conflict)
                return new DuplicateEntryException(await response.Content.ReadAsStringAsync());
            if (response.StatusCode == HttpStatusCode.NotFound)
                return new EntityNotFoundException(await response.Content.ReadAsStringAsync());
            if (response.StatusCode == HttpStatusCode.BadRequest && response.ReasonPhrase == ArgumentExceptionReasonPhrase)
                return new ArgumentException(await response.Content.ReadAsStringAsync());
            if (response.StatusCode == HttpStatusCode.BadRequest && response.ReasonPhrase == EntityCreatedWithErrorsExceptionReasonPhrase)
            {
                var e = new EntityCreatedWithErrorsException(await response.Content.ReadAsStringAsync());

                IEnumerable<string> headerValues;
                response.Headers.TryGetValues(EntityIDHeader, out headerValues);
                e.EntityID = headerValues.FirstOrDefault();

                return e;
            }
            if (response.StatusCode == HttpStatusCode.NotModified)
                return new CommitFailureException(await response.Content.ReadAsStringAsync());
            if (response.StatusCode == (HttpStatusCode)423)
                return new EntityLockedExpection(await response.Content.ReadAsStringAsync());
            return new Exception(await response.Content.ReadAsStringAsync());
        }

        public static async Task<Exception> HTTPStatusToException(HttpStatusCode status, string errorMessage)
        {
            var response = new HttpResponseMessage(status);
            response.Content = new StringContent(errorMessage);

            return await HTTPResponseToException(response);
        }
    }
}
