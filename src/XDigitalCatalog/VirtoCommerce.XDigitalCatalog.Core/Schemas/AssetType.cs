using GraphQL.Types;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Core.Schemas
{
    public class AssetType : ObjectGraphType<Asset>
    {
        public AssetType()
        {
            Name = "Asset";

            Field(x => x.Id, nullable: false).Description("The unique ID of the asset.");
            Field(x => x.Name, nullable: true).Description("The name of the asset.");
            Field(x => x.MimeType, nullable: true).Description("MimeType of the asset.");
            Field(x => x.Size, nullable: false).Description("Size of the asset.");
            Field(x => x.Url, nullable: false).Description("Url of the asset.");
            Field(x => x.RelativeUrl, nullable: true).Description("RelativeUrl of the asset.");
            Field(x => x.TypeId, nullable: false).Description("Type id of the asset.");
            Field(x => x.Group, nullable: true).Description("Group of the asset.");
            Field<StringGraphType>("cultureName",
                "Culture name",
                resolve: context => context.Source.LanguageCode);
        }
    }
}
