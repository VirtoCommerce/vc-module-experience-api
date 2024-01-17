using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class LanguageType : ObjectGraphType<Language>
    {
        public LanguageType()
        {
            Field(x => x.IsInvariant, nullable: false).Description("Is invariant");
            Field(x => x.CultureName, nullable: false).Description("Culture name format (e.g. en-US)");
            Field(x => x.NativeName, nullable: false).Description("Native name");
            Field("ThreeLetterLanguageName", x => x.ThreeLeterLanguageName, nullable: false).Description("ISO 639-2 three-letter code for the language.");
            Field(x => x.TwoLetterLanguageName, nullable: false).Description("ISO 639-1 two-letter code for the language.");
            Field(x => x.TwoLetterRegionName, nullable: false).Description("Two-letter code defined in ISO 3166 for the country/region.");
            Field(x => x.ThreeLetterRegionName, nullable: false).Description("Three-letter code defined in ISO 3166 for the country/region.");
        }
    }
}
