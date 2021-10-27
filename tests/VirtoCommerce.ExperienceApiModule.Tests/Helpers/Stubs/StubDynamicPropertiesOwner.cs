using System.Collections.Generic;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.ExperienceApiModule.Tests.Helpers.Stubs
{
    public class StubDynamicPropertiesOwner : IHasDynamicProperties
    {
        public StubDynamicPropertiesOwner()
        {

        }

        public StubDynamicPropertiesOwner(ICollection<DynamicObjectProperty> dynamicProperties)
        {
            DynamicProperties = dynamicProperties;
        }

        public string Id { get; set; }

        public string ObjectType => nameof(StubDynamicPropertiesOwner);

        public ICollection<DynamicObjectProperty> DynamicProperties { get; set; } = new List<DynamicObjectProperty>();
    }
}
