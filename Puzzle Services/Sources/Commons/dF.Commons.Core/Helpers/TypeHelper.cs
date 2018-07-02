using System;
using System.Reflection;

namespace dF.Commons.Core.Helpers
{
    public static class TypeHelper
    {
        public static bool TryParse<T>(object entity, out T result)
        {
            try
            {
                result = (T)Convert.ChangeType(entity, Type.GetTypeCode(typeof(T)));
            }
            catch (Exception)
            {
                result = default(T);
                return false;
            }

            return true;
        }

        public static bool IsNullOrDefault<T>(PropertyInfo property, ref T entity)
        {
            var propertyType = property.PropertyType;
            var IsNullable = false;

            if (propertyType.IsConstructedGenericType)
            {
                if (propertyType.Name == "Nullable`1")
                    IsNullable = true;

                propertyType = property.PropertyType.GenericTypeArguments[0];
            }

            switch (Type.GetTypeCode(propertyType))
            {
                case TypeCode.Empty:
                    return true;
                case TypeCode.String:
                    if (string.IsNullOrWhiteSpace((string)property.GetValue(entity)))
                        return true;
                    break;
                case TypeCode.Int32:
                    if (IsNullOrDefault<Int32, T>(property, ref entity, IsNullable))
                        return true;
                    break;
                case TypeCode.Int16:
                    if (IsNullOrDefault<Int16, T>(property, ref entity, IsNullable))
                        return true;
                    break;
                case TypeCode.Byte:
                    if (IsNullOrDefault<Byte, T>(property, ref entity, IsNullable))
                        return true;
                    break;
                case TypeCode.Int64:
                    if (IsNullOrDefault<Int64, T>(property, ref entity, IsNullable))
                        return true;
                    break;
                case TypeCode.SByte:
                    if (IsNullOrDefault<SByte, T>(property, ref entity, IsNullable))
                        return true;
                    break;
                case TypeCode.Single:
                    if (IsNullOrDefault<Single, T>(property, ref entity, IsNullable))
                        return true;
                    break;
                case TypeCode.Double:
                    if (IsNullOrDefault<Double, T>(property, ref entity, IsNullable))
                        return true;
                    break;
                case TypeCode.Decimal:
                    if (IsNullOrDefault<Decimal, T>(property, ref entity, IsNullable))
                        return true;
                    break;
                case TypeCode.UInt16:
                    if (IsNullOrDefault<UInt16, T>(property, ref entity, IsNullable))
                        return true;
                    break;
                case TypeCode.UInt32:
                    if (IsNullOrDefault<UInt32, T>(property, ref entity, IsNullable))
                        return true;
                    break;
                case TypeCode.UInt64:
                    if (IsNullOrDefault<UInt64, T>(property, ref entity, IsNullable))
                        return true;
                    break;
                case TypeCode.Boolean:
                    if (IsNullOrDefault<Boolean, T>(property, ref entity, IsNullable))
                        return true;
                    break;
                case TypeCode.Char:
                    if (IsNullOrDefault<Char, T>(property, ref entity, IsNullable))
                        return true;
                    break;
                case TypeCode.DateTime:
                    if (IsNullOrDefault<DateTime, T>(property, ref entity))
                        return true;
                    break;
                case TypeCode.DBNull:
                case TypeCode.Object:
                default:
                    if ((propertyType == typeof(Guid)))
                    {
                        if (!IsNullOrDefault<Guid, T>(property, ref entity))
                            break;
                    }
                    return true;
            }

            return false;
        }

        public static bool IsNullOrDefault<Type, TEntity>(PropertyInfo property, ref TEntity entity, bool IsNullable = false)
        {
            var value = property.GetValue(entity);

            return value == null || (!IsNullable && value.Equals(default(Type)));
        }

        public static bool IsNull<TEntity>(PropertyInfo property, ref TEntity entity)
        {
            var value = property.GetValue(entity);
            return value == null;
        }
    }
}
