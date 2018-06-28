using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;

using dF.Commons.Core.Helpers;
using dF.Commons.Models.BL;
using dF.Commons.Models.Global.Enums;
using dF.Commons.Models.Globals;
using dF.Commons.Models.Globals.Extensions;
using dF.Commons.Security.Constants;
using dF.Commons.Services.BL.Contracts.Context;
using dF.Commons.Services.Data.Contracts;

namespace dF.Commons.Services.BL
{
    public class BusinessAggregateParamsReadOnly<T> where T : class
    {
        public IBaseUOW UOW { get; private set; }
        public virtual IRepositoryReadOnly<T> Repository { get; private set; }

        public IDictionary<string, object> CarryOnBag { get; set; } = new Dictionary<string, object>();
        public IBusinessAggregateContext Context { get; private set; }
        public Func<ClaimsPrincipal, Actions, Result> ResourceAccess { get; }

        public List<ResourceLink> ResourceMap { get; }

        public string ResourceName { get; private set; }
        public string PluralResourceName { get; private set; }

        //public object[] KeyValues { get; internal set; }
        public Expression<Func<T, bool>>[] ParentKeys { get; internal set; } = null;
        protected Lazy<IDictionary<string, object>> _keyValues = null;
        public IDictionary<string, object> KeyValues { get => _keyValues.Value; }
        //public Expression<Func<T, bool>>[] KeySelectors { get; internal set; }

        #region Constructors
        public BusinessAggregateParamsReadOnly(IBaseUOW uow, IRepositoryReadOnly<T> repository, IBusinessAggregateContext context,
            Func<ClaimsPrincipal, Actions, Result> resourceAccess, List<ResourceLink> resourceMap, string resourceName,
            string pluralResourceName)
        {
            UOW = uow;

            Repository = repository;

            Context = context;

            ResourceAccess = resourceAccess;

            if (string.IsNullOrWhiteSpace(resourceName))
                throw new ArgumentException("Resource Name cannot be Null or Empty. Please provide a value.");
            ResourceName = resourceName;

            if (string.IsNullOrWhiteSpace(pluralResourceName))
                throw new ArgumentException("Plural Resource Name cannot be Null or Empty. Please provide a value.");
            PluralResourceName = pluralResourceName;

            ResourceMap = resourceMap;
        }

        public BusinessAggregateParamsReadOnly(IBaseUOW uow, IRepositoryReadOnly<T> repository, IBusinessAggregateContext context,
            Func<ClaimsPrincipal, Actions, Result> resourceAccess, List<ResourceLink> resourceMap, string resourceName,
            string pluralResourceName, params Expression<Func<T, bool>>[] parentKeys) :
            this(uow, repository, context, resourceAccess, resourceMap, resourceName, pluralResourceName)
        {
            ParentKeys = parentKeys;

            if (ParentKeys.Length > 0)
                _keyValues = new Lazy<IDictionary<string, object>>(() => ExpressionKeys.Unwrap(ParentKeys));
        }
        #endregion

        public Result<object> GetParentKey(string key)
        {
            if (ParentKeys.Length == 0)
                return Result.Fail<object>("No Parent Keys defined", ErrorType.ArgumentNull);

            if (KeyValues.TryGetValue(key, out object value))
                return Result.Ok(value);
            else
                return Result.Fail<object>($"Parent Key {key} missing", ErrorType.KeyNotFound);
        }

        public Result<TKey> GetParentKey<TKey>(string key)
        {
            return GetParentKey(key).Map(k => {
                if (TypeHelper.TryParse<TKey>(k.Value, out var result))
                    return Result.Ok(result);
                else
                    return Result.Fail<TKey>("Invalid Cast", ErrorType.InvalidOperation);
            });
        }

        public Result<object> GetCarryOnValue(string key)
        {
            if (CarryOnBag.TryGetValue(key, out var value))
                return Result.Ok(value);
            else
                return Result.Fail<object>($"Carry On Value with Key {key} missing", ErrorType.KeyNotFound);
        }

        public Result<TKey> GetCarryOnValue<TKey>(string key)
        {
            return GetCarryOnValue(key).Map(k => {
                if (TypeHelper.TryParse<TKey>(k.Value, out var result))
                    return Result.Ok(result);
                else
                    return Result.Fail<TKey>("Invalid Cast", ErrorType.InvalidOperation);
            });
        }

        public Result AddCarryOnValue(string key, object value)
        {
            try
            {
                CarryOnBag[key] = value;

                return Result.Ok();
            }
            catch (Exception e)
            {
                return Result.Fail(e);
            }
        }
    }
}
