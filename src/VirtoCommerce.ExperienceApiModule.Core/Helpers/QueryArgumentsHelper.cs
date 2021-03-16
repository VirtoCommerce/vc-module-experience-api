using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.Core.Helpers
{
    public static class QueryArgumentsHelper
    {
        public static QueryArguments GetComplexTypeQueryArguments<TGraphType>(string name) where TGraphType : IGraphType
        {
            return new QueryArguments(new QueryArgument(GraphTypeExtenstionHelper.GetComplexType<TGraphType>())
            {
                Name = name
            });
        }

        public static QueryArguments GetQueryArguments<TGraphType>(string name) where TGraphType : IGraphType
        {
            return new QueryArguments(new QueryArgument(GraphTypeExtenstionHelper.GetActualType<TGraphType>())
            {
                Name = name
            });
        }
    }
}
