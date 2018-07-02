using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace dF.Commons.Core.Helpers
{
    public static class ShallowCloning<T> where T : class
    {
        private static Func<T, T> cloner = CreateCloner();

        private static Func<T, T> CreateCloner()
        {
            var cloneMethod = new DynamicMethod("CloneImplementation", typeof(T), new Type[] { typeof(T) }, true);
            var defaultCtor = typeof(T).GetConstructor(new Type[] { });

            var generator = cloneMethod.GetILGenerator();

            var loc1 = generator.DeclareLocal(typeof(T));

            generator.Emit(OpCodes.Newobj, defaultCtor);
            generator.Emit(OpCodes.Stloc, loc1);

            foreach (var field in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                generator.Emit(OpCodes.Ldloc, loc1);
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldfld, field);
                generator.Emit(OpCodes.Stfld, field);
            }

            generator.Emit(OpCodes.Ldloc, loc1);
            generator.Emit(OpCodes.Ret);

            return ((Func<T, T>)cloneMethod.CreateDelegate(typeof(Func<T, T>)));
        }

        public static T Clone(T myObject)
        {
            if (myObject == null)
                return null;

            return cloner(myObject);
        }
    }

    public static class ShallowCloning<T, K> where K : class, T
    {
        private static Func<K> createNewInstance = CreateNewInstance();

        private static Func<K> CreateNewInstance()
        {
            var cloneMethod = new DynamicMethod("CloneImplementation", typeof(K), new Type[] { typeof(K) }, true);
            var defaultCtor = typeof(K).GetConstructor(new Type[] { });

            var generator = cloneMethod.GetILGenerator();

            generator.Emit(OpCodes.Newobj, defaultCtor);
            generator.Emit(OpCodes.Ret);

            return ((Func<K>)cloneMethod.CreateDelegate(typeof(Func<K>)));
        }

        public static K Clone(T source)
        {
            if (source == null)
                return null;

            var sink = createNewInstance();

            var propertiesSource = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var propertiesSink = sink.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var propertySrc in propertiesSource)
            {
                var propertySnk = propertiesSink.FirstOrDefault(p => p.Name == propertySrc.Name);

                //if (propertySnk != null && !propertySrc.GetValue(source).Equals(propertySnk.GetValue(sink)))
                if (propertySnk != null)
                    propertySnk.SetValue(sink, propertySrc.GetValue(source));
            }

            return sink;
        }
    }
}

/// Sources:
/// - https://stackoverflow.com/questions/966451/fastest-way-to-do-shallow-copy-in-c-sharp