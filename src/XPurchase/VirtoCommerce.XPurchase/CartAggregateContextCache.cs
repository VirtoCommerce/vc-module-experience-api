using System;
using System.Threading;

namespace VirtoCommerce.XPurchase
{
    /// <summary>
    /// Prevent from loading CartAggregate from the database and run middleware for the current asynchronous control flow
    /// </summary>
    public static class CartAggregateContextCache
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

        private static readonly AsyncLocal<CartAggregate> Storage = new AsyncLocal<CartAggregate>();

        /// <summary>
        /// Get Current Cart Aggregate from the current asynchronous control flow.
        /// </summary>
        public static CartAggregate CurrentCart => Storage.Value;

        /// <summary>
        /// Get flag indicates that CartAggregateLoad are suppressed for the current asynchronous control flow.
        /// </summary>
        public static bool IsCartCached => Storage.Value != null;

        /// <summary>
        /// Add CartAggregate to the current asynchronous control flow.
        /// </summary>
        /// <param name="cartAggregate"></param>
        /// <returns></returns>
        public static IDisposable Cache(CartAggregate cartAggregate)
        {
            Storage.Value = cartAggregate;
            return new DisposableActionGuard(() => { Storage.Value = cartAggregate; });
        }
    }
}
