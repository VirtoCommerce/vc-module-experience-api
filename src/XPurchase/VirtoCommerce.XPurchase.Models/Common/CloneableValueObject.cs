using System;

namespace VirtoCommerce.XPurchase.Models.Common
{
    public class CloneableValueObject : ValueObject, ICloneable
    {
        public virtual object Clone() => MemberwiseClone();
    }
}
