using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common.Caching;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Extensions;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common
{
    public abstract class ValueObject : IValueObject, ICacheKey, ICloneable
    {
        private static readonly ConcurrentDictionary<Type, IReadOnlyCollection<PropertyInfo>> TypeProperties
            = new ConcurrentDictionary<Type, IReadOnlyCollection<PropertyInfo>>();

        private const string NullText = "null";

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj is null || GetType() != obj.GetType())
                return false;

            var other = obj as ValueObject;

            return other != null && GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return GetEqualityComponents().Aggregate(17, (current, obj) => current * 23 + (obj?.GetHashCode() ?? 0));
            }
        }

        public static bool operator ==(ValueObject left, ValueObject right) => Equals(left, right);

        public static bool operator !=(ValueObject left, ValueObject right) => !Equals(left, right);

        public override string ToString() => $"{{{string.Join(", ", GetProperties().Select(f => $"{f.Name}: {f.GetValue(this)}"))}}}";

        public virtual string GetCacheKey()
            => string.Join("|", GetEqualityComponents()
                .Select(x => x ?? NullText)
                .Select(x => x is ICacheKey cacheKey
                    ? cacheKey.GetCacheKey()
                    : x.ToString()));

        protected virtual IEnumerable<object> GetEqualityComponents()
        {
            foreach (var property in GetProperties())
            {
                var value = property.GetValue(this);
                if (value == null)
                {
                    yield return NullText;
                }
                else
                {
                    var valueType = value.GetType();
                    if (valueType.IsAssignableFromGenericList())
                    {
                        foreach (var child in (IEnumerable)value)
                        {
                            yield return child ?? NullText;
                        }
                    }
                    else
                    {
                        yield return value;
                    }
                }
            }
        }

        protected virtual IEnumerable<PropertyInfo> GetProperties() => TypeProperties.GetOrAdd(GetType(),
            t => t.GetTypeInfo().GetProperties(BindingFlags.Instance | BindingFlags.Public).OrderBy(p => p.Name).ToList());

        #region ICloneable members

        public virtual object Clone() => MemberwiseClone();

        #endregion ICloneable members
    }
}
