using System;
using System.Threading.Tasks;
using GraphQL.Language.AST;
using GraphQL.Types;
using GraphQL.Validation;
using GraphQL.Validation.Errors;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Validation
{
    public partial class CustomRules
    {
        public class CustomKnownArgumentNames : IValidationRule
        {
            public virtual async Task<INodeVisitor> ValidateAsync(ValidationContext context)
            {
                var visitor = new NodeVisitors(new MatchingNodeVisitor<Argument>((node, context) => Validate(node, context)));

                return await Task.FromResult(visitor);
            }

            protected virtual void Validate(Argument node, ValidationContext context)
            {
                var argument = context.TypeInfo.GetAncestor(2);
                switch (argument)
                {
                    case Field:
                        {
                            var field = context.TypeInfo.GetFieldDef();
                            if (field != null)
                            {
                                var fieldArgument = field.Arguments?.Find(node.Name);
                                if (fieldArgument == null)
                                {
                                    var fieldClone = new FieldType { Name = fieldArgument.Name };
                                    var parentType = context.TypeInfo.GetParentType() ?? throw new InvalidOperationException("Parent type must not be null.");
                                    context.ReportError(new KnownArgumentNamesError(context, node, fieldClone, parentType));
                                }
                            }

                            break;
                        }

                    case Directive:
                        {
                            var directive = context.TypeInfo.GetDirective();
                            if (directive != null)
                            {
                                var directiveArgument = directive.Arguments?.Find(node.Name);
                                if (directiveArgument == null)
                                {
                                    var directiveClone = new DirectiveGraphType(directive.Name, directive.Locations);
                                    context.ReportError(new KnownArgumentNamesError(context, node, directiveClone));
                                }
                            }

                            break;
                        }
                }
            }
        }
    }
}
