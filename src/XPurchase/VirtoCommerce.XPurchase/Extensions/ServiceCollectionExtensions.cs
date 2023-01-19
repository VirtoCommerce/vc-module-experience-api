using System;
using System.Collections.Generic;
using AutoMapper;
using GraphQL.Server;
using GraphQL.Types;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.XDigitalCatalog;
using VirtoCommerce.XDigitalCatalog.Schemas;
using VirtoCommerce.XPurchase.Authorization;
using VirtoCommerce.XPurchase.Schemas;
using VirtoCommerce.XPurchase.Services;
using VirtoCommerce.XPurchase.Validators;

namespace VirtoCommerce.XPurchase.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXPurchase(this IServiceCollection services, IGraphQLBuilder graphQlbuilder)
        {
            graphQlbuilder.AddGraphTypes(typeof(XPurchaseAnchor));

            services.AddSchemaBuilder<PurchaseSchema>();
            services.AddSingleton<IAuthorizationHandler, CanAccessCartAuthorizationHandler>();
            services.AddTransient<ICartAggregateRepository, CartAggregateRepository>();

            services.AddTransient<ICartValidationContextFactory, CartValidationContextFactory>();
            services.AddTransient<ICartAvailMethodsService, CartAvailMethodsService>();

            services.AddMediatR(typeof(XPurchaseAnchor));

            services.AddTransient<ICartProductService, CartProductService>();

            services.AddTransient<CartAggregate>();
            services.AddTransient<Func<CartAggregate>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<CartAggregate>());

            services.AddAutoMapper(typeof(XPurchaseAnchor));

            return services;
        }

        public static IServiceCollection AddXPurchase(this IServiceCollection services, IGraphQLBuilder graphQlbuilder, ExtentionFieldsContainer fieldsContainer)
        {
            services.AddXPurchase(graphQlbuilder);

            // try to register extra fields
            var productTypeName = nameof(ProductType);
            if (!fieldsContainer.Fields.ContainsKey(productTypeName))
            {
                fieldsContainer.Fields[productTypeName] = new List<FieldType>();
            }

            // simple field with source resolver
            var extentionField = fieldsContainer.CreateField<ExpProduct, StringGraphType>(
                name: "extendableField",
                resolve: context => context.Source.IndexedProduct.Id);
            fieldsContainer.Fields[productTypeName].Add(extentionField);

            // async field with service resolver
            var extentionField2 = fieldsContainer.CreateFieldAsync<object, ListGraphType<VirtoCommerce.ExperienceApiModule.Core.Schemas.DynamicPropertyValueType>>(
                name: "extendableField2",
                resolve: async context =>
                {
                    //using var scope = context.RequestServices.CreateScope();
                    var dynamicPropService = context.RequestServices.GetRequiredService<IDynamicPropertyResolverService>();
                    var test = new LineItem
                    {
                        DynamicProperties = new List<DynamicObjectProperty>(),
                    };
                    var res = await dynamicPropService.LoadDynamicPropertyValues(test, "en-US");

                    return res;
                });
            fieldsContainer.Fields[productTypeName].Add(extentionField2);

            return services;
        }
    }
}
