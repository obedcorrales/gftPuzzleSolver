using System;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;

namespace dF.Commons.Proxies.DirtyPropertiesTrackerProxy
{
    [Serializable]
    internal class DirtyPropertiesTrackerInterceptorSelector : IInterceptorSelector
    {
        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            if (IsSetter(method))
                return interceptors;
            return interceptors.Where(i => !(i is DirtyPropertiesTrackerInterceptor)).ToArray();
        }

        private bool IsSetter(MethodInfo method)
        {
            if (method.IsSpecialName && method.Name.StartsWith("set_", StringComparison.OrdinalIgnoreCase))
            {
                if (method.DeclaringType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                                        .FirstOrDefault(p => p.Name == method.Name.Substring(4))
                                        .PropertyType.Name == "ICollection`1")
                    return false;
                return true;
            }
            else
                return false;
        }
    }
}
