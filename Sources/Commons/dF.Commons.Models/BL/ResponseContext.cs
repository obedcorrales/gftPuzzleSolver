using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using dF.Commons.Models.Global.Constants;
using dF.Commons.Models.Global.Enums;
using dF.Commons.Models.Globals;

namespace dF.Commons.Models.BL
{
    public class ResponseContext : ResultBase
    {
        #region Fields
        protected object _result;
        private IList<Link> _links = null;
        private IDictionary<string, object> _context = null;
        protected ResponseContext _innerResponse = null;
        #endregion

        #region Properties
        public object Result
        {
            get
            {
                if (!IsSuccess)
                    return null;

                return _result;
            }
        }

        public ResponseContext InnerResponse
        {
            get
            {
                return _innerResponse;
            }
        }

        public IList<Link> Links
        {
            get { return _links != null ? _links : _links = new List<Link>(); }
            set { _links = value; }
        }
        public IDictionary<string, object> Context
        {
            get { return _context != null ? _context : _context = new Dictionary<string, object>(); }
            set { _context = value; }
        }
        public int? TotalCount { get; set; }
        public int? RecordCount { get; set; }
        #endregion

        #region Constructors
        protected ResponseContext(Result result) : base(result)
        {
            if (result.InnerResult != null)
                _innerResponse = new ResponseContext(result.InnerResult);
        }

        protected ResponseContext(ErrorType errorType = ErrorType.Empty, string error = "", Exception e = null)
            : base(errorType, error, e) { }

        protected ResponseContext(object result, IList<Link> links = null, IDictionary<string, object> context = null) : base()
        {
            _result = result;
            _links = links;
            _context = context;
        }
        #endregion

        #region Builders
        #region Link Builders
        #region WithLink Builders
        protected ResponseContext WithLink(Link link)
        {
            if (IsSuccess)
            {
                if (link.Href == null && link.LinkId == null)
                    System.Diagnostics.Debug.WriteLine("Link is Empty");
                else
                    Links.Add(link);
            }

            return this;
        }

        public ResponseContext WithLink(string rel, Uri hRef, string action = "", bool standardRel = false)
        {
            return WithLink(new Link(rel, hRef, action, standardRel));
        }

        public ResponseContext WithLink(string rel, LinkID linkId, string action = "", bool standardRel = false)
        {
            return WithLink(new Link(rel, linkId, action, standardRel));
        }

        public ResponseContext WithLink(LinkID linkId, string action = "", bool standardRel = false)
        {
            var rel = string.Empty;

            if (!string.IsNullOrWhiteSpace(linkId.ID))
                rel = linkId.ID;
            else
                throw new ArgumentNullException("Please provide a Link ID");

            return WithLink(new Link(rel, linkId, action, standardRel));
        }
        #endregion

        #region WithCollectionLink Builders
        public ResponseContext WithCollectionLink(string rel, Uri hRef, bool standardRel = false)
        {
            return WithLink(rel, hRef, LinkRelations.Item.Actions.GetCollection, standardRel);
        }

        public ResponseContext WithCollectionLink(string rel, LinkID linkId, bool standardRel = false)
        {
            return WithLink(rel, linkId, LinkRelations.Item.Actions.GetCollection, standardRel);
        }

        public ResponseContext WithCollectionLink(LinkID linkId, bool standardRel = false)
        {
            return WithLink(linkId, LinkRelations.Item.Actions.GetCollection, standardRel);
        }
        #endregion

        #region WithUpdateItemLink Builders
        public ResponseContext WithUpdateItemLink(string rel, Uri hRef, bool standardRel = false)
        {
            return WithLink(rel, hRef, LinkRelations.Item.Actions.UpdateItem, standardRel);
        }

        public ResponseContext WithUpdateItemLink(string rel, LinkID linkId, bool standardRel = false)
        {
            return WithLink(rel, linkId, LinkRelations.Item.Actions.UpdateItem, standardRel);
        }

        public ResponseContext WithUpdateItemLink(LinkID linkId, bool standardRel = false)
        {
            return WithLink(linkId, LinkRelations.Item.Actions.UpdateItem, standardRel);
        }
        #endregion

        #region WithUpdateRelatedItemLink Builders
        public ResponseContext WithUpdateRelatedItemLink(string rel, Uri hRef, bool standardRel = false)
        {
            return WithLink(rel, hRef, LinkRelations.Item.Actions.UpdateRelatedItem, standardRel);
        }

        public ResponseContext WithUpdateRelatedItemLink(string rel, LinkID linkId, bool standardRel = false)
        {
            return WithLink(rel, linkId, LinkRelations.Item.Actions.UpdateRelatedItem, standardRel);
        }

        public ResponseContext WithUpdateRelatedItemLink(LinkID linkId, bool standardRel = false)
        {
            return WithLink(linkId, LinkRelations.Item.Actions.UpdateRelatedItem, standardRel);
        }
        #endregion

        #region WithCreateRelatedLink Builders
        public ResponseContext WithCreateRelatedLink(string rel, Uri hRef, bool standardRel = false)
        {
            return WithLink(rel, hRef, LinkRelations.Item.Actions.CreateRelatedItem, standardRel);
        }

        public ResponseContext WithCreateRelatedLink(string rel, LinkID linkId, bool standardRel = false)
        {
            return WithLink(rel, linkId, LinkRelations.Item.Actions.CreateRelatedItem, standardRel);
        }

        public ResponseContext WithCreateRelatedLink(LinkID linkId, bool standardRel = false)
        {
            return WithLink(linkId, LinkRelations.Item.Actions.CreateRelatedItem, standardRel);
        }
        #endregion

        #region WithRelatedItemLink Builders
        public ResponseContext WithRelatedItemLink(string rel, Uri hRef, bool standardRel = false)
        {
            return WithLink(rel, hRef, LinkRelations.Item.Actions.AddRelatedItem, standardRel);
        }

        public ResponseContext WithRelatedItemLink(string rel, LinkID linkId, bool standardRel = false)
        {
            return WithLink(rel, linkId, LinkRelations.Item.Actions.AddRelatedItem, standardRel);
        }

        public ResponseContext WithRelatedItemLink(LinkID linkId, bool standardRel = false)
        {
            return WithLink(linkId, LinkRelations.Item.Actions.AddRelatedItem, standardRel);
        }
        #endregion

        #region WithRemoveRelatedItemLink Builders
        public ResponseContext WithRemoveRelatedItemLink(string rel, Uri hRef, bool standardRel = false)
        {
            return WithLink(rel, hRef, LinkRelations.Item.Actions.RemoveRelatedItem, standardRel);
        }

        public ResponseContext WithRemoveRelatedItemLink(string rel, LinkID linkId, bool standardRel = false)
        {
            return WithLink(rel, linkId, LinkRelations.Item.Actions.RemoveRelatedItem, standardRel);
        }

        public ResponseContext WithRemoveRelatedItemLink(LinkID linkId, bool standardRel = false)
        {
            return WithLink(linkId, LinkRelations.Item.Actions.RemoveRelatedItem, standardRel);
        }
        #endregion

        #region WithDeleteItemLink Builders
        public ResponseContext WithDeleteItemLink(string rel, Uri hRef, bool standardRel = false)
        {
            return WithLink(rel, hRef, LinkRelations.Item.Actions.DeleteItem, standardRel);
        }

        public ResponseContext WithDeleteItemLink(string rel, LinkID linkId, bool standardRel = false)
        {
            return WithLink(rel, linkId, LinkRelations.Item.Actions.DeleteItem, standardRel);
        }

        public ResponseContext WithDeleteItemLink(LinkID linkId, bool standardRel = false)
        {
            return WithLink(linkId, LinkRelations.Item.Actions.DeleteItem, standardRel);
        }
        #endregion
        #endregion

        #region Generic Builders
        public ResponseContext WithContextEntry(string key, object value)
        {
            Context.Add(key, value);

            return this;
        }

        public ResponseContext withTotalCount(int? totalCount)
        {
            if (IsSuccess && totalCount.HasValue && totalCount.Value > 0)
                TotalCount = totalCount;

            return this;
        }

        public ResponseContext withRecordCount(int? recordCount)
        {
            if (IsSuccess && recordCount.HasValue && recordCount.Value > 0)
                RecordCount = recordCount;

            return this;
        }

        public ResponseContext withInnerResponse(ResponseContext response)
        {
            _innerResponse = response;

            return this;
        }
        #endregion
        #endregion

        #region Static Constructors
        #region Sync
        #region Failure
        public static ResponseContext Fail(string message, ErrorType errorType = ErrorType.Empty)
        {
            return new ResponseContext(errorType, message);
        }

        public static ResponseContext<TResult> Fail<TResult>(string message, ErrorType errorType = ErrorType.Empty)
        {
            return new ResponseContext<TResult>(errorType, message);
        }

        public static ResponseContext Fail(Exception e)
        {
            return new ResponseContext(ErrorType.Empty, e.Message, e);
        }

        public static ResponseContext<TResult> Fail<TResult>(Exception e)
        {
            return new ResponseContext<TResult>(ErrorType.Empty, e.Message, e);
        }
        #endregion

        #region Success
        public static ResponseContext Ok(object result)
        {
            return new ResponseContext(result);
        }

        public static ResponseContext<TResult> Ok<TResult>(TResult value)
        {
            return new ResponseContext<TResult>(value);
        }

        public static ResponseContext Ok(object result, IList<Link> links = null, IDictionary<string, object> context = null)
        {
            return new ResponseContext(result, links, context);
        }

        public static ResponseContext<TResult> Ok<TResult>(TResult value, IList<Link> links = null, IDictionary<string, object> context = null)
        {
            return new ResponseContext<TResult>(value, links, context);
        }
        #endregion
        #endregion

        #region Async
        #region Failure
        public static Task<ResponseContext> FailAsync(string message, ErrorType errorType = ErrorType.Empty)
        {
            return Task.FromResult(new ResponseContext(errorType, message));
        }

        public static Task<ResponseContext<TResult>> FailAsync<TResult>(string message, ErrorType errorType = ErrorType.Empty)
        {
            return Task.FromResult(new ResponseContext<TResult>(errorType, message));
        }

        public static Task<ResponseContext> FailAsync(Exception e)
        {
            return Task.FromResult(new ResponseContext(ErrorType.Empty, e.Message, e));
        }

        public static Task<ResponseContext<TResult>> FailAsync<TResult>(Exception e)
        {
            return Task.FromResult(new ResponseContext<TResult>(ErrorType.Empty, e.Message, e));
        }
        #endregion

        #region Success
        public static Task<ResponseContext> OkAsync(object result)
        {
            return Task.FromResult(new ResponseContext(result));
        }

        public static Task<ResponseContext<TResult>> OkAsync<TResult>(TResult value)
        {
            return Task.FromResult(new ResponseContext<TResult>(value));
        }

        public static Task<ResponseContext> OkAsync(object result, IList<Link> links = null, IDictionary<string, object> context = null)
        {
            return Task.FromResult(new ResponseContext(result, links, context));
        }

        public static Task<ResponseContext<TResult>> OkAsync<TResult>(TResult value, IList<Link> links = null, IDictionary<string, object> context = null)
        {
            return Task.FromResult(new ResponseContext<TResult>(value, links, context));
        }
        #endregion
        #endregion
        #endregion

        public static explicit operator ResponseContext(Result result)
        {
            return new ResponseContext(result);
        }
    }

    public class ResponseContext<TResult> : ResponseContext
    {
        #region Properties
        public new TResult Result
        {
            get
            {
                if (!IsSuccess)
                    return default(TResult);
                //throw new InvalidOperationException();

                return _result != null ? (TResult)_result : default(TResult);
            }
        }

        public new ResponseContext<TResult> InnerResponse
        {
            get
            {
                return (ResponseContext<TResult>)_innerResponse;
            }
        }
        #endregion

        #region Constructors
        private ResponseContext(Result result)
            : base(result) { }

        private ResponseContext(Result<TResult> result)
            : base(result)
        {
            if (result.IsSuccess)
                _result = result.Value;
        }

        protected internal ResponseContext(ErrorType errorType = ErrorType.Empty, string error = "", Exception e = null)
            : base(errorType, error, e) { }

        protected internal ResponseContext(TResult result, IList<Link> links = null, IDictionary<string, object> context = null)
            : base(result, links, context) { }
        #endregion

        #region Builders
        #region Link Builders
        #region WithLink Builders
        public new ResponseContext<TResult> WithLink(string rel, Uri hRef, string action = "", bool standardRel = false)
        {
            base.WithLink(rel, hRef, action, standardRel);
            return this;
        }

        public new ResponseContext<TResult> WithLink(string rel, LinkID linkId, string action = "", bool standardRel = false)
        {
            base.WithLink(rel, linkId, action, standardRel);
            return this;
        }

        public new ResponseContext<TResult> WithLink(LinkID linkId, string action = "", bool standardRel = false)
        {
            base.WithLink(linkId, action, standardRel);
            return this;
        }
        #endregion

        #region WithCollectionLink Builders
        public new ResponseContext<TResult> WithCollectionLink(string rel, Uri hRef, bool standardRel = false)
        {
            base.WithCollectionLink(rel, hRef, standardRel);
            return this;
        }

        public new ResponseContext<TResult> WithCollectionLink(string rel, LinkID linkId, bool standardRel = false)
        {
            base.WithCollectionLink(rel, linkId, standardRel);
            return this;
        }

        public new ResponseContext<TResult> WithCollectionLink(LinkID linkId, bool standardRel = false)
        {
            base.WithCollectionLink(linkId, standardRel);
            return this;
        }
        #endregion

        #region WithUpdateItemLink Builders
        public new ResponseContext<TResult> WithUpdateItemLink(string rel, Uri hRef, bool standardRel = false)
        {
            base.WithUpdateItemLink(rel, hRef, standardRel);
            return this;
        }

        public new ResponseContext<TResult> WithUpdateItemLink(string rel, LinkID linkId, bool standardRel = false)
        {
            base.WithUpdateItemLink(rel, linkId, standardRel);
            return this;
        }

        public new ResponseContext<TResult> WithUpdateItemLink(LinkID linkId, bool standardRel = false)
        {
            base.WithUpdateItemLink(linkId, standardRel);
            return this;
        }
        #endregion

        #region WithUpdateRelatedItemLink Builders
        public new ResponseContext<TResult> WithUpdateRelatedItemLink(string rel, Uri hRef, bool standardRel = false)
        {
            base.WithUpdateRelatedItemLink(rel, hRef, standardRel);
            return this;
        }

        public new ResponseContext<TResult> WithUpdateRelatedItemLink(string rel, LinkID linkId, bool standardRel = false)
        {
            base.WithUpdateRelatedItemLink(rel, linkId, standardRel);
            return this;
        }

        public new ResponseContext<TResult> WithUpdateRelatedItemLink(LinkID linkId, bool standardRel = false)
        {
            base.WithUpdateRelatedItemLink(linkId, standardRel);
            return this;
        }
        #endregion

        #region WithCreateRelatedLink Builders
        public new ResponseContext<TResult> WithCreateRelatedLink(string rel, Uri hRef, bool standardRel = false)
        {
            base.WithCreateRelatedLink(rel, hRef, standardRel);
            return this;
        }

        public new ResponseContext<TResult> WithCreateRelatedLink(string rel, LinkID linkId, bool standardRel = false)
        {
            base.WithCreateRelatedLink(rel, linkId, standardRel);
            return this;
        }

        public new ResponseContext<TResult> WithCreateRelatedLink(LinkID linkId, bool standardRel = false)
        {
            base.WithCreateRelatedLink(linkId, standardRel);
            return this;
        }
        #endregion

        #region WithRelatedItemLink Builders
        public new ResponseContext<TResult> WithRelatedItemLink(string rel, Uri hRef, bool standardRel = false)
        {
            base.WithRelatedItemLink(rel, hRef, standardRel);
            return this;
        }

        public new ResponseContext<TResult> WithRelatedItemLink(string rel, LinkID linkId, bool standardRel = false)
        {
            base.WithRelatedItemLink(rel, linkId, standardRel);
            return this;
        }

        public new ResponseContext<TResult> WithRelatedItemLink(LinkID linkId, bool standardRel = false)
        {
            base.WithRelatedItemLink(linkId, standardRel);
            return this;
        }
        #endregion

        #region WithRelatedItemLink Builders
        public new ResponseContext<TResult> WithRemoveRelatedItemLink(string rel, Uri hRef, bool standardRel = false)
        {
            base.WithRemoveRelatedItemLink(rel, hRef, standardRel);
            return this;
        }

        public new ResponseContext<TResult> WithRemoveRelatedItemLink(string rel, LinkID linkId, bool standardRel = false)
        {
            base.WithRemoveRelatedItemLink(rel, linkId, standardRel);
            return this;
        }

        public new ResponseContext<TResult> WithRemoveRelatedItemLink(LinkID linkId, bool standardRel = false)
        {
            base.WithRemoveRelatedItemLink(linkId, standardRel);
            return this;
        }
        #endregion

        #region WithDeleteItemLink Builders
        public new ResponseContext<TResult> WithDeleteItemLink(string rel, Uri hRef, bool standardRel = false)
        {
            base.WithDeleteItemLink(rel, hRef, standardRel);
            return this;
        }

        public new ResponseContext<TResult> WithDeleteItemLink(string rel, LinkID linkId, bool standardRel = false)
        {
            base.WithDeleteItemLink(rel, linkId, standardRel);
            return this;
        }

        public new ResponseContext<TResult> WithDeleteItemLink(LinkID linkId, bool standardRel = false)
        {
            base.WithDeleteItemLink(linkId, standardRel);
            return this;
        }
        #endregion
        #endregion

        #region Generic Builders
        public new ResponseContext<TResult> WithContextEntry(string key, object value)
        {
            Context.Add(key, value);

            return this;
        }

        public new ResponseContext<TResult> withTotalCount(int? totalCount)
        {
            base.withTotalCount(totalCount);
            return this;
        }

        public new ResponseContext<TResult> withRecordCount(int? recordCount)
        {
            base.withRecordCount(recordCount);
            return this;
        }

        public ResponseContext<TResult> withInnerResponse(ResponseContext<TResult> response)
        {
            _innerResponse = response;

            return this;
        }
        #endregion
        #endregion

        #region Operators
        public static implicit operator TResult(ResponseContext<TResult> response)
        {
            return response.Result;
        }

        public static explicit operator ResponseContext<TResult>(Result result)
        {
            return new ResponseContext<TResult>(result);
        }

        public static explicit operator ResponseContext<TResult>(Result<TResult> result)
        {
            return new ResponseContext<TResult>(result);
        }
        #endregion
    }
}
