using System.Collections.Generic;
using System.Linq;
using GraphQL.Language.AST;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class AstFieldExtensions
    {
        public static IEnumerable<string> GetAllNodesPaths(this IEnumerable<Field> fields)
        {
            return fields.SelectMany(x => x.GetAllTreeNodesPaths()).Distinct();
        }

        private static IEnumerable<string> GetAllTreeNodesPaths(this INode node, string path = null)
        {
            if (node is Field field)
            {
                path = path != null ? string.Join(".", path, field.Name) : field.Name;
            }
            if (node.Children != null)
            {
                var childrenPaths = node.Children.SelectMany(n => n.GetAllTreeNodesPaths(path));
                foreach (var childPath in childrenPaths.Any() ? childrenPaths : new[] { path })
                {
                    yield return childPath;
                }
            }
            else
            {
                yield return path;
            }
        }
    }
}
