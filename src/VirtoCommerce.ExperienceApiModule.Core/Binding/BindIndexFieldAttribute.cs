using System;

namespace VirtoCommerce.ExperienceApiModule.Core.Binding
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class BindIndexFieldAttribute : Attribute
    {
        public virtual string FieldName { get; set; }
        private Type _binderType;
        public Type BinderType
        {
            get => _binderType;
            set
            {
                if (value != null && !typeof(IIndexModelBinder).IsAssignableFrom(value))
                {
                    throw new ArgumentException($"{ value.FullName }  must be is assignable from {typeof(IIndexModelBinder).FullName}");
                }

                _binderType = value;
            }
        }
    }
}
