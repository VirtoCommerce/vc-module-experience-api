using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class LanguageType : ObjectGraphType<Language>
    {
        public LanguageType()
        {
            Field(x => x.CultureName, nullable: true).Description("Culture name format (e.g. en-US)");
            Field(x => x.NativeName, nullable: true).Description("Native name");
            Field(x => x.ThreeLeterLanguageName, nullable: true).Description("ISO 639-2 three-letter code for the language.");
            Field(x => x.TwoLetterLanguageName, nullable: true).Description("ISO 639-1 two-letter code for the language.");
            Field(x => x.TwoLetterRegionName, nullable: true).Description("Two-letter code defined in ISO 3166 for the country/region.");
            Field(x => x.ThreeLetterRegionName, nullable: true).Description("Three-letter code defined in ISO 3166 for the country/region.");
        }
    }
}
