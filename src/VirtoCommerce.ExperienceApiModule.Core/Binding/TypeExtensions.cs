using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace VirtoCommerce.ExperienceApiModule.Core.Binding
{
    public static class TypeExtensions
    {

        private static ConcurrentDictionary<Type, IIndexModelBinder> _bindersCache = new ConcurrentDictionary<Type, IIndexModelBinder>();

        public static IIndexModelBinder GetIndexModelBinder(this Type type, IIndexModelBinder defaultBinder)
        {
            var result = defaultBinder;
            var bindAttr = type.GetCustomAttributes<BindIndexFieldAttribute>().FirstOrDefault();

            if (bindAttr != null)
            {
                result = GetBinder(bindAttr);
            }
            if (result != null)
            {
                result.BindingInfo = type.GetBindingInfo() ?? result.BindingInfo;
            }
            return result;
        }

        public static IIndexModelBinder GetIndexModelBinder(this PropertyInfo propInfo)
        {
            IIndexModelBinder result = null;
            var bindAttr = propInfo.GetCustomAttributes<BindIndexFieldAttribute>().FirstOrDefault();

            if (bindAttr != null)
            {
                result = GetBinder(bindAttr);
            }
            if (result != null)
            {
                result.BindingInfo = propInfo.GetBindingInfo() ?? result.BindingInfo;
            }
            return result;
        }

        private static BindingInfo GetBindingInfo(this PropertyInfo propInfo)
        {
            BindingInfo result = null;
            var bindAttr = propInfo.GetCustomAttributes<BindIndexFieldAttribute>().FirstOrDefault();

            if (bindAttr != null)
            {
                result = GetBindingInfo(bindAttr);
            }
            return result;
        }
        
        private static BindingInfo GetBindingInfo(this Type type)
        {
            BindingInfo result = null;
            var bindAttr = type.GetCustomAttributes<BindIndexFieldAttribute>().FirstOrDefault();

            if (bindAttr != null)
            {
                result = GetBindingInfo(bindAttr);
            }
            return result;
        }

        private static BindingInfo GetBindingInfo(BindIndexFieldAttribute attr)
        {
            return new BindingInfo
            {
                FieldName = attr.FieldName
            };
        }

        private static IIndexModelBinder GetBinder(BindIndexFieldAttribute attr)
        {
            var binderType = attr.BinderType;
            if (binderType == null)
            {
                binderType = typeof(DefaultPropertyIndexBinder);
            }
            return _bindersCache.GetOrAdd(binderType, type => Activator.CreateInstance(binderType) as IIndexModelBinder);
        }
    }
}
