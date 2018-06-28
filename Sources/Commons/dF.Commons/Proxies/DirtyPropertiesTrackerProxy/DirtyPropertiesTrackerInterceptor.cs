using System;
using System.Linq;
using Castle.DynamicProxy;

using dF.Commons.Core.Helpers;

namespace dF.Commons.Proxies.DirtyPropertiesTrackerProxy
{
    [Serializable]
    internal class DirtyPropertiesTrackerInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            try
            {
                var dirtyPropertiesTracker = invocation.InvocationTarget as IDirtyPropertiesTracker;
                var propertySetNames = dirtyPropertiesTracker.GetDirtyPropertiesNameList();

                if (invocation.Method.Name.StartsWith("set_", StringComparison.OrdinalIgnoreCase))
                {
                    if (dirtyPropertiesTracker.IsTrackingAllPropertyChanges && (propertySetNames.Count(n => n == invocation.Method.Name.Substring(4)) == 0))
                        propertySetNames.Add(invocation.Method.Name.Substring(4));
                    else if (dirtyPropertiesTracker.IsUsingDefaultsToRemoveTrackedProperties && (propertySetNames.Count(n => n == invocation.Method.Name.Substring(4)) > 0) && (TypeDefaults.IsDefaultValue(invocation.Arguments[0])))
                        propertySetNames.Remove(invocation.Method.Name.Substring(4));
                }

                invocation.Proceed();
            }
            catch (Exception)
            {
                invocation.ReturnValue = null;
            }
        }
    }

    /// - http://kozmic.net/dynamic-proxy-tutorial/
    /// - http://kozmic.net/2009/10/30/castle-dynamic-proxy-tutorial-part-xv-patterns-and-antipatterns/
}
