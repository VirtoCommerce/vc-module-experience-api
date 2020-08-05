using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Aliases
{
    public class AliasContainer : IEnumerable<AliasBase>
    {
        private readonly List<InnerAlias> _innerAliases = new List<InnerAlias>();
        private readonly List<RootAlias> _rootAliases = new List<RootAlias>();

        private IEnumerable<AliasBase> GetAliases => _rootAliases.Cast<AliasBase>().Concat(_innerAliases.Cast<AliasBase>());

        public IEnumerator<AliasBase> GetEnumerator() => GetAliases.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetAliases.GetEnumerator();

        public void AddRootAliases(params string[] values)
        {
            foreach (var value in values.Where(value => !_rootAliases.Any(y => y.Value.EqualsInvariant(value))))
            {
                _rootAliases.Add(new RootAlias(value));
            }
        }

        public void SetInnerAliase(string value)
        {
            _innerAliases.Clear();
            _innerAliases.Add(new InnerAlias(value));
        }
    }
}
