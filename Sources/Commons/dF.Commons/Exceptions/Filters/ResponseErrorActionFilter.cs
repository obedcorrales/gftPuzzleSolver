using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

using dF.Commons.Exceptions.Extensions;
using dF.Commons.Models.BL;

namespace dF.Commons.Exceptions.Filters
{
    public class ResponseErrorActionFilter : ActionFilterAttribute
    {
        private static string GenericResponseContextTypeName = typeof(ResponseContext<>).Name;
        private static string ResponseContextTypeName = typeof(ResponseContext).Name;

        public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);

            var oc = actionExecutedContext.Response?.Content as ObjectContent;

            if (oc?.Value != null)
            {
                if (oc.ObjectType.Name == GenericResponseContextTypeName || oc.ObjectType.Name == ResponseContextTypeName)
                {
                    var rContext = oc.Value as ResponseContext;

                    rContext.OnFailureThrowException();
                }
            }

            return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }
    }
}
