using System;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common
{
    public class CloneableValueObject : ValueObject, ICloneable
    {
        public virtual object Clone() => MemberwiseClone();
    }
}
