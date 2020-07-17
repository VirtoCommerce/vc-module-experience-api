using GraphQL.Types;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class ImageType : ObjectGraphType<Image>
    {
        /// <summary>
        ///
        /// </summary>
        /// <example>
        /// "sortOrder":0,
        /// "binaryData":null,
        /// "relativeUrl":"catalog/LG55EG9600/1431446771000_1119832.jpg",
        /// "url":"http://localhost:10645/assets/catalog/LG55EG9600/1431446771000_1119832.jpg",
        /// "typeId":"Image",
        /// "group":"images",
        /// "name":"1431446771000_1119832.jpg",
        /// "outerId":null,
        /// "languageCode":null,
        /// "isInherited":false,
        /// "seoObjectType":"Image",
        /// "seoInfos":null,
        /// "id":"a40b05e231ba4be0893bd4bbcfb92376"
        /// </example>
        public ImageType()
        {
            Field<StringGraphType>("id", resolve: context => context.Source.Id);
            Field<StringGraphType>("name", resolve: context => context.Source.Name);
            Field<StringGraphType>("url", resolve: context => context.Source.Url);
            Field<StringGraphType>("relativeUrl", resolve: context => context.Source.RelativeUrl);
            Field<IntGraphType>("sortOrder", resolve: context => context.Source.SortOrder);
        }
    }
}
