using System;
using PipelineNet.MiddlewareResolver;

namespace VirtoCommerce.ExperienceApiModule.Core.Pipelines
{
    public class ServiceProviderMiddlewareResolver : IMiddlewareResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceProviderMiddlewareResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object Resolve(Type type)
        {
            return _serviceProvider.GetService(type);
        }
    }
}
