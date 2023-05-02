using System;

namespace Shared.Infrastructure;
public static class Assert
{
    public static void IsTrue(bool condition)
    {
        if (!condition)
        {
            throw new ArgumentException();
        }
    }
    public static void IsFalse(bool condition)
    {
        if (condition)
        {
            throw new ArgumentException();
        }
    }
}
