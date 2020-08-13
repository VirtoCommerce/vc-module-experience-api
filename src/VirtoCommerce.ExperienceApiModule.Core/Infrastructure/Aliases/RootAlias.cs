namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Aliases
{
    public sealed class RootAlias : AliasBase
    {
        public override AliasInfoType Type => AliasInfoType.Root;

        public RootAlias(string value) : base(value)
        {
        }
    }
}
