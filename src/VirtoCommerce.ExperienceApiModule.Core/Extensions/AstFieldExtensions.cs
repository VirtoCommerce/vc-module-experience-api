using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL;
using GraphQL.Execution;
using GraphQL.Language.AST;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class AstFieldExtensions
    {
        [Obsolete("Use method with the 'context' argument")]
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

            // combine fragment nodes and other children nodes
            var combinedNodes = GetCombinedChildrenNodes(node, context);
            if (combinedNodes.Any())
            {
                var childrenPaths = combinedNodes.Where(n => context != null && ShouldIncludeNode(context, n))
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

        private static List<INode> GetCombinedChildrenNodes(INode node, IResolveFieldContext context)
        {
            var combinedNodes = new List<INode>();

            if (node.Children.IsNullOrEmpty())
            {
                return combinedNodes;
            }

            foreach (var child in node.Children)
            {
                if (child is FragmentSpread fragment)
                {
                    var fragmentDefenition = context?.Document?.Fragments?.FirstOrDefault(x => x.Name == fragment.Name);
                    if (fragmentDefenition?.Children != null)
                    {
                        combinedNodes.AddRange(fragmentDefenition.Children.Where(x => x is not NamedType));
                    }
                }
                else
                {
                    combinedNodes.Add(child);
                }
            }

            return combinedNodes;
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
