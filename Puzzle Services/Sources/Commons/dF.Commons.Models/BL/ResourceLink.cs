using System.Collections.Generic;

using dF.Commons.Models.BL.Enums;
using dF.Commons.Models.BL.Extensions;

namespace dF.Commons.Models.BL
{
    public class ResourceLink
    {
        public string ResourceID { get; set; }
        public string MethodName { get; set; }
        public CRUDverbs Verb { get; set; }
        public List<ResourceLinkParameters> Parameters { get; set; }
    }
}
