using System;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;
using DynamicProperty = VirtoCommerce.ExperienceApiModule.XPurchase.Models.DynamicProperty;
using DynamicPropertyDictionaryItem = VirtoCommerce.ExperienceApiModule.XPurchase.Models.DynamicPropertyDictionaryItem;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Converters
{
    public static class DynamicPropertyConverter
    {
        public static DynamicProperty ToDynamicProperty(this Platform.Core.DynamicProperties.DynamicObjectProperty propertyDto)
        {
            var result = new DynamicProperty
            {
                Id = propertyDto.Id,
                IsArray = propertyDto.IsArray,
                IsDictionary = propertyDto.IsDictionary,
                IsRequired = propertyDto.IsRequired,
                Name = propertyDto.Name,
                ValueType = propertyDto.ValueType.ToString(),
                DisplayNames = propertyDto?.DisplayNames
                    .Select(x => new LocalizedString(new Language(x.Locale), x.Name))
                    .ToList()
            };

            if (propertyDto.Values != null)
            {
                if (result.IsDictionary)
                {
                    var dictValues = propertyDto.Values
                        .Where(x => x.Value != null)
                        .Select(x => x.Value)
                        .Cast<JObject>()
                        .Select(x => x.ToObject<Platform.Core.DynamicProperties.DynamicPropertyDictionaryItem>())
                        .ToArray();

                    result.DictionaryValues = dictValues.Select(x => x.ToDictItem()).ToList();
                }
                else
                {
                    result.Values = propertyDto.Values
                        .Where(x => x.Value != null)
                        .Select(x => x.ToLocalizedString())
                        .ToList();
                }
            }

            return result;
        }

        public static Platform.Core.DynamicProperties.DynamicObjectProperty ToDynamicPropertyDto(this DynamicProperty dynamicProperty)
            => new Platform.Core.DynamicProperties.DynamicObjectProperty
            {
                Id = dynamicProperty.Id,
                IsArray = dynamicProperty.IsArray,
                IsDictionary = dynamicProperty.IsDictionary,
                IsRequired = dynamicProperty.IsRequired,
                Name = dynamicProperty.Name,

                ValueType = Enum.TryParse<Platform.Core.DynamicProperties.DynamicPropertyValueType>(dynamicProperty.ValueType, out var valueType)
                        ? valueType
                        : Platform.Core.DynamicProperties.DynamicPropertyValueType.Undefined,

                Values = dynamicProperty?.Values?.Select(x => x.ToPropertyValueDto()).ToList()
                      ?? dynamicProperty?.DictionaryValues?.Select(v => v.ToPropertyValueDto()).ToList(),
            };

        private static DynamicPropertyDictionaryItem ToDictItem(this Platform.Core.DynamicProperties.DynamicPropertyDictionaryItem dto)
            => new DynamicPropertyDictionaryItem
            {
                Id = dto.Id,
                Name = dto.Name,
                PropertyId = dto.PropertyId,
                DisplayNames = dto.DisplayNames?.Select(x => new LocalizedString(new Language(x.Locale), x.Name)).ToList()
            };

        private static LocalizedString ToLocalizedString(this Platform.Core.DynamicProperties.DynamicPropertyObjectValue dto)
            => new LocalizedString(new Language(dto.Locale), string.Format(CultureInfo.InvariantCulture, "{0}", dto.Value));

        private static Platform.Core.DynamicProperties.DynamicPropertyObjectValue ToPropertyValueDto(this DynamicPropertyDictionaryItem dictItem)
            => new Platform.Core.DynamicProperties.DynamicPropertyObjectValue
            {
                Value = dictItem
            };

        private static Platform.Core.DynamicProperties.DynamicPropertyObjectValue ToPropertyValueDto(this LocalizedString dynamicPropertyObjectValue)
            => new Platform.Core.DynamicProperties.DynamicPropertyObjectValue
            {
                Value = dynamicPropertyObjectValue.Value,
                Locale = dynamicPropertyObjectValue.Language.CultureName
            };
    }
}
