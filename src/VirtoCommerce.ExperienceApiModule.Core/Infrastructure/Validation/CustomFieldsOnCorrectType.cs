using System.Threading.Tasks;
using GraphQL;
using GraphQL.Language.AST;
using GraphQL.Validation;
using GraphQL.Validation.Errors;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Validation
{
    public class CustomFieldsOnCorrectType : IValidationRule
    {
        private readonly INodeVisitor _nodeVisitor;

        public CustomFieldsOnCorrectType()
        {
            _nodeVisitor = new NodeVisitors(new MatchingNodeVisitor<Field>(Validate));
        }

        public virtual Task<INodeVisitor> ValidateAsync(ValidationContext context)
        {
            return Task.FromResult(_nodeVisitor);
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
