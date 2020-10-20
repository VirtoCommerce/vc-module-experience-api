using GraphQL.Types;
using VirtoCommerce.Exp.ExtensionSamples.UseCases.TypeExtension;

namespace VirtoCommerce.Exp.ExtensionSamples
{
    public class ProductRatingType : ObjectGraphType<ProductRating>
    {
        public ProductRatingType()
        {
            Field(d => d.Rating, nullable: true).Description("product rating");
           
        }
     
    }
}
