using System;

namespace dF.Commons.Core.Exceptions
{
    [Serializable]
    public class CommitFailureException : Exception
    {
        public CommitFailureException() { }
        public CommitFailureException(string message) : base(message) { }
        public CommitFailureException(string message, Exception inner) : base(message, inner) { }
        protected CommitFailureException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
