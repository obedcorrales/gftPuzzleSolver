using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

using dF.Commons.Exceptions.Resources;
using dF.Commons.Helpers;
using dF.Commons.Models.BL;
using dF.Commons.Models.BL.Enums;
using dF.Commons.Models.BL.Extensions;
using dF.Commons.Models.Global.Enums;
using dF.Commons.Models.Globals;
using dF.Commons.Models.Globals.Extensions;
using dF.Commons.Security.Constants;
using dF.Commons.Services.BL.Contracts;
using dF.Commons.Services.BL.Extensions;

namespace dF.Commons.Services.BL
{
    public class BusinessAggregate<T> : BusinessAggregateReadOnly<T>, IBusinessAggregate<T> where T : class
    {
        #region Parameters
        public new BusinessAggregateParams<T> AggregateParams { get; private set; }
        protected bool CommitInmediately { get; set; }
        protected bool _wrapAddInTrasaction = true;
        protected bool _wrapUpdateInTrasaction = true;
        #endregion

        #region Extension Points

        #region Security Clearance
        internal Func<ClaimsPrincipal, Result> _onAddSecurityClearance = null;
        internal Func<ClaimsPrincipal, Result> _onUpdateSecurityClearance = null;
        internal Func<ClaimsPrincipal, Result> _onDeleteSecurityClearance = null;
        #endregion

        #region Validation
        internal Func<T, Task<Result>> _addValidation = null;
        internal Func<T, Task<Result>> _updateValidation = null;
        #endregion

        #region Before Commit action
        internal Func<T, Task<Result<T>>> _beforeAdd = null;
        internal Func<T, Task<Result<T>>> _beforeUpdate = null;
        #endregion

        #region Include in Transaction
        internal Func<T, Task<Result<T>>> _includeInAddTransaction = null;
        internal Func<T, Task<Result<T>>> _includeInUpdateTransaction = null;
        #endregion

        #region After CRUD action
        internal Func<Result<T>, Task<Result<T>>> _afterAdd = null;
        internal Func<Result<T>, Task<Result<T>>> _afterUpdate = null;
        #endregion

        #region HATEOAS
        internal Func<ResponseContext<T>, ResponseContext<T>> _addHATEOAS = null;
        internal Func<ResponseContext<T>, ResponseContext<T>> _deleteHATEOAS = null;
        internal Func<ResponseContext<T>, ResponseContext<T>> _updateHATEOAS = null;
        #endregion

        #endregion

        public BusinessAggregate(BusinessAggregateParams<T> businessAggregateParams, bool commitInmediately = true)
            : base(businessAggregateParams)
        {
            AggregateParams = businessAggregateParams;
            CommitInmediately = commitInmediately;
        }

        public BusinessAggregate(Result result, string resourceName) : base(result, resourceName) { }

        #region CRUD
        #region Get All
        protected override ResponseContext<IList<K>> GetAllHATEOAS<K>(ResponseContext<IList<K>> response, int page, int? pageSize)
        {
            //if (_getAllHATEOAS != null)
            //    return _getAllHATEOAS(response);
            //else if (ResourceMap != null)
            if (ResourceMap != null)
                response.ThenIf(AggregateParams.ResourceAccess(AggregateParams.Context.Principal, Actions.Add), rc =>
                        rc.WithCreateRelatedLink(ResourceMap.BuildLinkID(ResourceName, CRUDverbs.Create)));
            // Paging Links are added by default later on by the HATEOAS engine, unless defined here already

            return response;

            //return base.GetAllHATEOAS(response, page, pageSize);
        }
        #endregion

        #region GetById
        protected override ResponseContext<K> GetByIdHATEOAS<K>(ResponseContext<K> response)
        {

            if (ResourceMap != null)
            {
                base.GetByIdHATEOAS(response).ThenIf(AggregateParams.ResourceAccess(AggregateParams.Context.Principal, Actions.Add), rc =>
                        rc.WithCreateRelatedLink(ResourceMap.BuildLinkID(ResourceName, CRUDverbs.Create)));
                // TODO: Find a way to inject the ID names for the model
                //
                //.ThenIf(aggregateParams.ResourceAccess(aggregateParams.Context.Principal, Actions.Update), rc =>
                //    rc.AddUpdateItemLink(ResourceMap.BuildLinkID(ResourceName, CRUDverbs.Update).withParameter( /* Need the IDs */, /* Could use the IDs send as object[] in GetById above */ )))
                //.ThenIf(aggregateParams.ResourceAccess(aggregateParams.Context.Principal, Actions.Remove), rc =>
                //    rc.AddRemoveRelatedItemLink(ResourceMap.BuildLinkID(ResourceName, CRUDverbs.Delete)));
            }

            return response;
        }
        #endregion

        #region Add
        protected virtual Task<Result> ValidateAddEntity(T entity)
        {
            return _addValidation != null ? _addValidation(entity) : OkAsync();
        }

        protected virtual Result OnAddSecurityClearance()
        {
            return _onAddSecurityClearance != null ?
                _onAddSecurityClearance(AggregateParams.Context.Principal) :
                AggregateParams.ResourceAccess(AggregateParams.Context.Principal, Actions.Add);
        }

        protected virtual Task<Result<T>> OnBeforeAdd(T entity)
        {
            return _beforeAdd != null ? _beforeAdd(entity) : OkAsync(entity);
        }

        protected virtual async Task<Result<T>> OnAddAsync(T entity)
        {
            return (await AggregateParams.Repository.AddAsync(entity)).Then(() => Ok(entity));
        }

        protected virtual Task<Result<T>> IncludeInAddTransaction(T entity)
        {
            if (_includeInAddTransaction != null)
                return _includeInAddTransaction(entity);
            else
            {
                _wrapAddInTrasaction = false;
                return OkAsync(entity);
            }
        }

        public async Task<ResponseContext<T>> AddAsync(T entity)
        {
            try
            {
                if (entity == null)
                    return ResponseContext.Fail<T>(string.Format(ExceptionMessages.ArgumentException_ProvideValue, ResourceName), ErrorType.Argument);

                return (await this.Async(async r => (await ValidateAddEntity(entity))
                                            .Then(() => OnAddSecurityClearance()))
                                  .ThenAsync(() => OnBeforeAdd(entity))
                                  .ThenAsync(result => OnAddAsync(result.Value))
                                  .ThenAsync(result => IncludeInAddTransaction(result.Value))
                                  .ThenIfAsync(CommitInmediately, async result =>
                                  {
                                      Result<int> commitResult;
                                      if (_wrapAddInTrasaction)
                                          commitResult = await AggregateParams.UOW.CommitWithinTransactionAsync();
                                      else
                                          commitResult = await AggregateParams.UOW.CommitAsync();

                                      return commitResult.Ensure(r => r.Value > 0, ErrorType.CommitFailure, string.Format(ExceptionMessages.Exception_CreateEntityError, ResourceName))
                                                         .Return(() => result);
                                  })
                                  .ThenAsync(result => AfterAdd(result)))
                    .ToResponseContext()
                    .withRecordCount(1)
                    .ThenIf(AggregateParams.Context.IsHATEOASRequest, response => AddHATEOAS(response));
            }
            catch (Exception e)
            {
                throw new Exception(string.Format(ExceptionMessages.Exception_CreateEntityError, ResourceName), e);
            }
        }

        protected virtual Task<Result<T>> AfterAdd(Result<T> result)
        {
            return _afterAdd != null ? _afterAdd(result) : Task.FromResult(result);
        }

        protected virtual ResponseContext<T> AddHATEOAS(ResponseContext<T> response)
        {
            if (_addHATEOAS != null)
                return _addHATEOAS(response);
            else if (ResourceMap != null)
                response.ThenIf(AggregateParams.ResourceAccess(AggregateParams.Context.Principal, Actions.Add), rc =>
                        rc.WithCreateRelatedLink(ResourceMap.BuildLinkID(ResourceName, CRUDverbs.Create)))
                    .ThenIf(AggregateParams.ResourceAccess(AggregateParams.Context.Principal, Actions.ReadAll), rc =>
                        rc.WithCollectionLink(ResourceMap.BuildLinkID(PluralResourceName, CRUDverbs.Read)));
            // TODO: Find a way to inject the ID names for the model
            //
            //.ThenIf(aggregateParams.ResourceAccess(aggregateParams.Context.Principal, Actions.Update), rc =>
            //    rc.AddUpdateItemLink(ResourceMap.BuildLinkID(ResourceName, CRUDverbs.Update).withParameter( /* Need the IDs */, /* Could use the IDs send as object[] in GetById above */ )))
            //.ThenIf(aggregateParams.ResourceAccess(aggregateParams.Context.Principal, Actions.Remove), rc =>
            //    rc.AddRemoveRelatedItemLink(ResourceMap.BuildLinkID(ResourceName, CRUDverbs.Delete)));

            return response;
        }
        #endregion

        #region Update
        protected virtual Task<Result> ValidateUpdateEntity(T entity)
        {
            return _updateValidation != null ? _updateValidation(entity) : OkAsync();
        }

        protected virtual Result OnUpdateSecurityClearance()
        {
            return _onUpdateSecurityClearance != null ?
                _onUpdateSecurityClearance(AggregateParams.Context.Principal) :
                AggregateParams.ResourceAccess(AggregateParams.Context.Principal, Actions.Update);
        }

        protected virtual Task<Result<T>> OnBeforeUpdate(T entity)
        {
            return _beforeUpdate != null ? _beforeUpdate(entity) : OkAsync(entity);
        }

        protected virtual async Task<Result<T>> OnUpdateAsync(T entity)
        {
            return (await AggregateParams.Repository.UpdateAsync(entity)
                            .ToResultAsync<T>()
                            .ThenAsync(() => Result.OkAsync(entity)));
        }

        protected virtual Task<Result<T>> IncludeInUpdateTransaction(T entity)
        {
            if (_includeInUpdateTransaction != null)
                return _includeInUpdateTransaction(entity);
            else
            {
                _wrapUpdateInTrasaction = false;
                return OkAsync(entity);
            }
        }

        public async Task<ResponseContext<T>> UpdateAsync(T entity, params Expression<Func<T, bool>>[] keySelectors)
        {
            try
            {
                if (entity == null)
                    return ResponseContext.Fail<T>(string.Format(ExceptionMessages.ArgumentException_ProvideValue, ResourceName), ErrorType.Argument);

                return (await this.Async(async r => (await ValidateUpdateEntity(entity))
                                            .Then(() => OnUpdateSecurityClearance()))
                                  .ThenAsync(() => GetByIdBasicAsync(keySelectors))
                                  .ThenAsync(result => EntityPatcher<T>.PatchAsync(entity, result.Value))
                                  .ThenAsync(result => OnBeforeUpdate(result.Value))
                                  .ThenAsync(result => OnUpdateAsync(result.Value))
                                  .ThenAsync(result => IncludeInUpdateTransaction(result.Value))
                                  .ThenIfAsync(CommitInmediately, async result =>
                                  {
                                      Result<int> commitResult;
                                      if (_wrapUpdateInTrasaction)
                                          commitResult = await AggregateParams.UOW.CommitWithinTransactionAsync();
                                      else
                                          commitResult = await AggregateParams.UOW.CommitAsync();

                                      return commitResult.Ensure(r => r.Value > 0, ErrorType.CommitFailure, string.Format(ExceptionMessages.Exception_UpdateEntityError, ResourceName))
                                                         .Return(() => result);
                                  })
                                  .ThenAsync(result => AfterUpdate(result)))
                    .ToResponseContext()
                    .withRecordCount(1)
                    .ThenIf(AggregateParams.Context.IsHATEOASRequest, response => UpdateHATEOAS(response));
            }
            catch (Exception e)
            {
                throw new Exception(string.Format(ExceptionMessages.Exception_UpdateEntityError, ResourceName), e);
            }
        }

        protected virtual Task<Result<T>> AfterUpdate(Result<T> result)
        {
            return _afterUpdate != null ? _afterUpdate(result) : Task.FromResult(result);
        }

        protected virtual ResponseContext<T> UpdateHATEOAS(ResponseContext<T> response)
        {
            if (_updateHATEOAS != null)
                return _updateHATEOAS(response);
            else if (ResourceMap != null)
            {
                response.ThenIf(AggregateParams.ResourceAccess(AggregateParams.Context.Principal, Actions.Add), rc =>
                        rc.WithCreateRelatedLink(ResourceMap.BuildLinkID(ResourceName, CRUDverbs.Create)))
                    .ThenIf(AggregateParams.ResourceAccess(AggregateParams.Context.Principal, Actions.ReadAll), rc =>
                        rc.WithCollectionLink(ResourceMap.BuildLinkID(PluralResourceName, CRUDverbs.Read)));
                // TODO: Find a way to inject the ID names for the model
                //
                //.ThenIf(aggregateParams.ResourceAccess(aggregateParams.Context.Principal, Actions.Update), rc =>
                //    rc.AddUpdateItemLink(ResourceMap.BuildLinkID(ResourceName, CRUDverbs.Update).withParameter( /* Need the IDs */, /* Could use the IDs send as object[] in GetById above */ )))
                //.ThenIf(aggregateParams.ResourceAccess(aggregateParams.Context.Principal, Actions.Remove), rc =>
                //    rc.AddRemoveRelatedItemLink(ResourceMap.BuildLinkID(ResourceName, CRUDverbs.Delete)));
            }

            return response;
        }
        #endregion

        #region Delete
        public async Task<ResponseContext> DeleteAsync(params Expression<Func<T, bool>>[] keySelectors)
        {
            try
            {
                return (await this.Then(() => OnDeleteSecurityClearance())
                                  .Async(r => GetByIdBasicAsync(keySelectors))
                                  .ThenAsync(entity => OnDeleteAsync(entity).ToResultAsync<T>())
                                  .ThenIfAsync(CommitInmediately, async result =>
                                        await AggregateParams.UOW.CommitAsync()
                                            .EnsureAsync(r => r.Value > 0, ErrorType.CommitFailure, "")
                                            .ReturnAsync(() => result)))
                    .ToResponseContext();
            }
            catch (Exception e)
            {
                throw new Exception(string.Format(ExceptionMessages.Exception_DeleteEntityError, ResourceName), e);
            }
        }

        protected virtual Result OnDeleteSecurityClearance()
        {
            return _onDeleteSecurityClearance != null ?
                _onDeleteSecurityClearance(AggregateParams.Context.Principal) :
                AggregateParams.ResourceAccess(AggregateParams.Context.Principal, Actions.Remove);
        }

        protected virtual async Task<Result> OnDeleteAsync(T entity)
        {
            return await AggregateParams.Repository.DeleteAsync(entity);
        }
        #endregion
        #endregion
    }
}
