using GraphQL.Types;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class AssetType : ObjectGraphType<Asset>
    {
        public AssetType() // TODO: add descriptions
        {
            Name = "Asset";

            Field(x => x.Id).Description("The unique ID of the asset.");
            Field(x => x.Name, nullable: true).Description("The name of the asset.");
            Field(x => x.MimeType, nullable: true);
            Field(x => x.Size, nullable: true);
            Field(x => x.Url, nullable: true);
            Field(x => x.RelativeUrl, nullable: true);
            Field(x => x.TypeId, nullable: true);
        }
    }
}
