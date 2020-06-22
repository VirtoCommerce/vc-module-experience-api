using GraphQL.Types;
using VirtoCommerce.XPurchase.Models.Stores;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class StoreType : ObjectGraphType<Store>
    {
        public StoreType()
        {
            Field(x => x.Name, nullable: true).Description("Name");

            Field(x => x.Description, nullable: true).Description("Description");

            Field(x => x.Url, nullable: true).Description("Url of store storefront");

            Field(x => x.SecureUrl, nullable: true).Description("Secure url of store, must use https protocol, required");

            Field(x => x.Host, nullable: true).Description("Host");

            Field<StoreStatusEnum>("storeState", resolve: context => context.Source.StoreState);

            Field(x => x.TimeZone, nullable: true).Description("TimeZone");

            Field(x => x.Country, nullable: true).Description("Country");

            Field(x => x.Region, nullable: true).Description("Region");

            Field<LanguageType>("defaultLanguage", resolve: context => context.Source.DefaultLanguage);

            Field<ListGraphType<LanguageType>>("languages", resolve: context => context.Source.Languages);

            Field(x => x.DefaultCurrencyCode, nullable: true).Description("Default currency of store");

            Field<ListGraphType<StringGraphType>>("currenciesCodes", resolve: context => context.Source.CurrenciesCodes);

            Field(x => x.Catalog, nullable: true).Description("Product catalog id assigned to store");

            Field(x => x.Email, nullable: true).Description("Contact email of store");

            Field(x => x.AdminEmail, nullable: true).Description("Administrator contact email of store");

            Field(x => x.ThemeName, nullable: true).Description("Store theme name");
        }
    }
}
