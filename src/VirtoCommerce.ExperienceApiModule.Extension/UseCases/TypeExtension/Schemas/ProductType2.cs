using System.Linq;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Data.GraphQL.Schemas;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Extension.GraphQL.Schemas
{
    public class ProductType2 : ProductType
    {
        public ProductType2(
            IMediator mediator,
            IDataLoaderContextAccessor dataLoader)
            : base(mediator, dataLoader)
        {
  
            Field<ListGraphType<PriceType>>("prices", arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "currency", Description = "currency" }), resolve: context =>
            {
                var result = ((CatalogProduct2)context.Source).Prices;
                var currency = context.GetArgument<string>("currency");
                if (currency != null)
                {
                    result = result.Where(x => x.Currency.EqualsInvariant(currency)).ToList();
                }
                return result;
            });
        }
    }
}
