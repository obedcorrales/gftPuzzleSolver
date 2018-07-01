using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.Application;
using System.Web.Http;

using dF.Commons.Helpers;

namespace dF.Commons.HATEOAS
{
    public static class HATEOASConfig
    {
        public static void JsonFormatter(this HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // https://code.msdn.microsoft.com/loop-reference-handling-in-caaffaf7
            // http://blogs.msdn.com/b/hongyes/archive/2012/09/04/loop-reference-handling-in-serializer.aspx

            #region Loop Reference Handling » Option #1
            //config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            #endregion

            #region Loop Reference Handling » Option #2
            //config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.All;
            #endregion

            #region Loop Reference Handling » Option #3
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            #endregion
        }

        public static void AddSwaggerApiDocumentation(this HttpConfiguration config, SwaggerAPIOptions options)
        {
            config
                .EnableSwagger(c => {

                    c.SingleApiVersion("v1", options.ApiDocsTitle)
                        .Description(options.ApiDocsDescription);

                    if (!string.IsNullOrWhiteSpace(options.ApiXmlDocumentationPath))
                        c.IncludeXmlComments(options.ApiXmlDocumentationPath);

                    c.RootUrl(req => {
                        var host = req.RequestUri.Host;

                        return UriHelpers.MapHostWildcardTo(options.BaseUrl, host).TrimEnd('/');
                    });
                })
                .EnableSwaggerUi();
        }
    }
}
