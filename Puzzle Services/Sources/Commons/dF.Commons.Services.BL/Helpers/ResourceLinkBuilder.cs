using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using dF.Commons.Models.BL;
using dF.Commons.Models.BL.Attributes;
using dF.Commons.Models.BL.Enums;
using dF.Commons.Models.BL.Extensions;
using dF.Commons.Services.BL.Contracts;
using dF.Commons.Services.BL.Extensions;

namespace dF.Commons.Services.BL.Helpers
{
    public static class ResourceLinkBuilder
    {
        //static string _IBusinessAggregateName = typeof(IBusinessAggregate<>).Name;
        static string _IBusinessAggregateName = typeof(IBusinessAggregateReadOnly<>).Name;

        public static List<ResourceLink> BuildResourceMap<T>() where T : class
        {
            return BuildResourceMap(typeof(T));
        }

        public static List<ResourceLink> BuildResourceMap(Type type)
        {
            var resourceLinks = new List<ResourceLink>();

            var isBusinessAggregate = type.GetInterface(_IBusinessAggregateName) != null;
            string aggregateResourceName = null;
            string aggregatePluralResourceName = null;

            BindingFlags flags;
            if (isBusinessAggregate)
            {
                flags = BindingFlags.Public | BindingFlags.Instance;

                aggregateResourceName = type.GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static).FirstOrDefault(p => p.Name == "ResourceName")?.GetValue(null)?.ToString();
                aggregatePluralResourceName = type.GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static).FirstOrDefault(p => p.Name == "PluralResourceName")?.GetValue(null)?.ToString();
            }
            else
                flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            MethodInfo[] methods = type.GetMethods(flags);

            #region Search within methods
            foreach (MethodInfo method in methods)
            {
                CRUDverbs verb = CRUDverbs.Read;
                var resourceID = string.Empty;
                var methodName = string.Empty;
                var caught = false;

                var attr = method.GetCustomAttribute<ResourceIDAttribute>();

                if (attr != null)
                {
                    resourceID = attr.ResourceID;
                    verb = attr.Action;
                    methodName = method.Name;

                    caught = true;
                }

                if (!caught && isBusinessAggregate)
                {
                    resourceID = aggregateResourceName ?? type.Name;

                    switch (method.Name)
                    {
                        case "GetAllAsync":
                            resourceID = aggregatePluralResourceName ?? $"plural_{type.Name}";
                            methodName = "GetAllAsync";
                            break;
                        case "GetByIdAsync":
                            methodName = "GetByIdAsync";
                            break;
                        case "AddAsync":
                            verb = CRUDverbs.Create;
                            methodName = "AddAsync";
                            break;
                        case "UpdateAsync":
                            verb = CRUDverbs.Update;
                            methodName = "UpdateAsync";
                            break;
                        case "DeleteAsync":
                            verb = CRUDverbs.Delete;
                            methodName = "DeleteAsync";
                            break;
                        default:
                            break;
                    }

                    if (methodName != string.Empty)
                    {
                        if (resourceLinks.Count(r => r.MethodName == methodName) > 0)
                            methodName = string.Empty;
                        else
                            caught = true;
                    }
                }


                if (caught)
                {
                    if (resourceLinks.Count(r => r.ResourceID == resourceID && r.Verb == verb && r.MethodName == methodName) == 0)
                    {
                        var tmpLink = new ResourceLink
                        {
                            ResourceID = resourceID,
                            Verb = verb,
                            MethodName = methodName
                        };

                        var parameters = new List<ResourceLinkParameters>();

                        foreach (var parameter in method.GetParameters())
                        {
                            parameters.Add(new ResourceLinkParameters
                            {
                                Name = parameter.Name,
                                Type = parameter.ParameterType,
                                IsRequired = !parameter.IsOptional,
                                DefaultValue = parameter.DefaultValue
                            });
                        }

                        if (parameters.Count > 0)
                            tmpLink.Parameters = parameters;

                        resourceLinks.Add(tmpLink);
                    }
                }
            }
            #endregion

            #region Search for BusinessAggregate properties
            if (isBusinessAggregate)
            {
                // NonPublic is taken out, cause if Private, then it would not be mapped to anything on the API controller either
                //var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                foreach (var property in properties)
                {
                    if (property.PropertyType.GetInterface(_IBusinessAggregateName) != null)
                        resourceLinks.AddResourceMap(property.PropertyType);
                }
            }
            #endregion

            return resourceLinks;
        }
    }
}
