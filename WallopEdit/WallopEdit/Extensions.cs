using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WallopEdit
{
    internal static class Extensions
    {
        public static void ThrowIfNull<T>(this object source, Func<T> exFactory) where T : Exception
        {
            if(source == null)
            {
                throw exFactory();
            }
        }

        public static TResult Unwrap<TResult>(this TResult? source, Func<Exception> exFactory)
            => source.Unwrap<TResult, Exception>(exFactory);

        public static TResult Unwrap<TResult, TException>(this TResult? source, Func<TException> exFactory) where TException : Exception
        {
            if (source == null)
            {
                throw exFactory();
            }
            return source;
        }
    }
}
