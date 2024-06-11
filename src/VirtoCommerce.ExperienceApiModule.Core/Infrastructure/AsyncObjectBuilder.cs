using System;
using System.Threading;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

/// <summary>
/// Use this class to prevent loading objects from the database and running middleware for the current asynchronous control flow
/// </summary>
public static class AsyncObjectBuilder<T>
    where T : class
{
    private static readonly AsyncLocal<T> _storage = new();

    /// <summary>
    /// Returns true if the object is being built in the current asynchronous control flow.
    /// </summary>
    public static bool IsBuilding(out T obj)
    {
        obj = _storage.Value;
        return obj != null;
    }

    /// <summary>
    /// Add object to the current asynchronous control flow.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static IDisposable Build(T obj)
    {
        _storage.Value = obj;
        return new DisposableActionGuard(() => { _storage.Value = null; });
    }
}
