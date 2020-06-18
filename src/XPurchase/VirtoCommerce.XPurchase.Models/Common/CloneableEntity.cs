using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase.Models.Common
{
    public abstract class CloneableEntity : CloneableValueObject, IEntity
    {
        public virtual string Id { get; set; }

        public bool IsTransient() => Id == null;

        protected override IEnumerable<object> GetEqualityComponents() => new List<object> { Id };
    }
}
