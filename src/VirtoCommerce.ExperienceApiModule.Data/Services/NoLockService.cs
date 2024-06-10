using System;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.Data.Infrastructure
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
