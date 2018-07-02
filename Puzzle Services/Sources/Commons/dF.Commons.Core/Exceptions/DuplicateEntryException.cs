using System;

namespace dF.Commons.Core.Exceptions
{
    [Serializable]
    public class DuplicateEntryException : Exception
    {
        public DuplicateEntryException() { }
        public DuplicateEntryException(string message) : base(message) { }
        public DuplicateEntryException(string message, Exception inner) : base(message, inner) { }
        protected DuplicateEntryException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
