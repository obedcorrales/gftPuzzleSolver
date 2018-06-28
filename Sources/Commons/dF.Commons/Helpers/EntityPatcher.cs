using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using dF.Commons.Core.Helpers;
using dF.Commons.Models.Global.Enums;
using dF.Commons.Models.Globals;
using dF.Commons.Proxies.DirtyPropertiesTrackerProxy;

namespace dF.Commons.Helpers
{
    public static class EntityPatcher<T>
    {
        //private static string DirtyPropertiesInterfaceName = typeof(IDirtyPropertiesTracker).Name;

        public static Result<T> Patch(T entityPatch, T originalEntity)
        {
            var shouldPatch = false;
            // TODO: Use FasterFlect or MetaProgramming as suggested in http://stackoverflow.com/questions/3377576/if-reflection-is-inefficient-when-is-it-most-appropriate
            T clonedEntity = originalEntity;
            //T clonedEntity = ShallowCloning<T>.Clone(originalEntity);

            if (entityPatch is IDirtyPropertiesTracker)
            {
                var patchType = entityPatch.GetType();

                var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
                var patchProperties = patchType.GetProperties(flags);
                MethodInfo[] patchMethods = null;
                var originalProperties = clonedEntity.GetType().GetProperties(flags);
                var usePropertyToGetValue = true;

                foreach (var propertyName in (entityPatch as IDirtyPropertiesTracker).GetDirtyPropertiesNameList())
                {
                    var patchProperty = patchProperties.FirstOrDefault(p => p.Name == propertyName);
                    var originalProperty = originalProperties.FirstOrDefault(p => p.Name == propertyName);

                    if (patchProperty != null)
                    {
                        var ShouldOverwrite = false;
                        object patchValue = null;
                        object originalValue = null;

                        UsePropertyOrMethod:
                        if (usePropertyToGetValue)
                        {
                            if (!patchProperty.CanRead)
                            {
                                usePropertyToGetValue = false;
                                patchMethods = patchType.GetMethods();
                                goto UsePropertyOrMethod;
                            }

                            patchValue = patchProperty.GetValue(entityPatch);
                            originalValue = originalProperty.GetValue(clonedEntity);

                            if (patchValue == null
                                && (patchProperty.PropertyType.Name == "Nullable`1" || Type.GetTypeCode(patchProperty.PropertyType) == TypeCode.String)
                                && originalValue != null)
                                ShouldOverwrite = true;
                            else if (!patchValue.Equals(originalValue))
                                ShouldOverwrite = true;

                            if (ShouldOverwrite)
                            {
                                originalProperty.SetValue(clonedEntity, patchValue);
                                shouldPatch = true;
                            }
                        }
                        else
                        {
                            var getMethod = patchMethods.FirstOrDefault(p => p.Name == "get_" + propertyName);

                            if (getMethod != null)
                            {
                                patchValue = getMethod.Invoke(entityPatch, null);
                                originalValue = getMethod.Invoke(clonedEntity, null);

                                if (patchValue == null
                                    && (patchProperty.PropertyType.Name == "Nullable`1" || Type.GetTypeCode(patchProperty.PropertyType) == TypeCode.String)
                                    && originalValue != null)
                                    ShouldOverwrite = true;
                                else if (!patchValue.Equals(originalValue))
                                    ShouldOverwrite = true;
                            }

                            if (ShouldOverwrite)
                            {
                                originalProperty.SetValue(clonedEntity, patchValue);
                                shouldPatch = true;
                            }
                        }
                    }
                }
            }
            else
            {
                var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
                var properties = entityPatch.GetType().GetProperties(flags);

                foreach (var property in properties)
                {
                    if (TypeHelper.IsNullOrDefault<T>(property, ref entityPatch))
                        continue;

                    if (!property.GetValue(entityPatch).Equals(property.GetValue(clonedEntity)))
                    {
                        property.SetValue(clonedEntity, property.GetValue(entityPatch));
                        shouldPatch = true;
                    }
                }
            }

            if (shouldPatch)
                return Result.Ok(clonedEntity);
            else
                return Result.Fail<T>("Nothing to Patch", ErrorType.InvalidOperation);
        }

        public static Task<Result<T>> PatchAsync(T entityPatch, T originalEntity)
        {
            return Task.FromResult(Patch(entityPatch, originalEntity));
        }
    }
}
