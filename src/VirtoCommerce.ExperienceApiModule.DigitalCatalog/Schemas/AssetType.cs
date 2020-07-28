using GraphQL.Types;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class AssetType : ObjectGraphType<Asset>
    {
        public AssetType()
        {
            Name = "Asset";

            Field(x => x.Id).Description("The unique ID of the asset.");
            Field(x => x.Name, nullable: true).Description("The name of the asset.");
            Field(x => x.MimeType, nullable: true).Description("MimeType of the asset.");
            Field(x => x.Size, nullable: true).Description("Size of the asset.");
            Field(x => x.Url, nullable: true).Description("Url of the asset.");
            Field(x => x.RelativeUrl, nullable: true).Description("RelativeUrl of the asset.");
            Field(x => x.TypeId, nullable: true).Description("Type id of the asset.");
            Field(x => x.Group, nullable: true).Description("Group of the asset.");
        }
    }
}
