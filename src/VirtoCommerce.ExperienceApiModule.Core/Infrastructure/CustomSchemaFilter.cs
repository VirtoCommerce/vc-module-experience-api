using System.Threading.Tasks;
using GraphQL.Introspection;
using GraphQL.Types;
using Microsoft.Extensions.Options;
using VirtoCommerce.ExperienceApiModule.Core.Models;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
{
    public sealed class CustomSchemaFilter : ISchemaFilter
    {
        private readonly Task<bool> _isAllowed;

        public CustomSchemaFilter(IOptions<GraphQLPlaygroundOptions> playgroundOptions)
        {
            _isAllowed = Task.FromResult(playgroundOptions.Value.Enable);
        }

        public Task<bool> AllowType(IGraphType type) => _isAllowed;

        public Task<bool> AllowDirective(DirectiveGraphType directive) => _isAllowed;

        public Task<bool> AllowArgument(IFieldType field, QueryArgument argument) => _isAllowed;

        public Task<bool> AllowEnumValue(EnumerationGraphType parent, EnumValueDefinition enumValue) => _isAllowed;

        public Task<bool> AllowField(IGraphType parent, IFieldType field) => _isAllowed;
    }
}
