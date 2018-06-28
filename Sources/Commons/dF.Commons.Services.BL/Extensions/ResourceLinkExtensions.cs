using System;
using System.Collections.Generic;

using dF.Commons.Models.BL;
using dF.Commons.Models.BL.Contracts;
using dF.Commons.Models.BL.Enums;
using dF.Commons.Services.BL.Helpers;

namespace dF.Commons.Services.BL.Extensions
{
    public static class ResourceLinkExtensions
    {
        public static List<ResourceLink> AddResourceMap<T>(this List<ResourceLink> resourceLinks) where T : class
        {
            resourceLinks.AddRange(ResourceLinkBuilder.BuildResourceMap<T>());

            return resourceLinks;
        }

        public static List<ResourceLink> AddResourceMap(this List<ResourceLink> resourceLinks, Type type)
        {
            resourceLinks.AddRange(ResourceLinkBuilder.BuildResourceMap(type));

            return resourceLinks;
        }

        public static LinkID BuildLinkID(this List<ResourceLink> resourceMap, string resourceId, CRUDverbs verb = CRUDverbs.Read)
        {
            return LinkID.fromResourceMap(resourceMap, resourceId, verb);
        }

        public static LinkID BuildLinkID(this IServiceResourceMap serviceResourceMap, string resourceId, CRUDverbs verb = CRUDverbs.Read)
        {
            return LinkID.fromServiceMap(serviceResourceMap, resourceId, verb);
        }
    }
}
