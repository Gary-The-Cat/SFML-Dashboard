using System;

namespace Shared.Infrastructure;
public static class Noop
{
    public static Action NoAction => () => { };
}

