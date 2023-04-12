using System;
using System.Threading;

namespace VirtoCommerce.XPurchase
{
    public static class CartAggregateLoadSuppressor
    {
        private class DisposableActionGuard : IDisposable
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

        private static readonly AsyncLocal<bool> Storage = new AsyncLocal<bool>();

        /// <summary>
        /// The flag indicates that CartAggregateLoad are suppressed for the current asynchronous control flow
        /// </summary>
        public static bool IsSupressed => Storage.Value;

        /// <summary>
        /// The flag indicates that CartAggregateLoad are suppressed for the current asynchronous control flow
        /// </summary>
        public static IDisposable Supress()
        {
            Storage.Value = true;
            return new DisposableActionGuard(() => { Storage.Value = false; });
        }
    }
}
