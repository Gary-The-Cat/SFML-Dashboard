using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.ExtensionMethods
{
    public static class WeakReferenceExtensions
    {
        public static T Value<T>(this WeakReference<T> guaranteedWeakReference) where T : class
        {
            if (guaranteedWeakReference.TryGetTarget(out T value))
            {
                return value;
            }

            return default(T);
        }
    }
}
