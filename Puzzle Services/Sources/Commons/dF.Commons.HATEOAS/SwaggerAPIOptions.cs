using System;
using System.Collections.Generic;
using System.Text;

namespace dF.Commons.HATEOAS
{
    public class SwaggerAPIOptions
    {
        public string ApiDocsTitle { get; set; }
        public string ApiDocsDescription { get; set; }
        public string ApiDocsContact { get; set; }
        public string LoginUrl { get; set; }
        public string BaseUrl { get; set; }
        public string ApiXmlDocumentationPath { get; set; }
    }
}
