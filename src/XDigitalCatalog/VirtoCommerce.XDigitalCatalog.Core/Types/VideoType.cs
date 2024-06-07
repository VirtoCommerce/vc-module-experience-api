using GraphQL.Types;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Core.Types
{
    public class VideoType : ObjectGraphType<Video>
    {
        public VideoType()
        {
            Field(d => d.Name, nullable: false).Description("Video name");
            Field(d => d.Description, nullable: false).Description("Video description");
            Field(d => d.UploadDate, nullable: true).Description("Video upload date");
            Field(d => d.ThumbnailUrl, nullable: false).Description("Video thumbnial URL");
            Field(d => d.ContentUrl, nullable: false).Description("Video URL");
            Field(d => d.EmbedUrl, nullable: true).Description("Embeded video URL");
            Field(d => d.Duration, nullable: true).Description("Video duration");
            Field<StringGraphType>("cultureName", description: "Culture name", resolve: context => context.Source.LanguageCode);
            Field(d => d.OwnerId, nullable: false).Description("ID of the object video is attached to");
            Field(d => d.OwnerType, nullable: false).Description("Type of the object video is attached to (Product, Category)");
            Field(d => d.SortOrder, nullable: false).Description("Sort order");
        }
    }
}
