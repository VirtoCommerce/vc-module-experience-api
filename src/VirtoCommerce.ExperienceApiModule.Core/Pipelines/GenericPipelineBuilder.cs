using System;
using Microsoft.Extensions.DependencyInjection;

namespace VirtoCommerce.ExperienceApiModule.Core.Pipelines
{
    public class GenericPipelineBuilder<TResult>
    {
        public GenericPipelineBuilder(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            Services = services;
        }

        public IServiceCollection Services { get; }

        public GenericPipelineBuilder<TResult> Configure(Action<GenericPipelineOptions<TResult>> configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            Services.Configure(configuration);

            return this;
        }

        public GenericPipelineBuilder<TResult> AddMiddleware(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            Services.AddTransient(type);

            return Configure(options => options.Middlewares.Add(type));
        }

        public GenericPipelineBuilder<TResult> ReplaceMiddleware(Type oldType, Type newType)
        {
            if (oldType == null)
            {
                throw new ArgumentNullException(nameof(oldType));
            }
            if (newType == null)
            {
                throw new ArgumentNullException(nameof(newType));
            }
            Services.AddTransient(newType);

            return Configure(options =>
            {
                var oldTypeIndex = options.Middlewares.IndexOf(oldType);
                if (oldTypeIndex < 0)
                {
                    throw new OperationCanceledException($"{oldType} is not registered");
                }
                options.Middlewares[oldTypeIndex] = newType;
            });
        }
    }
}
