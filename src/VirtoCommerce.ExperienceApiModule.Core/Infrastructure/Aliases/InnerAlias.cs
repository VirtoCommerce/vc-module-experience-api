namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Aliases
{
    public sealed class InnerAlias : AliasBase
    {
        public override AliasInfoType Type => AliasInfoType.Inner;

        public InnerAlias(string value) : base(value)
        {
        }
    }
}
