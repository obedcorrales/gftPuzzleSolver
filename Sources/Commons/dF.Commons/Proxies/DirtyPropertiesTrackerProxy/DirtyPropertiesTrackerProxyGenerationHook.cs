using System;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;

namespace dF.Commons.Proxies.DirtyPropertiesTrackerProxy
{
    [Serializable]
    internal class DirtyPropertiesTrackerProxyGenerationHook : IProxyGenerationHook
    {
        public void MethodsInspected()
        {
        }

        public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
        {
        }

        public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
        {
            return methodInfo.IsSpecialName && (IsSetterName(methodInfo.Name) || IsGetterName(methodInfo.Name)) && !IsCollection(methodInfo);
        }

        private bool IsGetterName(string name)
        {
            return name.StartsWith("get_", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsSetterName(string name)
        {
            return name.StartsWith("set_", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsCollection(MethodInfo methodInfo)
        {
            return methodInfo.DeclaringType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                        .FirstOrDefault(p => p.Name == methodInfo.Name.Substring(4))
                        .PropertyType.Name == "ICollection`1";
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
