using System;

namespace dF.Commons.Models.BL.Extensions
{
    public class ResourceLinkParameters
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public bool IsRequired { get; set; }
        public object DefaultValue { get; set; }
    }
}
