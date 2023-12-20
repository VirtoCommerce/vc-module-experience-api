using System.Threading.Tasks;
using GraphQL.Language.AST;
using GraphQL.Validation;
using GraphQL.Validation.Errors;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Validation
{
    public class CustomKnownTypeNames : IValidationRule
    {
        private readonly INodeVisitor _nodeVisitor;

        public CustomKnownTypeNames()
        {
            _nodeVisitor = new NodeVisitors(new MatchingNodeVisitor<NamedType>(Validate));
        }

        public virtual Task<INodeVisitor> ValidateAsync(ValidationContext context)
        {
            return Task.FromResult(_nodeVisitor);
        }

        protected virtual void Validate(NamedType node, ValidationContext context)
        {
            var type = context.Schema.AllTypes[node.Name];
            if (type == null)
            {
                context.ReportError(new KnownTypeNamesError(context, node, suggestedTypes: null));
            }
        }
    }
}
