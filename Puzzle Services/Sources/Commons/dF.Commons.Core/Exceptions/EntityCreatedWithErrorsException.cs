using System;

namespace dF.Commons.Core.Exceptions
{
    [Serializable]
    public class EntityCreatedWithErrorsException : Exception
    {
        public string EntityID { get; set; }

        public EntityCreatedWithErrorsException() { }

        public EntityCreatedWithErrorsException(string message) : base(message) { }
        public EntityCreatedWithErrorsException(string message, string entityId) : base(message)
        {
            EntityID = entityId;
        }

        public EntityCreatedWithErrorsException(string message, Exception inner) : base(message, inner) { }
        public EntityCreatedWithErrorsException(string message, Exception inner, string entityId) : base(message, inner)
        {
            EntityID = entityId;
        }

        protected EntityCreatedWithErrorsException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
