using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XGateway.Core.Services;

namespace VirtoCommerce.XGateway.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServiceGateways<T>(this IServiceCollection serviceCollection, Type[] implemtationTypes) where T : IServiceGateway
        {
            //TODO
            //foreach (var implType in implemtationTypes)
            //{
            //    serviceCollection.AddTransient(typeof(IServiceGateway), implType);
            //}

            //serviceCollection.AddTransient(typeof(T), factory =>
            //{
            //    var providers = factory.GetServices<IServiceGateway>();
            //    var config = factory.GetService<IOptions<ExperienceOptions>>().Value;
            //    return providers.FirstOrDefault(p => p.GetType().GetInterfaces().Contains(typeof(T)) && p.Gateway.EqualsInvariant(config.Gateway));
            //});
        }
    }
}
