using GraphQL.Builders;
using VirtoCommerce.ExperienceApiModule.XOrder.Schemas;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Extensions
{
    public static class ConnectionBuilderExtensions
    {
        public static ConnectionBuilder<object> Arguments(this ConnectionBuilder<object> connectionBuilder)
        {
            connectionBuilder.FieldType.Arguments = AbstractTypeFactory<QueryConnectionArguments>.TryCreateInstance().AddArguments(connectionBuilder.FieldType.Arguments);
            return connectionBuilder;
        }
    }
}
