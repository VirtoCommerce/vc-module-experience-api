using System;
using System.Threading.Tasks;
using GraphQL.Language.AST;
using GraphQL.Types;
using GraphQL.Validation;
using GraphQL.Validation.Errors;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Validation
{
    public class CustomKnownArgumentNames : IValidationRule
    {
        private readonly INodeVisitor _nodeVisitor;

        public CustomKnownArgumentNames()
        {
            _nodeVisitor = new NodeVisitors(new MatchingNodeVisitor<Argument>(Validate));
        }

        public virtual Task<INodeVisitor> ValidateAsync(ValidationContext context)
        {
            return Task.FromResult(_nodeVisitor);
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
                                var fieldClone = new FieldType { Name = field.Name };
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
