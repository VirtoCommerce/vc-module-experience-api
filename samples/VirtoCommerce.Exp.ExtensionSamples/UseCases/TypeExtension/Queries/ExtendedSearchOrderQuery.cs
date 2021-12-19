using GraphQL;
using GraphQL.Builders;
using VirtoCommerce.ExperienceApiModule.XOrder.Queries;

namespace VirtoCommerce.Exp.ExtensionSamples.UseCases.TypeExtension.Queries
{
    public class ExtendedSearchOrderQuery : SearchOrderQuery
    {
        public string Test { get; set; }

        public override void Map(IResolveConnectionContext<object> context)
        {
            base.Map(context);
            Test = context.GetArgument<string>("test");
        }
    }
}
