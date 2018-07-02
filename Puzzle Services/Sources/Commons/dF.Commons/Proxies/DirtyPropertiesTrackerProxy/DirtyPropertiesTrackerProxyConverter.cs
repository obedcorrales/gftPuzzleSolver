using System;
using Newtonsoft.Json.Converters;

namespace dF.Commons.Proxies.DirtyPropertiesTrackerProxy
{
    public class DirtyPropertiesTrackerProxyConverter<TEntity> : CustomCreationConverter<TEntity> where TEntity : class, new()
    {
        public override TEntity Create(Type objectType)
        {
            return DirtyPropertiesTrackerProxy.MakeTrackable<TEntity>();
        }
    }
}
