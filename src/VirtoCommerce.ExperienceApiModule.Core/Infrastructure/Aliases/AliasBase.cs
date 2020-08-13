namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Aliases
{
    public abstract class AliasBase
    {
        public string Value { get; private set; }
        public abstract AliasInfoType Type { get; }

        protected AliasBase(string value)
        {
            Value = value;
        }
    }
}
