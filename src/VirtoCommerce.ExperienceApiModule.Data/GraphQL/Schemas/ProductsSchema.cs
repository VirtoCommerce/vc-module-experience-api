using GraphQL.Types;
using GraphQL.Utilities;
using System;

namespace VirtoCommerce.ExperienceApiModule.Data.GraphQL.Schemas
{
    public class ProductsSchema : Schema
    {
        public ProductsSchema(IServiceProvider provider)
            : base(provider)
        {
            Query = provider.GetRequiredService<ProductsQuery>();
        }
    }
}
