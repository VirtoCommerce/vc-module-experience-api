using System;
using System.Threading.Tasks;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
{
    public class DistributedLockService : IDistributedLockService
    {
        protected readonly IConnectionMultiplexer _redisConnMultiplexer;

        private readonly TimeSpan _expiry = TimeSpan.FromSeconds(20);
        private readonly TimeSpan _wait = TimeSpan.FromSeconds(10);
        private readonly TimeSpan _retry = TimeSpan.FromSeconds(2);

        public DistributedLockService(IConnectionMultiplexer redisConnMultiplexer)
        {
            _redisConnMultiplexer = redisConnMultiplexer;
        }

        public T Execute<T>(string resourceKey, Func<T> resolver)
        {
            using (var redlockFactory = RedLockFactory.Create(new RedLockMultiplexer[] { new RedLockMultiplexer(_redisConnMultiplexer) }))
            {
                using (var redLock = redlockFactory.CreateLock(resourceKey, _expiry, _wait, _retry))
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
            using (var redlockFactory = RedLockFactory.Create(new RedLockMultiplexer[] { new RedLockMultiplexer(_redisConnMultiplexer) }))
            {
                using (var redLock = await redlockFactory.CreateLockAsync(resourceKey, _expiry, _wait, _retry))
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
