using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
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
        internal static async Task<IEnumerable<DynamicPropertyObjectValue>> LoadMemberDynamicPropertyValues(this Member member, IDynamicPropertySearchService dynamicPropertySearchService)
        {
            // actual values
            var result = member.DynamicProperties.SelectMany(x => x.Values);

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
    }
}
