using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using dF.Commons.Models.Global.Enums;
using dF.Commons.Models.Globals;

namespace dF.Commons.Services.Data.InMemory
{
    public class PropertyDescriptor
    {
        public string Name { get; set; }
        public Type Type { get; protected set; }
        public PropertyInfo Property { get; private set; }
        public bool IsIdentity { get; private set; } = false;
        public bool IsNullable { get; private set; } = false;
        public IList<KeyValuePair<string, object>> AssignedIds { get; private set; }

        bool _isNumeric = false;
        bool _isGuid = false;
        double _identityMax = 0;

        public Result<object> GetNextId(params object[] parentKeys)
        {
            if (!IsIdentity)
                return Result.Fail<object>("Not an Identity", ErrorType.Argument);

            // TODO: make thread safe
            if (_isNumeric)
            {
                var keyString = "";

                foreach (var key in parentKeys)
                {
                    keyString += $"{ key.ToString() }.";
                }

                double nextId = 0;

                var maxObject = AssignedIds.Where(id => id.Key == keyString).OrderByDescending(id => id.Value).FirstOrDefault();
                if (maxObject.Key != null)
                {
                    double max = 0;
                    if (double.TryParse(maxObject.Value.ToString(), out max))
                        nextId = max + 1;
                    else
                        Result.Fail<object>("Not a numeric type", ErrorType.Argument);

                    if (nextId >= _identityMax)
                        Result.Fail<object>("Id exceeds type's max value", ErrorType.IndexOutOfRange);
                }

                return Result.Ok<object>(nextId);
            }
            else if (_isGuid)
                return Result.Ok<object>(Guid.NewGuid());
            else
                return Result.Fail<object>("Not an Identity Type", ErrorType.Argument);
        }

        public Result AssignId(object value, bool isParentId = false, params object[] parentKeys)
        {
            if (_isNumeric)
            {
                var keyString = "";

                foreach (var key in parentKeys)
                {
                    keyString += $"{ key.ToString() }.";
                }

                if (AssignedIds.Count(i => i.Key == keyString && i.Value.ToString() == value.ToString()) > 0)
                {
                    if (!isParentId)
                        return Result.Fail("Id had already been assigned", ErrorType.DuplicateEntry);
                }
                else
                    AssignedIds.Add(new KeyValuePair<string, object>(keyString, value));
            }

            return Result.Ok();
        }

        public static Result<PropertyDescriptor> Create(PropertyInfo property, bool isIdentity)
        {
            var isNullable = false;
            var isNumeric = false;
            var isGuid = false;
            var Type = property.PropertyType;
            double identityMax = 0;

            if (Type.Name == "Nullable`1")
                isNullable = true;

            if (isNullable && isIdentity)
                return Result.Fail<PropertyDescriptor>("Identity can't be nullable", ErrorType.ArgumentNull);

            if (isIdentity == true)
            {
                isNumeric = true;

                switch (Type.GetTypeCode(Type))
                {
                    case TypeCode.Byte:
                        identityMax = Byte.MaxValue;
                        break;
                    case TypeCode.SByte:
                        identityMax = SByte.MaxValue;
                        break;
                    case TypeCode.UInt16:
                        identityMax = UInt16.MaxValue;
                        break;
                    case TypeCode.UInt32:
                        identityMax = UInt32.MaxValue;
                        break;
                    case TypeCode.UInt64:
                        identityMax = UInt64.MaxValue;
                        break;
                    case TypeCode.Int16:
                        identityMax = Int16.MaxValue;
                        break;
                    case TypeCode.Int32:
                        identityMax = Int32.MaxValue;
                        break;
                    case TypeCode.Int64:
                        identityMax = Int64.MaxValue;
                        break;
                    case TypeCode.Decimal:
                        identityMax = (Double)Decimal.MaxValue;
                        break;
                    case TypeCode.Double:
                        identityMax = Double.MaxValue;
                        break;
                    case TypeCode.Single:
                        identityMax = Single.MaxValue;
                        break;
                    default:
                        isNumeric = false;

                        if (Type == typeof(Guid))
                            isGuid = true;
                        else
                            return Result.Fail<PropertyDescriptor>("Not an Identity Type", ErrorType.Argument);
                        break;
                }
            }

            return Result.Ok(new PropertyDescriptor(property, isIdentity, identityMax, isNumeric, isGuid));
        }

        protected PropertyDescriptor(PropertyInfo property, bool isIdentity, double identityMax, bool isNumeric, bool isGuid)
        {
            Property = property;
            Name = property.Name;
            Type = property.PropertyType;
            IsIdentity = isIdentity;
            _identityMax = identityMax;
            _isNumeric = isNumeric;
            _isGuid = isGuid;
            AssignedIds = new List<KeyValuePair<string, object>>();
        }
    }
}
