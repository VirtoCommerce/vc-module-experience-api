using GraphQL.DataLoader;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Data.GraphQL.Schemas;

namespace VirtoCommerce.ExperienceApiModule.Extension.GraphQL.Schemas
{
    public class ProductType2 : ProductType
    {
        public ProductType2(
            IMediator mediator,
            IDataLoaderContextAccessor dataLoader)
            :base(mediator, dataLoader)
        {
            Field(d => ((CatalogProduct2)d).Price, nullable: true).Description("The product price");
            Field(d => ((CatalogProduct2)d).Currency).Description("The product price currency");
        }
    }
}
