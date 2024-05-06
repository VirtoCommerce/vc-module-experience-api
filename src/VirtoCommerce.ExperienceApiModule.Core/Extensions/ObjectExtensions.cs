using System;
using Newtonsoft.Json.Linq;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static T JsonClone<T>(this T source)
        {
            var jObject = JObject.FromObject(source);
            var result = jObject.ToObject<T>();
            return result;
        }

        public static T JsonConvert<T>(this object source)
        {
            var jObject = JObject.FromObject(source);
            var result = jObject.ToObject<T>();
            return result;
        }

        public static TResult Convert<TSource, TResult>(this TSource source, Func<TSource, TResult> convert)
        {
            return convert(source);
        }
    }
}
