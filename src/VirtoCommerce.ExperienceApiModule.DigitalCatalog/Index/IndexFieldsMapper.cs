using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace VirtoCommerce.ExperienceApiModule.XDigitalCatalog.Index
{
    public static class IndexFieldsMapper
    {
        public class RegexpNameMapper
        {
            protected Regex _regex { get; set; }

            protected string Replacement { get; set; }
            public string[] AdditionalFields { get; set; }

            public RegexpNameMapper(string pattern, string replacement, string[] additionalFields = null)
            {
                _regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                Replacement = replacement;
                AdditionalFields = additionalFields;
            }

            public virtual bool CanMap(string input)
            {
                return _regex.IsMatch(input);
            }

            public virtual string Map(string input)
            {
                return _regex.Replace(input, Replacement);
            }
        }

        private static IList<RegexpNameMapper> _mappers;

        public static IList<RegexpNameMapper> Mappers
        {
            get
            {
                if (_mappers == null)
                {
                    _mappers = new List<RegexpNameMapper>()
                    {
                        new RegexpNameMapper(@".*", "$0", new [] { "__object.id", "__object.categoryId", "__object.catalogId" }),
                        new RegexpNameMapper(@"(items.)?price[s]?.(?<part>[^\.]+).*$","__prices.$2", new [] { "__prices.currency" }),
                        new RegexpNameMapper(@"^variations", "__variations", new [] { "__variations" }),
                        new RegexpNameMapper(@"^hasVariations", "__variations", new [] { "__variations" }),
                        new RegexpNameMapper(@"^items.variations", "__variations", new [] { "__variations" }),
                        new RegexpNameMapper(@"^items.hasVariations", "__variations", new [] { "__variations" }),
                        new RegexpNameMapper(@"^items", "__object"),
                        new RegexpNameMapper(@"^(?!__)", "__object."),

                        new RegexpNameMapper(@"properties.value$", "properties.values"),
                        new RegexpNameMapper(@"imgSrc", "images"),

                        new RegexpNameMapper(@"__object.availabilityData.isActive", "__object.isActive"),
                        new RegexpNameMapper(@"__object.availabilityData.isBuyable", "__object.isBuyable"),
                        new RegexpNameMapper(@"__object.availabilityData.trackInventory", "__object.trackInventory"),

                        new RegexpNameMapper(@"__object.parent.*",  "__object.parentId"),
                        new RegexpNameMapper(@"__object.hasParent.*", "__object.parentId"),
                        new RegexpNameMapper(@"__object.parent.*", "__object.parentId"),

                        new RegexpNameMapper(@"__object.category.*", "__object.categoryId"),
                        new RegexpNameMapper(@"__object.seoInfo.*", "__object.seoInfos"),

                        new RegexpNameMapper(@"__object.breadcrumbs.*", "__object.outlines"),

                        #region Category
		
                        new RegexpNameMapper(@"__object.slug$", "__object.outlines", new [] { "__object.seoInfos" }),
                        new RegexpNameMapper(@"__object.outline$", "__object.outlines"),
                        new RegexpNameMapper(@"__object.level$", "__object.outlines"),
                        new RegexpNameMapper(@"__object.descriptions", "__object.reviews", new [] { "__object.descriptions" }),
                        new RegexpNameMapper(@"__object.description.*", "__object.reviews", new [] { "__object.descriptions" })

	                    #endregion
                    };
                }
                return _mappers;
            }
        }

        public static IEnumerable<string> MapToIndexIncludes(IEnumerable<string> includeFields)
        {
            IEnumerable<string> result = new string[] { };
            foreach (var includeField in includeFields)
            {
                var indexField = includeField;
                foreach (var mapper in Mappers)
                {
                    if (mapper.CanMap(indexField))
                    {
                        indexField = mapper.Map(indexField);
                        if (mapper.AdditionalFields != null)
                        {
                            result = result.Union(mapper.AdditionalFields);
                        }
                    }
                }
                result = result.Union(new string[] { indexField });
            }
            return result;
        }
    }
}
