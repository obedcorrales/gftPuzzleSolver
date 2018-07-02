using System.Web.Http.Filters;

namespace dF.Commons.Exceptions.Filters
{
    public class dFExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var errorResponse = actionExecutedContext.Request.ErrorToHTTPResponse(actionExecutedContext.Exception);

            actionExecutedContext.Response = errorResponse;
        }
    }
}
