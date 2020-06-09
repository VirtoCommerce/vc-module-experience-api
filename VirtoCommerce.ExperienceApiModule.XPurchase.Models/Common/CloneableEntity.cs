using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common
{
    public abstract class CloneableEntity : CloneableValueObject, IEntity
    {
        public virtual string Id { get; set; }

        public bool IsTransient()
        {
            return Id == null;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            return new List<object> { Id };
        }
    }
}
