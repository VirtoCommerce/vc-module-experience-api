using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common
{
    public class CloneableValueObject : ValueObject, ICloneable
    {
        public virtual object Clone() => MemberwiseClone();
    }
}
