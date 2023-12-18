using System.Threading.Tasks;
using GraphQL.Language.AST;
using GraphQL.Validation;
using GraphQL.Validation.Errors;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Validation
{
    public class CustomKnownTypeNames : IValidationRule
    {
        public virtual async Task<INodeVisitor> ValidateAsync(ValidationContext context)
        {
            var visitor = new NodeVisitors(new MatchingNodeVisitor<NamedType>((node, context) => Validate(node, context)));

            return await Task.FromResult(visitor);
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
