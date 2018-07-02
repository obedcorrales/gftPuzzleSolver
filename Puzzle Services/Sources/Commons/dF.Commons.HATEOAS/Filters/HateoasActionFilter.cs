using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

using dF.Commons.HATEOAS.Extensions;
using dF.Commons.Models.BL;
using dF.Commons.Models.Global.Constants;
using dF.Commons.Models.Service;
using dF.Commons.Models.Service.Enums;

namespace dF.Commons.HATEOAS.Filters
{
    public class HateoasActionFilter : ActionFilterAttribute
    {
        private static string GenericResponseContextTypeName = typeof(ResponseContext<>).Name;
        private static string ResponseContextTypeName = typeof(ResponseContext).Name;
        private static string GenericAPIResultTypeName = typeof(APIResult<>).Name;
        private static string APIResultTypeName = typeof(APIResult).Name;

        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            actionContext.Request.SetIsHATEOASRequest();
            actionContext.Request.SetRequestFlags();

            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }

        public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);

            if (actionExecutedContext.Exception == null)
            {
                object returnVal = null;
                var oc = actionExecutedContext.Response?.Content as ObjectContent;
                if (oc?.Value != null)
                {
                    if (actionExecutedContext.Request.IsHATEOASRequest())
                    {
                        var ocTypeName = oc.Value.GetType().Name;

                        if (ocTypeName != APIResultTypeName && ocTypeName != GenericAPIResultTypeName)
                        {
                            returnVal = oc.Value;

                            var selfUri = actionExecutedContext.Response.Headers.Location;

                            if (selfUri == null)
                                selfUri = actionExecutedContext.Request.RequestUri;

                            APIResult result = null;

                            if (oc.ObjectType.Name == GenericResponseContextTypeName || oc.ObjectType.Name == ResponseContextTypeName)
                            {
                                var rContext = oc.Value as ResponseContext;

                                if (rContext.IsSuccess)
                                {
                                    result = new APIResult(rContext.Result, selfUri);

                                    if (rContext.RecordCount.HasValue)
                                        result.RecordCount = rContext.RecordCount.Value;

                                    if (rContext.TotalCount.HasValue)
                                        result.TotalCount = rContext.TotalCount.Value;

                                    object tmpObject;
                                    var IsPagable = actionExecutedContext.ActionContext.ActionArguments.TryGetValue("page", out tmpObject);

                                    var LinksReturnFlag = actionExecutedContext.ActionContext.Request.GetRequestFlags().LinksReturnFlag;
                                    var ShouldReturnLinkIdsOnly = LinksReturnFlag == LinksReturnFlag.OnlyLinkID;
                                    var ShouldPreferLinkIds = LinksReturnFlag == LinksReturnFlag.PreferLinkID;

                                    //HateoasContext hateoasContext = null;
                                    bool IsThereHateoasContext = false;
                                    bool HasPaging = false;

                                    if (rContext.Links.Count > 0 || IsPagable)
                                    {
                                        //hateoasContext = actionExecutedContext.ActionContext.RequestContext.Configuration.GetHateoasContext();

                                        //IsThereHateoasContext = hateoasContext != null;
                                        HasPaging = false;

                                        foreach (var link in rContext.Links)
                                        {
                                            //if (link.Href != null && !ShouldReturnLinkIdsOnly && !(ShouldPreferLinkIds && link.LinkId != null))
                                            //    result.Links.Add(link.MakeRelativeLink(selfUri));
                                            //else if (link.LinkId != null)
                                            if (link.LinkId != null)
                                            {
                                                if (IsThereHateoasContext && !ShouldReturnLinkIdsOnly && !ShouldPreferLinkIds)
                                                {
                                                    //result.Links.Add(hateoasContext.SwaggerSpecs.BuildUriFromSpecs(link).MakeRelativeLink(selfUri));

                                                    if (IsPagable && !HasPaging && link.Rel == LinkRelations.Collections.Rel && link.Action != LinkRelations.Collections.Actions.Search)
                                                        HasPaging = true;
                                                }
                                                else if (LinksReturnFlag != LinksReturnFlag.OnlyHref)
                                                {
                                                    if (link.LinkId != null)
                                                        link.LinkId.ClearResourceSpecs();

                                                    result.Links.Add(link);
                                                }
                                            }
                                        }
                                    }

                                    if (IsPagable && !HasPaging && IsThereHateoasContext)
                                    {
                                        //var pathId = actionExecutedContext.ActionContext.ActionDescriptor.GetCustomAttributes<PathIDAttribute>().FirstOrDefault()?.PathId;

                                        //if (!string.IsNullOrWhiteSpace(pathId))
                                        //{
                                        //    string page = tmpObject.ToString();
                                        //    string pageSize = string.Empty;
                                        //    string active = string.Empty;

                                        //    if (actionExecutedContext.ActionContext.ActionArguments.TryGetValue("pageSize", out tmpObject))
                                        //        pageSize = tmpObject.ToString();

                                        //    if (actionExecutedContext.ActionContext.ActionArguments.TryGetValue("active", out tmpObject))
                                        //        active = tmpObject.ToString();
                                        //    else if (actionExecutedContext.ActionContext.RequestContext.RouteData.Route.RouteTemplate.EndsWith("/All", StringComparison.OrdinalIgnoreCase))
                                        //        active = ActiveStatusEnums.All.ToString();
                                        //    else if (actionExecutedContext.ActionContext.RequestContext.RouteData.Route.RouteTemplate.EndsWith("/Inactive", StringComparison.OrdinalIgnoreCase))
                                        //        active = ActiveStatusEnums.Inactive.ToString();

                                        //    var links = hateoasContext.BuildPagingUrisFromSpecs(pathId, page, pageSize, active, ShouldReturnLinkIdsOnly);

                                        //    if (!ShouldReturnLinkIdsOnly)
                                        //    {
                                        //        foreach (var link in links)
                                        //        {
                                        //            result.Links.Add(link.MakeRelativeLink(selfUri));
                                        //        }
                                        //    }
                                        //}
                                    }
                                }
                            }
                            else
                                result = new APIResult(oc.Value, selfUri);

                            oc.Value = result;
                            actionExecutedContext.Response.Content = oc;
                        }
                    }
                    else if (oc.ObjectType.Name == GenericResponseContextTypeName || oc.ObjectType.Name == ResponseContextTypeName)
                    {
                        var rContext = oc.Value as ResponseContext;

                        if (rContext.IsSuccess)
                        {
                            if (oc.Formatter == actionExecutedContext.ActionContext.RequestContext.Configuration.Formatters.XmlFormatter)
                                actionExecutedContext.Response.Content = new ObjectContent(rContext.Result.GetType(), rContext.Result, oc.Formatter);
                            else
                            {
                                oc.Value = rContext.Result;
                                actionExecutedContext.Response.Content = oc;
                            }
                        }
                    }
                }
                else if (actionExecutedContext.Response != null)
                    actionExecutedContext.Response.StatusCode = HttpStatusCode.NotFound;
            }

            return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }
    }
}
