using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;

using dF.Commons.Models.BL;
using dF.Commons.Models.Globals;
using dF.Commons.Security.Constants;
using dF.Commons.Services.BL.Contracts.Context;
using dF.Commons.Services.Data.Contracts;

namespace dF.Commons.Services.BL
{
    public class BusinessAggregateParams<T> : BusinessAggregateParamsReadOnly<T> where T : class
    {
        public new IRepository<T> Repository { get; private set; }

        public BusinessAggregateParams(IBaseUOW uow, IRepository<T> repository, IBusinessAggregateContext context,
            Func<ClaimsPrincipal, Actions, Result> resourceAccess, List<ResourceLink> resourceMap, string resourceName,
            string pluralResourceName)
            : base(uow, repository, context, resourceAccess, resourceMap, resourceName, pluralResourceName)
        {
            Repository = repository;
        }

        public BusinessAggregateParams(IBaseUOW uow, IRepository<T> repository, IBusinessAggregateContext context,
            Func<ClaimsPrincipal, Actions, Result> resourceAccess, List<ResourceLink> resourceMap, string resourceName,
            string pluralResourceName, params Expression<Func<T, bool>>[] parentKeys)
            : this(uow, repository, context, resourceAccess, resourceMap, resourceName, pluralResourceName)
        {
            ParentKeys = parentKeys;

            if (ParentKeys.Length > 0)
                _keyValues = new Lazy<IDictionary<string, object>>(() => ExpressionKeys.Unwrap(ParentKeys));
        }
    }
}
