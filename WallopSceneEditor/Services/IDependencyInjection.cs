using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.Services
{
    internal interface IDependencyInjection
    {
        void Add<T>(T instance)
            where T : notnull;

        void Add<T, TInherit>(TInherit instance)
            where TInherit : notnull, T
            where T : notnull;

        T Resolve<T>()
            where T : notnull;

        object Resolve(Type type);

        T CreateInstance<T>()
            where T : notnull;

        object CreateInstance(Type type);
    }
}
