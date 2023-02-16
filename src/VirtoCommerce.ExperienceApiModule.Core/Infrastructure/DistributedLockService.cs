using System;
using System.Threading.Tasks;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
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
            using (var redLockFactory = RedLockFactory.Create(new RedLockMultiplexer[] { new RedLockMultiplexer(_connectionMultiplexer) }))
            {
                using (var redLock = redLockFactory.CreateLock(resourceKey, _expiry, _wait, _retry))
                {
                    if (redLock.IsAcquired)
                    {
                        return resolver();
                    }
                }

                throw new LockError($"Service is busy.");
            }
        }

        public async Task<T> ExecuteAsync<T>(string resourceKey, Func<Task<T>> resolver)
        {
            using (var redLockFactory = RedLockFactory.Create(new RedLockMultiplexer[] { new RedLockMultiplexer(_connectionMultiplexer) }))
            {
                using (var redLock = await redLockFactory.CreateLockAsync(resourceKey, _expiry, _wait, _retry))
                {
                    if (redLock.IsAcquired)
                    {
                        return await resolver();
                    }
                }

                throw new LockError($"Service is busy.");
            }
        }
    }
}
