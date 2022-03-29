using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public abstract class ArgumentList : QueryArguments
    {
        protected virtual QueryArgument Argument<T>(string name, string description = null) where T : IGraphType
        {
            var argument = new QueryArgument<T> { Name = name, Description = description };
            Add(argument);
            return argument;
        }
    }
}
