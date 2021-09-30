using GraphQL;
using GraphQL.Builders;
using VirtoCommerce.ExperienceApiModule.XProfile.Commands;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Extensions
{
    public static class MemberExtensions
    {
        public static T GetSearchMembersQuery<T>(this IResolveConnectionContext context) where T : SearchMembersQueryBase
        {
            int.TryParse(context.After, out var skip);

            var result = AbstractTypeFactory<T>.TryCreateInstance();
            result.Keyword = context.GetArgument<string>("searchPhrase");
            result.Sort = context.GetArgument<string>("sort");
            result.Skip = skip;
            result.Take = context.First ?? context.PageSize ?? 20;

            return result;
        }
    }
}
