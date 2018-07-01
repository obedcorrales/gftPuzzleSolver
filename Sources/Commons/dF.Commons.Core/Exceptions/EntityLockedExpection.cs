using System;

namespace dF.Commons.Core.Exceptions
{
    [Serializable]
    public class EntityLockedExpection : Exception
    {
        public EntityLockedExpection() { }
        public EntityLockedExpection(string message) : base(message) { }
        public EntityLockedExpection(string message, Exception inner) : base(message, inner) { }
        protected EntityLockedExpection(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
