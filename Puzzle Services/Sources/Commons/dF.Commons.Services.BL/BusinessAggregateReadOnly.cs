using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

using dF.Commons.Core.Helpers;
using dF.Commons.Exceptions.Resources;
using dF.Commons.Models.BL;
using dF.Commons.Models.BL.Enums;
using dF.Commons.Models.BL.Extensions;
using dF.Commons.Models.Global.Constants;
using dF.Commons.Models.Global.Enums;
using dF.Commons.Models.Globals;
using dF.Commons.Models.Globals.Extensions;
using dF.Commons.Security.Constants;
using dF.Commons.Services.BL.Contracts;
using dF.Commons.Services.BL.Extensions;

namespace dF.Commons.Services.BL
{
    public class BusinessAggregateReadOnly<T> : Result<T>, IBusinessAggregateReadOnly<T> where T : class
    {
        #region Parameters
        public virtual string ResourceName => AggregateParams.ResourceName;
        public virtual string PluralResourceName => AggregateParams.PluralResourceName;

        public virtual List<ResourceLink> ResourceMap => AggregateParams.ResourceMap;

        public virtual BusinessAggregateParamsReadOnly<T> AggregateParams { get; private set; }
        #endregion

        #region Extension Points
        #region Before CRUD actions
        internal Func<Expression<Func<T, bool>>[], Result<Expression<Func<T, bool>>[]>> _beforeGetById = null;
        #endregion

        #region Data Retrieval
        internal Func<Expression<Func<T, bool>>[], Task<Result<T>>> _onGetByIdAsync = null;
        #endregion

        #region Security Clearance
        internal Func<ClaimsPrincipal, Result> _onGetAllSecurityClearance = null;
        internal Func<ClaimsPrincipal, Result> _onGetByIdSecurityClearance = null;
        #endregion

        #region After CRUD action
        internal Func<Result<IList<T>>, Result<IList<T>>> _afterGetAll = null;
        internal Func<Result<T>, Result<T>> _afterGetById = null;
        #endregion

        #region HATEOAS
        internal Func<ResponseContext<IList<T>>, ResponseContext<IList<T>>> _getAllHATEOAS = null;
        internal Func<ResponseContext<T>, ResponseContext<T>> _getByIdHATEOAS = null;
        #endregion

        #endregion

        #region Constructors
        public BusinessAggregateReadOnly(BusinessAggregateParamsReadOnly<T> businessAggregateParams) : base()
        {
            AggregateParams = businessAggregateParams;
        }

        public BusinessAggregateReadOnly(Result result, string resourceName) : base(result)
        {
            var context = new BusinessAggregateContext(new ClaimsPrincipal(), false);
            AggregateParams = new BusinessAggregateParamsReadOnly<T>(null, null, context, null, null, resourceName, $"{resourceName}(s)");
        }
        #endregion

        #region Common Functions
        protected Result<Expression<Func<T, bool>>[]> InjectKeySelectors(params Expression<Func<T, bool>>[] keySelectors)
        {
            var _keySelectors = keySelectors;

            if (AggregateParams.ParentKeys?.Count() > 0)
                _keySelectors = AggregateParams.ParentKeys.Concat(_keySelectors).ToArray();

            return Result.Ok(_keySelectors);
        }
        #endregion

        #region CRUD

        #region Get All
        // Before
        protected virtual Result OnGetAllSecurityClearance(params Expression<Func<T, bool>>[] where)
        {
            return _onGetAllSecurityClearance != null ?
                _onGetAllSecurityClearance(AggregateParams.Context.Principal) :
                AggregateParams.ResourceAccess(AggregateParams.Context.Principal, Actions.ReadAll);
        }

        // During
        protected virtual async Task<Result<IQueryable<T>>> OnGetAllAsync<TKey>(Expression<Func<T, TKey>> orderBy, int page, int? pageSize, bool descendingOrder, params Expression<Func<T, bool>>[] where)
        {
            //TODO: Define a way to insert a generic function to overwrite

            //var r = await _onGetAllAsync(orderBy, page, pageSize, descendingOrder);

            //if (r.IsSuccess)
            //    return r;

            return await AggregateParams.Repository.GetAllAsync(orderBy, page, pageSize, descendingOrder, where);
        }

        protected async Task<Result<IList<T>>> GetAllCoreAsync<TKey>(Expression<Func<T, TKey>> orderBy, int page = 1, int? pageSize = Paging.pageSize, bool descendingOrder = false, params Expression<Func<T, bool>>[] where)
        {
            try
            {
                return (await this.Then(() => OnGetAllSecurityClearance(where))
                                  .Async(r => InjectKeySelectors(where)
                                             .Async(keys => OnGetAllAsync(orderBy, page, pageSize, descendingOrder, keys.Value))))
                    .Map(entityResult => (IList<T>)entityResult.ToList())
                    .OnFailure(ErrorType.EntityNotFound, ExceptionMessages.EntityNotFoundException_NoRecordsFound, PluralResourceName)
                    .Then(results => AfterGetAll(results));
            }
            catch (Exception e)
            {
                throw new Exception(string.Format(ExceptionMessages.Exception_GetEntitiesError, PluralResourceName), e);
            }
        }

        protected async Task<Result<IList<K>>> GetAllCoreAsync<K, TKey>(Expression<Func<T, TKey>> orderBy, int page = 1, int? pageSize = Paging.pageSize, bool descendingOrder = false, params Expression<Func<T, bool>>[] where) where K : class, T
        {
            try
            {
                return (await GetAllCoreAsync(orderBy, page, pageSize, descendingOrder, where))
                    .Map(results =>
                    {
                        IList<K> mappedlist = new List<K>(results.Count);

                        foreach (var item in results)
                            mappedlist.Add(ShallowCloning<T, K>.Clone(item));

                        return mappedlist;
                    });
            }
            catch (Exception e)
            {
                throw new Exception(string.Format(ExceptionMessages.Exception_GetEntitiesError, PluralResourceName), e);
            }
        }

        public async Task<ResponseContext<IList<T>>> GetAllAsync<TKey>(Expression<Func<T, TKey>> orderBy, int page = 1, int? pageSize = Paging.pageSize, bool descendingOrder = false, params Expression<Func<T, bool>>[] where)
        {
            try
            {
                return (await GetAllCoreAsync(orderBy, page, pageSize, descendingOrder, where))
                    .ToResponseContext()
                    .Then(response => response.withRecordCount(response.Result.Count))
                    .ThenIf(AggregateParams.Context.IsHATEOASRequest, response => GetAllHATEOAS(response, page, pageSize));
            }
            catch (Exception e)
            {
                throw new Exception(string.Format(ExceptionMessages.Exception_GetEntitiesError, PluralResourceName), e);
            }
        }

        public async Task<ResponseContext<IList<K>>> GetAllAsync<K, TKey>(Expression<Func<T, TKey>> orderBy, int page = 1, int? pageSize = Paging.pageSize, bool descendingOrder = false, params Expression<Func<T, bool>>[] where) where K : class, T
        {
            try
            {
                return (await GetAllCoreAsync<K, TKey>(orderBy, page, pageSize, descendingOrder, where))
                    .ToResponseContext()
                    .Then(response => response.withRecordCount(response.Result.Count))
                    .ThenIf(AggregateParams.Context.IsHATEOASRequest, response => GetAllHATEOAS(response, page, pageSize));
            }
            catch (Exception e)
            {
                throw new Exception(string.Format(ExceptionMessages.Exception_GetEntitiesError, PluralResourceName), e);
            }
        }

        // After
        protected virtual Result<IList<T>> AfterGetAll(Result<IList<T>> results)
        {
            return _afterGetAll != null ? _afterGetAll(results) : results;
        }

        protected virtual ResponseContext<IList<K>> GetAllHATEOAS<K>(ResponseContext<IList<K>> response, int page, int? pageSize) where K : T
        {
            //return _getAllHATEOAS != null ? _getAllHATEOAS(response) : response;
            return response;
        }
        #endregion

        #region GetById
        // Before
        protected virtual Result OnGetByIdSecurityClearance(params Expression<Func<T, bool>>[] keySelectors)
        {
            return _onGetByIdSecurityClearance != null ?
                _onGetByIdSecurityClearance(AggregateParams.Context.Principal) :
                AggregateParams.ResourceAccess(AggregateParams.Context.Principal, Actions.Read);
        }

        protected virtual Result<Expression<Func<T, bool>>[]> BeforeGetById(params Expression<Func<T, bool>>[] keySelectors)
        {
            return _beforeGetById != null ? _beforeGetById(keySelectors) : Ok(keySelectors);
        }

        // During
        protected virtual async Task<Result<T>> OnGetByIdAsync(params Expression<Func<T, bool>>[] keySelectors)
        {
            return _onGetByIdAsync != null ? await _onGetByIdAsync(keySelectors) : await AggregateParams.Repository.GetByIdAsync(keySelectors);
        }

        protected async Task<Result<T>> GetByIdBasicAsync(params Expression<Func<T, bool>>[] keySelectors)
        {
            try
            {
                return (await this.Then(() => OnGetByIdSecurityClearance(keySelectors))
                                  .Async(r => BeforeGetById(keySelectors)
                                             .Then(k => InjectKeySelectors(k))
                                             .Async(keys => OnGetByIdAsync(keys.Value))))
                    .OnFailure(ErrorType.EntityNotFound, ExceptionMessages.EntityNotFoundException_NoRecordFoundByProvidedIDOrUnauthorize, ResourceName);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format(ExceptionMessages.Exception_GetEntityError, ResourceName), e);
            }
        }

        protected async Task<Result<T>> GetByIdCoreAsync(params Expression<Func<T, bool>>[] keySelectors)
        {
            try
            {
                return (await GetByIdBasicAsync(keySelectors))
                    .Then(result => AfterGetById(result));
            }
            catch (Exception e)
            {
                throw new Exception(string.Format(ExceptionMessages.Exception_GetEntityError, ResourceName), e);
            }
        }

        protected async Task<Result<K>> GetByIdCoreAsync<K>(params Expression<Func<T, bool>>[] keySelectors) where K : class, T
        {
            try
            {
                return (await GetByIdCoreAsync(keySelectors))
                    .Map(t => ShallowCloning<T, K>.Clone(t));
            }
            catch (Exception e)
            {
                throw new Exception(string.Format(ExceptionMessages.Exception_GetEntityError, ResourceName), e);
            }
        }

        public async Task<ResponseContext<T>> GetByIdAsync(params Expression<Func<T, bool>>[] keySelectors)
        {
            try
            {
                return (await GetByIdCoreAsync(keySelectors))
                    .ToResponseContext()
                    .withRecordCount(1)
                    .ThenIf(AggregateParams.Context.IsHATEOASRequest, response => GetByIdHATEOAS(response));
            }
            catch (Exception e)
            {
                throw new Exception(string.Format(ExceptionMessages.Exception_GetEntityError, ResourceName), e);
            }
        }

        public async Task<ResponseContext<K>> GetByIdAsync<K>(params Expression<Func<T, bool>>[] keySelectors) where K : class, T
        {
            try
            {
                return (await GetByIdCoreAsync<K>(keySelectors))
                    .ToResponseContext()
                    .withRecordCount(1)
                    .ThenIf(AggregateParams.Context.IsHATEOASRequest, response => GetByIdHATEOAS(response));
            }
            catch (Exception e)
            {
                throw new Exception(string.Format(ExceptionMessages.Exception_GetEntityError, ResourceName), e);
            }
        }

        // After
        protected virtual Result<T> AfterGetById(Result<T> result)
        {
            return _afterGetById != null ? _afterGetById(result) : result;
        }

        protected virtual ResponseContext<K> GetByIdHATEOAS<K>(ResponseContext<K> response) where K : class, T
        {
            //TODO: Define a way to insert a generic function to overwrite
            //if (_getByIdHATEOAS != null)
            //    return _getByIdHATEOAS(response);
            //else if (ResourceMap != null)
            if (ResourceMap != null)
            {
                response.ThenIf(AggregateParams.ResourceAccess(AggregateParams.Context.Principal, Actions.ReadAll), rc =>
                        rc.WithCollectionLink(ResourceMap.BuildLinkID(PluralResourceName, CRUDverbs.Read)));
            }

            return response;
        }
        #endregion

        #region Maps

        #endregion

        #endregion
    }
}
