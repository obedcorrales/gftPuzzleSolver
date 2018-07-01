using System;
using System.Collections.Generic;

using dF.Commons.Models.Globals;

namespace dF.Commons.Models.Service
{
    public class APIResult
    {
        public object Results { get; set; }
        public Uri Self { get; set; }
        public IList<Link> Links { get; set; }
        public int? TotalCount { get; set; }
        public int? RecordCount { get; set; }

        private APIResult() { }

        public APIResult(object results, Uri self)
        {
            Results = results;
            Self = self;
            Links = new List<Link>();
        }

        public APIResult(object result, Uri self, IList<Link> links)
            : this(result, self)
        {
            Links = links;
        }
    }

    public class APIResult<T> : APIResult where T : class
    {
        public new T Results
        {
            get { return base.Results as T; }
            set { base.Results = value; }
        }

        public APIResult(T results, Uri self) : base(results, self) { }

        public APIResult(T results, Uri self, IList<Link> links) : base(results, self, links) { }
    }
}
