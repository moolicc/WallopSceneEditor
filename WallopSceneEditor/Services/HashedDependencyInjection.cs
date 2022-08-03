using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.Services
{
    internal class HashedDependencyInjection : IDependencyInjection
    {
        private Dictionary<Type, object> _table;

        public HashedDependencyInjection()
        {
            _table = new Dictionary<Type, object>();
        }

        public void Add<T>(T instance)
            where T : notnull
        {
            _table.Add(typeof(T), instance!);
        }

        public void Add<T, TInherit>(TInherit instance)
            where TInherit : notnull, T
            where T : notnull
        {
            _table.Add(typeof(T), instance);
        }

        public T CreateInstance<T>()
            where T : notnull
        {
            return (T)CreateInstance(typeof(T));
        }

        public object CreateInstance(Type type)
        {
            var flags = System.Reflection.BindingFlags.Default
                | System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.Instance;

            var ctor = type.GetConstructors(flags)[0];
            var requiredParameterTypes = ctor.GetParameters();
            var parameters = new object[requiredParameterTypes.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var value = Resolve(requiredParameterTypes[i].ParameterType);
                if(value.GetType() != requiredParameterTypes[i].ParameterType)
                {
                    //value = Convert.ChangeType(value, requiredParameterTypes[i].ParameterType);
                }

                parameters[i] = value;
            }


            return ctor.Invoke(parameters);
        }

        public T Resolve<T>()
            where T : notnull
        {
            return (T)_table[typeof(T)];
        }

        public object Resolve(Type type)
        {
            return _table[type];
        }
    }
}
