using System;

using dF.Commons.Models.BL.Enums;

namespace dF.Commons.Models.BL.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ResourceIDAttribute : Attribute
    {
        public string ResourceID { get; set; }
        public CRUDverbs Action { get; set; }

        public ResourceIDAttribute(string resourceId, CRUDverbs action = CRUDverbs.Read)
        {
            ResourceID = resourceId;
            Action = action;
        }
    }
}
