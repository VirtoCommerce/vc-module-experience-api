using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.Core.Helpers
{
    public static class QueryArgumentPresets
    {
        public static QueryArguments ArgumentsForMoney() => new QueryArguments(
            new QueryArgument<StringGraphType> { Name = Constants.CurrencyCode },
            new QueryArgument<StringGraphType> { Name = Constants.CultureName }
        );
    }
}
