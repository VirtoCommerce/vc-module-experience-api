using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace VirtoCommerce.ExperienceApiModule.XDigitalCatalog.Index
{
    public static class IndexFieldsMapper
    {
        public class RegexpNameMapper
        {
            protected Regex MatchPattern { get; set; }
            protected string IndexFieldPattern { get; set; }
            public string[] AdditionalFields { get; set; }

            public RegexpNameMapper(Regex matchPattern, string indexFieldPattern, string[] additionalFields = null)
            {
                MatchPattern = matchPattern;
                IndexFieldPattern = indexFieldPattern;
                AdditionalFields = additionalFields;
            }

            public virtual bool CanMap(string includeField)
            {
                return MatchPattern.Match(includeField).Success;
            }

            public virtual string Map(string includeField)
            {
                return MatchPattern.Replace(includeField, IndexFieldPattern);
            }
        }

        public static IList<RegexpNameMapper> Mappers => new List<RegexpNameMapper>()
        {
            new RegexpNameMapper(new Regex(@".*", RegexOptions.Compiled | RegexOptions.IgnoreCase),"$0", new [] { "__object.id" }),
            new RegexpNameMapper(new Regex(@"(items.)?price[s]?.(?<part>[^\.]+).*$", RegexOptions.Compiled | RegexOptions.IgnoreCase),"__prices.$2", new [] { "__prices.currency" }),
            new RegexpNameMapper(new Regex(@"^items.variations", RegexOptions.Compiled | RegexOptions.IgnoreCase),"__variations"),
            new RegexpNameMapper(new Regex(@"^items", RegexOptions.Compiled | RegexOptions.IgnoreCase),"__object"),
            new RegexpNameMapper(new Regex(@"^(?!__)", RegexOptions.Compiled | RegexOptions.IgnoreCase),"__object."),

         
            new RegexpNameMapper(new Regex(@"properties.value$", RegexOptions.Compiled | RegexOptions.IgnoreCase),"properties.values"),
            new RegexpNameMapper(new Regex(@"imgSrc", RegexOptions.Compiled | RegexOptions.IgnoreCase),"images"),

            new RegexpNameMapper(new Regex(@"__object.availabilityData.isActive", RegexOptions.Compiled | RegexOptions.IgnoreCase),"__object.isActive"),
            new RegexpNameMapper(new Regex(@"__object.availabilityData.isBuyable", RegexOptions.Compiled | RegexOptions.IgnoreCase),"__object.isBuyable"),
            new RegexpNameMapper(new Regex(@"__object.availabilityData.trackInventory", RegexOptions.Compiled | RegexOptions.IgnoreCase),"__object.trackInventory"),

            new RegexpNameMapper(new Regex(@"__object.parent.*", RegexOptions.Compiled | RegexOptions.IgnoreCase),"__object.parentId"),
            new RegexpNameMapper(new Regex(@"__object.hasParent.*", RegexOptions.Compiled | RegexOptions.IgnoreCase),"__object.parentId"),
            new RegexpNameMapper(new Regex(@"__object.parent.*", RegexOptions.Compiled | RegexOptions.IgnoreCase),"__object.parentId"),

            new RegexpNameMapper(new Regex(@"__object.category.*", RegexOptions.Compiled | RegexOptions.IgnoreCase),"__object.categoryId"),
            new RegexpNameMapper(new Regex(@"__object.descriptions", RegexOptions.Compiled | RegexOptions.IgnoreCase),"__object.reviews"),
            new RegexpNameMapper(new Regex(@"__object.slug", RegexOptions.Compiled | RegexOptions.IgnoreCase),"__object.seoinfos.semanticUrl"),
            new RegexpNameMapper(new Regex(@"__object.metaDescription", RegexOptions.Compiled | RegexOptions.IgnoreCase),"__object.seoinfos.metaDescription"),
            new RegexpNameMapper(new Regex(@"__object.metaKeywords", RegexOptions.Compiled | RegexOptions.IgnoreCase),"__object.seoinfos.metaKeywords"),
            new RegexpNameMapper(new Regex(@"__object.metaTitle", RegexOptions.Compiled | RegexOptions.IgnoreCase),"__object.seoinfos.metaTitle"),
        };

        public static IEnumerable<string> MapToIndexIncludes(IEnumerable<string> includeFields)
        {
            var result = new List<string>();
            foreach (var includeField in includeFields)
            {
                var indexField = includeField;
                foreach (var mapper in Mappers)
                {
                    if (mapper.CanMap(indexField))
                    {
                        indexField = mapper.Map(indexField);
                        if(mapper.AdditionalFields != null)
                        {
                            result.AddRange(mapper.AdditionalFields);
                        }
                    }
                }
                result.Add(indexField);
            }
            return result.Distinct();
        }
    }
    
}
