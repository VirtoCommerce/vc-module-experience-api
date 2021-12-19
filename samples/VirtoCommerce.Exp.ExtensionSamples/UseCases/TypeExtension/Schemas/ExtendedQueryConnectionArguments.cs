using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.XOrder.Schemas;

namespace VirtoCommerce.Exp.ExtensionSamples.UseCases.TypeExtension.Schemas
{
    public class ExtendedQueryConnectionArguments : QueryConnectionArguments
    {
        public ExtendedQueryConnectionArguments()
        {
            Argument<StringGraphType>("test");
        }
    }
}
