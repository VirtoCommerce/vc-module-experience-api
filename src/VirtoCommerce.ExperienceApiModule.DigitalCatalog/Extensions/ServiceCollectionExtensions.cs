using GraphQL.Server;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.XDigitalCatalog.Middlewares;
using VirtoCommerce.XDigitalCatalog.Queries;
using VirtoCommerce.XDigitalCatalog.Schemas;
using AutoMapper;
using VirtoCommerce.XDigitalCatalog.Services;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using Microsoft.Extensions.Options;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XDigitalCatalog.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXCatalog(this IServiceCollection services, IGraphQLBuilder graphQlbuilder)
        {
            services.AddSchemaBuilder<DigitalCatalogSchema>();

            graphQlbuilder.AddGraphTypes(typeof(XDigitalCatalogAnchor));
            
            services.AddMediatR(typeof(XDigitalCatalogAnchor));
            //the generic pipeline that is used  for on-the-fly additional data evaluation (prices, inventories, discounts and taxes) for resulting products 
            services.AddPipeline<SearchProductResponse>(builder =>
            {
                builder.AddMiddleware(typeof(EvalProductsPricesMiddleware));
                builder.AddMiddleware(typeof(EvalProductsDiscountsMiddleware));
                builder.AddMiddleware(typeof(EvalProductsTaxMiddleware));
                builder.AddMiddleware(typeof(EvalProductsInventoryMiddleware));
            });

            services.AddAutoMapper(typeof(XDigitalCatalogAnchor));

            services.AddTransient<IService, ExpProductAssociationSearchService>();
            services.AddTransient<IService, ExpProductAssociationSearchServiceNswag>();
            services.AddService(typeof(IExpProductAssociationSearchService));

            //TODO
            //var serv = services.BuildServiceProvider().GetService<IExpProductAssociationSearchService>();

            return services;
        }
    }
}
