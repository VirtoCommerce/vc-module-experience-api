using System;
using Microsoft.Extensions.DependencyInjection;

namespace VirtoCommerce.ExperienceApiModule.Core.Pipelines
{
    public class GenericPipelineBuilder<TResult>
    {
        public GenericPipelineBuilder(IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            Services = services;
        }

        public IServiceCollection Services { get; }

        public GenericPipelineBuilder<TResult> Configure(Action<GenericPipelineOptions<TResult>> configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            Services.Configure(configuration);

            return this;
        }

        public GenericPipelineBuilder<TResult> AddMiddleware(Type type)
        {
            ArgumentNullException.ThrowIfNull(type);

            Services.AddTransient(type);

            return Configure(options => options.Middlewares.Add(type));
        }

        public GenericPipelineBuilder<TResult> ReplaceMiddleware(Type oldType, Type newType)
        {
            ArgumentNullException.ThrowIfNull(oldType);
            ArgumentNullException.ThrowIfNull(newType);

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

        public GenericPipelineBuilder<TResult> RemoveMiddleware(Type oldType)
        {
            ArgumentNullException.ThrowIfNull(oldType);

            return Configure(options =>
            {
                var oldTypeIndex = options.Middlewares.IndexOf(oldType);
                if (oldTypeIndex < 0)
                {
                    throw new OperationCanceledException($"{oldType} is not registered");
                }
                options.Middlewares.RemoveAt(oldTypeIndex);
            });
        }
    }
}
