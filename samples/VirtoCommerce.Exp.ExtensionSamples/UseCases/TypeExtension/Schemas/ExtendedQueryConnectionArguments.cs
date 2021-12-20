using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.XOrder.Schemas;

namespace VirtoCommerce.Exp.ExtensionSamples.UseCases.TypeExtension.Schemas
{
    public class ExtendedOrderQueryConnectionArguments : OrderQueryConnectionArguments
    {
        public ExtendedOrderQueryConnectionArguments()
        {
            Argument<StringGraphType>("test");
        }
    }
}
