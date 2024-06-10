using System;
using System.Threading.Tasks;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.Data.Services
{
    public class DistributedLockService : IDistributedLockService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        private readonly TimeSpan _expiry = TimeSpan.FromSeconds(20);
        private readonly TimeSpan _wait = TimeSpan.FromSeconds(10);
        private readonly TimeSpan _retry = TimeSpan.FromSeconds(2);

        public DistributedLockService(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }

        public T Execute<T>(string resourceKey, Func<T> resolver)
        {
            using var redLockFactory = RedLockFactory.Create(new[] { new RedLockMultiplexer(_connectionMultiplexer) });
            using var redLock = redLockFactory.CreateLock(resourceKey, _expiry, _wait, _retry);

            if (!redLock.IsAcquired)
            {
                throw new LockError("Service is busy.");
            }

            return resolver();
        }

        public async Task<T> ExecuteAsync<T>(string resourceKey, Func<Task<T>> resolver)
        {
            using var redLockFactory = RedLockFactory.Create(new[] { new RedLockMultiplexer(_connectionMultiplexer) });
            using var redLock = await redLockFactory.CreateLockAsync(resourceKey, _expiry, _wait, _retry);

            if (!redLock.IsAcquired)
            {
                throw new LockError("Service is busy.");
            }

            return await resolver();
        }
    }
}
