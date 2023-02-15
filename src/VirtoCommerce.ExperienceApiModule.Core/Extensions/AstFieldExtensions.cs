using System.Collections.Generic;
using System.Linq;
using GraphQL;
using GraphQL.Execution;
using GraphQL.Language.AST;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class AstFieldExtensions
    {
        public static IEnumerable<string> GetAllNodesPaths(this IEnumerable<Field> fields)
        {
            return GetAllNodesPaths(fields, context: null);
        }

        public static IEnumerable<string> GetAllNodesPaths(this IEnumerable<Field> fields, IResolveFieldContext context)
        {
            return fields.SelectMany(x => x.GetAllTreeNodesPaths(context)).Distinct();
        }

        private static IEnumerable<string> GetAllTreeNodesPaths(this INode node, IResolveFieldContext context, string path = null)
        {
            if (node is Field field)
            {
                path = path != null ? string.Join(".", path, field.Name) : field.Name;
            }
            if (node.Children != null)
            {
                var childrenPaths = node.Children.Where(n => context != null && ShouldIncludeNode(context, n))
                    .SelectMany(n => n.GetAllTreeNodesPaths(context, path));
                foreach (var childPath in childrenPaths.DefaultIfEmpty(path))
                {
                    yield return childPath;
                }
            }
            else
            {
                yield return path;
            }
        }

        private static bool ShouldIncludeNode(IResolveFieldContext context, INode node)
        {
            var directives = node is IHaveDirectives haveDirectives ? haveDirectives.Directives : null;

            if (directives != null)
            {
                var directive = directives.Find(context.Schema.Directives.Skip.Name);
                if (directive != null)
                {
                    var arg = context.Schema.Directives.Skip.Arguments!.Find("if")!;
                    var value = ExecutionHelper.CoerceValue(arg.ResolvedType!, directive.Arguments?.ValueFor(arg.Name), context.Variables, arg.DefaultValue).Value;
                    if (value is true)
                    {
                        return false;
                    }
                }

                directive = directives.Find(context.Schema.Directives.Include.Name);
                if (directive != null)
                {
                    var arg = context.Schema.Directives.Include.Arguments!.Find("if")!;
                    var value = ExecutionHelper.CoerceValue(arg.ResolvedType!, directive.Arguments?.ValueFor(arg.Name), context.Variables, arg.DefaultValue).Value;

                    return value is true;
                }
            }

            return true;
        }
    }
}
