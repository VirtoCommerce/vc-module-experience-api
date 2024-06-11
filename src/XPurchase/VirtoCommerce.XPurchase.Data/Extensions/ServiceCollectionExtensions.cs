using System;
using GraphQL.Server;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.TaxModule.Core.Model;
using VirtoCommerce.XDigitalCatalog.Core.Models;
using VirtoCommerce.XPurchase.Core;
using VirtoCommerce.XPurchase.Core.Authorization;
using VirtoCommerce.XPurchase.Core.Services;
using VirtoCommerce.XPurchase.Core.Validators;
using VirtoCommerce.XPurchase.Data.Middlewares;
using VirtoCommerce.XPurchase.Data.Services;

namespace VirtoCommerce.XPurchase.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXPurchase(this IServiceCollection services, IGraphQLBuilder graphQlbuilder)
        {
            var assemblyMarkerCore = typeof(XPurchaseCoreAnchor);
            var assemblyMarkerData = typeof(XPurchaseDataAnchor);
            graphQlbuilder.AddGraphTypes(assemblyMarkerCore);
            services.AddMediatR(assemblyMarkerCore, assemblyMarkerData);
            services.AddAutoMapper(assemblyMarkerCore, assemblyMarkerData);
            services.AddSchemaBuilders(assemblyMarkerData);

            services.AddSingleton<IAuthorizationHandler, CanAccessCartAuthorizationHandler>();
            services.AddTransient<ICartAggregateRepository, CartAggregateRepository>();
            services.AddTransient<ICartValidationContextFactory, CartValidationContextFactory>();
            services.AddTransient<ICartAvailMethodsService, CartAvailMethodsService>();
            services.AddTransient<ICartProductService, CartProductService>();
            services.AddSingleton<ICartResponseGroupParser, CartResponseGroupParser>();
            services.AddTransient<CartAggregate>();
            services.AddTransient<Func<CartAggregate>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<CartAggregate>());

            services.AddPipeline<SearchProductResponse>(builder =>
            {
                builder.AddMiddleware(typeof(EvalProductsWishlistsMiddleware));
            });

            services.AddPipeline<PromotionEvaluationContext>(builder =>
            {
                builder.AddMiddleware(typeof(LoadCartToEvalContextMiddleware));
            });
            services.AddPipeline<TaxEvaluationContext>(builder =>
            {
                builder.AddMiddleware(typeof(LoadCartToEvalContextMiddleware));
            });
            services.AddPipeline<PriceEvaluationContext>(builder =>
            {
                builder.AddMiddleware(typeof(LoadCartToEvalContextMiddleware));
            });

            return services;
        }
    }
}
