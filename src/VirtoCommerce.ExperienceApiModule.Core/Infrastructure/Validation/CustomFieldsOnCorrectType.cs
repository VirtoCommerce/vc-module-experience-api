using System.Threading.Tasks;
using GraphQL;
using GraphQL.Language.AST;
using GraphQL.Validation;
using GraphQL.Validation.Errors;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Validation
{
    public class CustomFieldsOnCorrectType : IValidationRule
    {
        public virtual async Task<INodeVisitor> ValidateAsync(ValidationContext context)
        {
            var visitor = new NodeVisitors(new MatchingNodeVisitor<Field>((node, context) => Validate(node, context)));

            return await Task.FromResult(visitor);
        }

        protected virtual void Validate(Field node, ValidationContext context)
        {
            var type = context.TypeInfo.GetParentType()?.GetNamedType();
            if (type != null)
            {
                var field = context.TypeInfo.GetFieldDef();
                if (field == null)
                {
                    context.ReportError(new FieldsOnCorrectTypeError(context, node, type, suggestedTypeNames: null, suggestedFieldNames: null));
                }
            }
        }
    }
}
