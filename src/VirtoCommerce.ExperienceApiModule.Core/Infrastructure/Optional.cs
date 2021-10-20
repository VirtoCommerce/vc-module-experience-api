namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
{
    public sealed class Optional<T>
    {
        public Optional()
        {
        }

        public Optional(T value, bool isSpecified)
        {
            Value = value;
            IsSpecified = isSpecified;
        }

        public Optional(object value)
        {
            IsSpecified = true;

            if (value != null)
            {
                Value = (T)value;
            }
        }

        public T Value { get; set; }

        public bool IsSpecified { get; set; }
    }
}
