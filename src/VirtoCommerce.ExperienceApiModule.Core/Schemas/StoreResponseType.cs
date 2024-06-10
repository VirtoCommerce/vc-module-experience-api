using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Models;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class StoreResponseType : ExtendableGraphType<StoreResponse>
    {
        public StoreResponseType()
        {
            Field(x => x.StoreId, nullable: false).Description("Store ID");
            Field(x => x.StoreName, nullable: false).Description("Store name");
            Field(x => x.CatalogId, nullable: false).Description("Store catalog ID");
            Field(x => x.StoreUrl, nullable: true).Description("Store URL");

            Field<NonNullGraphType<LanguageType>>(nameof(StoreResponse.DefaultLanguage), "Language", resolve: context => context.Source.DefaultLanguage);
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<LanguageType>>>>(nameof(StoreResponse.AvailableLanguages), "Available languages", resolve: context => context.Source.AvailableLanguages);

            Field<NonNullGraphType<CurrencyType>>(nameof(StoreResponse.DefaultCurrency), "Currency", resolve: context => context.Source.DefaultCurrency);
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<CurrencyType>>>>(nameof(StoreResponse.AvailableCurrencies), "Available currencies", resolve: context => context.Source.AvailableCurrencies);

            Field<NonNullGraphType<StoreSettingsType>>(nameof(StoreResponse.Settings), "Store settings", resolve: context => context.Source.Settings);
            Field<NonNullGraphType<GraphQLSettingsType>>(nameof(StoreResponse.GraphQLSettings), "GraphQL settings", resolve: context => context.Source.GraphQLSettings);
        }
    }
}
