using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.XProfile.Schemas;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Aggregates
{
    public interface IMemberAggregateRoot
    {
        public Member Member { get; set; }

        MemberAggregateRootBase UpdateAddresses(IList<Address> addresses);
        MemberAggregateRootBase UpdateDynamicProperties(IList<DynamicPropertyValue> values, IDynamicPropertyMetaDataResolver metaDataResolver, IDynamicPropertyDictionaryItemsSearchService dynamicPropertyDictionaryItemsSearchService);
    }


    public abstract class MemberAggregateRootBase : IMemberAggregateRoot
    {
        public virtual Member Member { get; set; }

        public virtual MemberAggregateRootBase UpdateAddresses(IList<Address> addresses)
        {
            Member.Addresses = addresses.ToList();

            return this;
        }

        public virtual MemberAggregateRootBase UpdateDynamicProperties(IList<DynamicPropertyValue> values, IDynamicPropertyMetaDataResolver metaDataResolver, IDynamicPropertyDictionaryItemsSearchService dynamicPropertyDictionaryItemsSearchService)
        {
            var memberDynamicProperties = Member.DynamicProperties.ToList();

            foreach (var propValueGroup in values.GroupBy(x => x.Name))
            {
                var propertyValue = memberDynamicProperties.FirstOrDefault(x => x.Name.EqualsInvariant(propValueGroup.Key));
                if (propertyValue is null)
                {
                    propertyValue = new DynamicObjectProperty { Name = propValueGroup.Key };
                    memberDynamicProperties.Add(propertyValue);
                }

                var metadata = metaDataResolver.GetByNameAsync(Member.ObjectType, propertyValue.Name).GetAwaiter().GetResult();
                if (metadata != null)
                {
                    propertyValue.SetMetaData(metadata);
                }

                // override all the values of a dictionary property. Need to set the correct ValueId.
                if (propertyValue.IsDictionary)
                {
                    var dictionaryItems = dynamicPropertyDictionaryItemsSearchService.SearchDictionaryItemsAsync(
                        new DynamicPropertyDictionaryItemSearchCriteria { PropertyId = propertyValue.Id, ObjectType = Member.ObjectType })
                        .GetAwaiter().GetResult()
                        .Results;

                    foreach (var propValue in propValueGroup.Where(x => x.Value is string))
                    {
                        var dictionaryItem = dictionaryItems.FirstOrDefault(x => x.Id == (string)propValue.Value || x.Name == (string)propValue.Value);
                        if (dictionaryItem != null)
                        {
                            propValue.ValueId = dictionaryItem.Id;
                        }
                    }
                }

                // override only values of a specific locale for multilingual property. Except dictionary properties.
                if (propertyValue.IsMultilingual && !propertyValue.IsDictionary)
                {
                    foreach (var propValue in propValueGroup)
                    {
                        var multilingualValue = propertyValue.Values.FirstOrDefault(x => x.Locale.EqualsInvariant(propValue.Locale));
                        if (multilingualValue is null)
                        {
                            multilingualValue = propValue;
                            propertyValue.Values = propertyValue.Values.Union(new[] { multilingualValue }).ToArray();
                        }
                        else
                        {
                            multilingualValue.Value = propValue.Value;
                        }
                    }
                }
                else
                {
                    propertyValue.Values = propValueGroup.ToArray();
                }
            }

            Member.DynamicProperties = memberDynamicProperties;

            return this;
        }
    }
}
