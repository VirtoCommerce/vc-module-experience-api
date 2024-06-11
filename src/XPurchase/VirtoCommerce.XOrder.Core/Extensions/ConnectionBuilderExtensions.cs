using GraphQL.Builders;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XOrder.Core.Schemas;

namespace VirtoCommerce.XOrder.Core.Extensions
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
