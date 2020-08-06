using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Aliases;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class MetaDataProviderExtensions
    {
        public static IProvideMetadata RootAlias(this IProvideMetadata provider, params string[] values)
        {
            if (!provider.TryGetAliasContainer(out var aliasContainer))
            {
                aliasContainer = new AliasContainer();
                provider.Metadata.Add("aliases", aliasContainer);
            }

            aliasContainer.AddRootAliases(values);

            return provider;
        }

        public static IProvideMetadata InnerAlias(this IProvideMetadata provider, string value)
        {
            if (!provider.TryGetAliasContainer(out var aliasContainer))
            {
                aliasContainer = new AliasContainer();
                provider.Metadata.Add("aliases", aliasContainer);
            }

            aliasContainer.SetInnerAlias(value);

            return provider;
        }

        public static AliasContainer GetAliasContainer(this IProvideMetadata provider)
            => provider.Metadata.TryGetValue("aliases", out var boxedAliasСontainer)
            && boxedAliasСontainer is AliasContainer aliasContainer ? aliasContainer : null;

        public static bool HasAliasContainer(this IProvideMetadata provider)
            => TryGetAliasContainer(provider, out _);

        public static bool TryGetAliasContainer(this IProvideMetadata provider, out AliasContainer aliasContainer)
        {
            aliasContainer = provider.GetAliasContainer();
            return aliasContainer != null;
        }
    }
}
