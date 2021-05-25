using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.Core.Helpers
{
    public static class QueryArgumentPresets
    {
        public static QueryArguments ArgumentsForMoney() => new QueryArguments(
            new QueryArgument<StringGraphType> { Name = Constants.CurrencyCode },
            new QueryArgument<StringGraphType> { Name = Constants.CultureName }
        );

        public static QueryArguments GetArgumentForDynamicProperties()
        {
            return new QueryArguments(new QueryArgument<StringGraphType>
            {
                Name = "cultureName",
                Description = "Filter multilingual dynamic properties to return only values of specified language (\"en-US\")"
            });
        }
    }
}
