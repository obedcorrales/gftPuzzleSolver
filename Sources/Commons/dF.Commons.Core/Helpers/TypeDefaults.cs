using System;

namespace dF.Commons.Core.Helpers
{
    public static class TypeDefaults
    {
        public static object Default(Type type)
        {
            if (type.IsValueType)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Int32:
                        return default(Int32);
                    case TypeCode.Int16:
                        return default(Int16);
                    case TypeCode.Byte:
                        return default(Byte);
                    case TypeCode.Int64:
                        return default(Int64);
                    case TypeCode.SByte:
                        return default(SByte);
                    case TypeCode.Single:
                        return default(Single);
                    case TypeCode.Double:
                        return default(Double);
                    case TypeCode.Decimal:
                        return default(Decimal);
                    case TypeCode.UInt16:
                        return default(UInt16);
                    case TypeCode.UInt32:
                        return default(UInt32);
                    case TypeCode.UInt64:
                        return default(UInt64);
                    case TypeCode.Boolean:
                        return default(Boolean);
                    case TypeCode.Char:
                        return default(Char);
                    case TypeCode.DateTime:
                        return default(DateTime);
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.Object:
                    default:
                        if ((type == typeof(Guid)))
                            return Guid.Empty;
                        break;
                }
            }
            else if (Type.GetTypeCode(type) == TypeCode.String)
                return default(String);

            return null;
        }

        public static bool IsDefaultValue(object value)
        {
            if (value == null)
                return true;

            var type = value.GetType();

            if (Type.GetTypeCode(type) == TypeCode.String)
                return (value.Equals(string.Empty));

            return value.Equals(TypeDefaults.Default(type));
        }
    }
}
