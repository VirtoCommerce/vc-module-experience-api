using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Extensions
{
    public static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Type, string> PrettyPrintCache =
            new ConcurrentDictionary<Type, string>();

        private static readonly ConcurrentDictionary<Type, string> TypeCacheKeys =
            new ConcurrentDictionary<Type, string>();

        public static string GetCacheKey(this Type type) => TypeCacheKeys.GetOrAdd(type, t => $"{t.PrettyPrint()}");

        public static Type[] GetTypeInheritanceChainTo(this Type type, Type toBaseType)
        {
            var result = new List<Type>
            {
                type,
            };

            var baseType = type.BaseType;

            while (baseType != toBaseType && baseType != typeof(object))
            {
                result.Add(baseType);
                baseType = baseType.BaseType;
            }

            return result.ToArray();
        }

        public static string PrettyPrint(this Type type) => PrettyPrintCache.GetOrAdd(type, t =>
        {
            try
            {
                return PrettyPrintRecursive(t, 0);
            }
            catch (Exception)
            {
                return t.Name;
            }
        });

        public static Money CloneAsMoney(this ICloneable value) => value?.Clone() as Money;

        public static Currency CloneAsCurrency(this ICloneable value) => value?.Clone() as Currency;

        private static string PrettyPrintRecursive(Type type, int depth)
        {
            if (depth > 3)
            {
                return type.Name;
            }

            var nameParts = type.Name.Split('`');

            if (nameParts.Length == 1)
            {
                return nameParts[0];
            }

            var genericArguments = type.GetTypeInfo().GetGenericArguments();
            return !type.IsConstructedGenericType
                       ? $"{nameParts[0]}<{new string(',', genericArguments.Length - 1)}>"
                       : $"{nameParts[0]}<{string.Join(",", genericArguments.Select(t => PrettyPrintRecursive(t, depth + 1)))}>";
        }
    }
}
