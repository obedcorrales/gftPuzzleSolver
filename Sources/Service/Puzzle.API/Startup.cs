using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using System.Web.Http;
using System.Web.Http.Batch;

using dF.Commons.Exceptions.Filters;
using dF.Commons.HATEOAS;
using dF.Commons.HATEOAS.Filters;

[assembly: OwinStartup(typeof(Puzzle.API.Startup))]

namespace Puzzle.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            #region Add CORS
            appBuilder.UseCors(CorsOptions.AllowAll);
            #endregion

            #region Add WebAPI
            var config = new HttpConfiguration();

            #region Add Routes
            config.MapHttpAttributeRoutes();

            var server = new HttpServer(config);

            config.Routes.MapHttpBatchRoute(
                routeName: "batch",
                routeTemplate: "api/batch",
                batchHandler: new DefaultHttpBatchHandler(server)
            );

            config.Routes.MapHttpBatchRoute(
                routeName: "asyncBatch",
                routeTemplate: "api/asyncBatch",
                batchHandler: new DefaultHttpBatchHandler(server) { ExecutionOrder = BatchExecutionOrder.NonSequential }
            );

            config.Routes.MapHttpRoute(
                name: "PuzzleServicesApi",
                routeTemplate: "PuzzleServices/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            #endregion

            #region Set Up HATEOAS
            HATEOASConfig.AddSwaggerApiDocumentation(config,
                new SwaggerAPIOptions
                {
                    ApiDocsTitle = "Puzzle Service",
                    ApiDocsDescription = "Swagger Documentation for Puzzle Service",
                    ApiDocsContact = "",
                    BaseUrl = API.Configuration.ServicesURL,
                });
            HATEOASConfig.JsonFormatter(config);
            #endregion

            #region Add filters to pipeline
            config.Filters.Add(new dFExceptionFilter());
            config.Filters.Add(new HateoasActionFilter());
            config.Filters.Add(new ResponseErrorActionFilter());
            #endregion

            config.EnsureInitialized();

            appBuilder.UseWebApi(config);
            #endregion
        }
    }
}
