using System;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

public class DisposableActionGuard : IDisposable
{
    private readonly Action _action;

    public DisposableActionGuard(Action action)
    {
        _action = action;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _action();
        }
    }
}
