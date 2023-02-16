using System;
using System.Threading.Tasks;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
{
    public class NoLockService : IDistributedLockService
    {
        public T Execute<T>(string resourceKey, Func<T> resolver)
        {
            return resolver();
        }

        public Task<T> ExecuteAsync<T>(string resourceKey, Func<Task<T>> resolver)
        {
            return resolver();
        }
    }
}
