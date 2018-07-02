using System;
using System.Collections.Generic;
using Castle.DynamicProxy;

using dF.Commons.Helpers;

namespace dF.Commons.Proxies.DirtyPropertiesTrackerProxy
{
    [Serializable]
    public static class Trackable
    {
        public static TEntity MakeTrackable<TEntity>() where TEntity : class, new()
        {
            return DirtyPropertiesTrackerProxy.MakeTrackable<TEntity>();
        }

        public static TEntity MakeTrackable<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return DirtyPropertiesTrackerProxy.MakeTrackable<TEntity>(entity);
        }
    }

    [Serializable]
    public class Trackable<TEntity> : DirtyPropertiesTrackerProxy<TEntity> where TEntity : class
    {
        public Trackable() : base() { }

        public static TEntity Create()
        {
            return MakeTrackable<TEntity>();
        }

        public static TEntity Create(TEntity entity)
        {
            return MakeTrackable<TEntity>(entity);
        }

        #region Operators
        public static implicit operator TEntity(Trackable<TEntity> proxy)
        {
            return proxy.Proxy;
        }
        #endregion
    }

    [Serializable]
    public class DirtyPropertiesTrackerProxy
    {
        #region Static
        private static readonly ProxyGenerator _generator = new ProxyGenerator();
        private static readonly DirtyPropertiesTrackerProxyGenerationHook _generationHook = new DirtyPropertiesTrackerProxyGenerationHook();

        public static TEntity MakeTrackable<TEntity>(TEntity entity) where TEntity : class
        {
            var trackableInterceptor = new DirtyPropertiesTrackerInterceptor();
            var options = new ProxyGenerationOptions(_generationHook);

            options.AddMixinInstance(new DirtyPropertiesTracker());

            var proxy = (TEntity)_generator.CreateClassProxy<TEntity>(
                options,
                trackableInterceptor);

            if (entity == null)
                return proxy;

            var result = EntityPatcher<TEntity>.Patch(entity, proxy);

            if (result.IsSuccess)
                return result.Value;
            else
                return proxy;
        }

        public static TEntity MakeTrackable<TEntity>() where TEntity : class
        {
            return MakeTrackable<TEntity>(null);
        }

        #endregion

        #region Instance
        public object Proxy { get; internal set; }
        public IList<string> DirtyProperties { get { return ((IDirtyPropertiesTracker)Proxy).GetDirtyPropertiesNameList(); } }

        protected DirtyPropertiesTrackerProxy() { }

        public String GetEntityName(Object entity)
        {
            if (entity.GetType().Assembly.FullName.StartsWith("DynamicProxyGenAssembly2") == true)
            {
                return (entity.GetType().BaseType.FullName);
            }
            else
            {
                return (entity.GetType().FullName);
            }
        }
        #endregion
    }

    [Serializable]
    public class DirtyPropertiesTrackerProxy<TEntity> : DirtyPropertiesTrackerProxy where TEntity : class
    {
        public new TEntity Proxy
        {
            get { return base.Proxy as TEntity; }
            private set { base.Proxy = value; }
        }

        public static TEntity MakeTrackable()
        {
            return MakeTrackable<TEntity>();
        }

        public static TEntity MakeTrackable(TEntity entity)
        {
            return MakeTrackable<TEntity>(entity);
        }

        public DirtyPropertiesTrackerProxy() : base()
        {
            Proxy = MakeTrackable<TEntity>();
        }

        public DirtyPropertiesTrackerProxy(TEntity entity) : base()
        {
            Proxy = MakeTrackable<TEntity>(entity);
        }

        #region Operators
        public static implicit operator DirtyPropertiesTrackerProxy<TEntity>(TEntity proxy)
        {
            return MakeTrackable<TEntity>();
        }

        public static implicit operator TEntity(DirtyPropertiesTrackerProxy<TEntity> proxy)
        {
            return proxy.Proxy;
        }
        #endregion
    }
}

/// Research Sources
///  - https://github.com/castleproject/Core/blob/master/docs/dynamicproxy.md
///  
///  - http://kozmic.net/dynamic-proxy-tutorial/
///  - http://kozmic.net/2008/12/16/castle-dynamicproxy-tutorial-part-i-introduction/
///  - http://kozmic.pl/2009/01/17/castle-dynamic-proxy-tutorial-part-iii-selecting-which-methods-to
///  - http://kozmic.pl/2009/02/14/castle-dynamic-proxy-tutorial-part-v-interceptorselector-fine-grained-control
///  - http://kozmic.pl/2009/07/05/castle-dynamic-proxy-tutorial-part-xii-caching
///  - http://kozmic.net/2009/08/12/castle-dynamic-proxy-tutorial-part-xiii-mix-in-this-mix/
///  
///  - http://stackoverflow.com/questions/11082911/castle-dynamicproxy-get-unproxied-object
///  - http://stackoverflow.com/questions/3416306/how-to-really-down-cast-a-dynamicproxy-back-to-its-original-type-to-send-over?rq=1
///  - http://stackoverflow.com/questions/22377908/castle-dynamicproxy-create-a-new-property-with-a-custom-attribute-for-xml-seria
///  
///  - https://ayende.com/blog/4106/nhibernate-inotifypropertychanged
///  - http://www.codeproject.com/Articles/140042/Aspect-Examples-INotifyPropertyChanged-via-Aspects
///  - http://stackoverflow.com/questions/1315621/implementing-inotifypropertychanged-does-a-better-way-exist
///  
///  - http://chimera.labs.oreilly.com/books/1234000001708/ch13.html#_model_binders
/// 
///  - http://stackoverflow.com/questions/18342241/how-to-generate-poco-proxies-from-an-existing-database