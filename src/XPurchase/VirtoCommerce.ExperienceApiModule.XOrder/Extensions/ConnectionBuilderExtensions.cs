using GraphQL.Builders;
using VirtoCommerce.ExperienceApiModule.XOrder.Schemas;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Extensions
{
    public static class ConnectionBuilderExtensions
    {
        public static ConnectionBuilder<object> OrderArguments(this ConnectionBuilder<object> connectionBuilder)
        {
            connectionBuilder.FieldType.Arguments = AbstractTypeFactory<OrderQueryConnectionArguments>.TryCreateInstance().AddArguments(connectionBuilder.FieldType.Arguments);
            return connectionBuilder;
        }

        public static ConnectionBuilder<object> OrganizationOrderArguments(this ConnectionBuilder<object> connectionBuilder)
        {
            connectionBuilder.FieldType.Arguments = AbstractTypeFactory<OrganizationOrderQueryConnectionArguments>.TryCreateInstance().AddArguments(connectionBuilder.FieldType.Arguments);
            return connectionBuilder;
        }
    }
}
