using System.Threading.Tasks;
using GraphQL.Introspection;
using GraphQL.Types;
using Microsoft.Extensions.Options;
using VirtoCommerce.ExperienceApiModule.Core.Models;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
{
    public sealed class CustomSchemaFilter : ISchemaFilter
    {
        private readonly Task<bool> IsAllowed;

        public CustomSchemaFilter(IOptions<GraphQLPlaygroundOptions> playgroundOptions)
        {
            IsAllowed = Task.FromResult(playgroundOptions.Value.Enable);
        }

        public Task<bool> AllowType(IGraphType type) => IsAllowed;

        public Task<bool> AllowDirective(DirectiveGraphType directive) => IsAllowed;

        public Task<bool> AllowArgument(IFieldType field, QueryArgument argument) => IsAllowed;

        public Task<bool> AllowEnumValue(EnumerationGraphType parent, EnumValueDefinition enumValue) => IsAllowed;

        public Task<bool> AllowField(IGraphType parent, IFieldType field) => IsAllowed;
    }
}
