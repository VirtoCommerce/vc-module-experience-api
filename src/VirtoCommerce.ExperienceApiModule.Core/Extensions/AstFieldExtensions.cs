using System.Collections.Generic;
using System.Linq;
using GraphQL.Language.AST;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Aliases;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class AstFieldExtensions
    {
        public static string[] GetAllNodesPaths(this IResolveFieldContext context)
        {
            var currentRootType = GetNestedGraphType(context.ReturnType);

            return context.SubFields.Values.SelectMany(x => x.GetAllTreeNodesPaths(currentRootType)).ToArray();
        }

        private static string[] GetAllTreeNodesPaths(this INode node, IComplexGraphType type, string path = null)
        {
            var rootPrefix = "__object.";
            var pathes = new List<string>();

            var aliasesMap = type.Fields
                .Where(x => x.HasAliasContainer())
                .ToDictionary(x => x.Name, x => x.GetAliasContainer());

            var isRoot = path == null;

            if (node is Field field)
            {
                var fieldName = field.Name;

                if (aliasesMap.TryGetValue(fieldName, out var aliaseContainer))
                {
                    foreach (var alias in aliaseContainer)
                    {
                        if (alias is RootAlias rootAlias)
                        {
                            pathes.Add(alias.Value);
                        }
                        if (alias is InnerAlias innerAlias)
                        {
                            fieldName = alias.Value;
                        }
                    }
                }

                path = isRoot ? $"{rootPrefix}{fieldName}" : string.Join(".", path, fieldName);
            }

            if (node.Children != null)
            {
                pathes.AddRange(node.Children.SelectMany(childNode => (childNode, node) switch
                {
                    (Field childField, Field rootField) => childNode.GetAllTreeNodesPaths(type.GetNestedGraphType(rootField.Name), path),

                    (SelectionSet subSelection, Field rootField)
                        when subSelection.Children.Any()
                            => childNode.Children.SelectMany(subSelect => subSelect.GetAllTreeNodesPaths(type.GetNestedGraphType(rootField.Name), path)),

                    _ => new[] { path }
                }));
            }
            else
            {
                pathes.Add(path);
            }

            return pathes.ToArray();
        }

        private static IComplexGraphType GetNestedGraphType(this IComplexGraphType type, string name)
        {
            var fieldType = type.Fields.FirstOrDefault(field => field.Name == name);

            return GetNestedGraphType(fieldType.ResolvedType);
        }

        private static IComplexGraphType GetNestedGraphType(this IGraphType type)
        {
            if (type is ListGraphType listGraphType)
            {
                return listGraphType.ResolvedType as IComplexGraphType;
            }

            return type as IComplexGraphType;
        }
    }
}
