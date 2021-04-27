using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.XProfile.Commands;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Extensions
{
    public static class MemberExtensions
    {
        /// <summary>
        /// Load the dynamic property values for member. Include empty meta-data for missing values.
        /// </summary>
        /// <returns>Loaded Dynamic Property Values for specified member</returns>
        internal static async Task<IEnumerable<DynamicPropertyObjectValue>> LoadMemberDynamicPropertyValues(this Member member, IDynamicPropertySearchService dynamicPropertySearchService, string cultureName)
        {
            // actual values
            var result = member.DynamicProperties.SelectMany(x => x.Values);

            if (!cultureName.IsNullOrEmpty())
            {
                result = result.Where(x => x.Locale.IsNullOrEmpty() || x.Locale.EqualsInvariant(cultureName));
            }

            // find and add all the properties without values
            var criteria = AbstractTypeFactory<DynamicPropertySearchCriteria>.TryCreateInstance();
            criteria.ObjectType = member.ObjectType;
            criteria.Take = int.MaxValue;
            var searchResult = await dynamicPropertySearchService.SearchDynamicPropertiesAsync(criteria);

            var propertiesWithoutValue = searchResult.Results.Where(prop => member.DynamicProperties.All(x => x.Id != prop.Id));
            var emptyValues = propertiesWithoutValue.Select(x =>
            {
                var newValue = AbstractTypeFactory<DynamicPropertyObjectValue>.TryCreateInstance();
                newValue.ObjectId = member.Id;
                newValue.ObjectType = member.ObjectType;
                newValue.PropertyId = x.Id;
                newValue.PropertyName = x.Name;
                newValue.ValueType = x.ValueType;
                return newValue;
            });

            return result.Union(emptyValues);
        }


        public static T GetSearchMembersQuery<T>(this IResolveConnectionContext context) where T : SearchMembersQueryBase
        {
            int.TryParse(context.After, out var skip);

            var result = AbstractTypeFactory<T>.TryCreateInstance();
            result.Filter = context.GetArgument<string>("filter");
            result.Sort = context.GetArgument<string>("sort");
            result.Skip = skip;
            result.Take = context.First ?? context.PageSize ?? 20;
            return result;
        }
    }
}
