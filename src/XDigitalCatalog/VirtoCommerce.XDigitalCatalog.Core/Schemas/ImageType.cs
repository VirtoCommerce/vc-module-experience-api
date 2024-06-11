using GraphQL.Types;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Core.Schemas
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
            Field<NonNullGraphType<StringGraphType>>("id",
                "Image ID",
                resolve: context => context.Source.Id);
            Field<StringGraphType>("name",
                "Image name",
                resolve: context => context.Source.Name);
            Field<StringGraphType>("group",
                "Image group",
                resolve: context => context.Source.Group);
            Field<NonNullGraphType<StringGraphType>>("url",
                "Image URL",
                resolve: context => context.Source.Url);
            Field<StringGraphType>("relativeUrl",
                "Image relative URL",
                resolve: context => context.Source.RelativeUrl);
            Field<NonNullGraphType<IntGraphType>>("sortOrder",
                "Sort order",
                resolve: context => context.Source.SortOrder);
            Field<StringGraphType>("cultureName",
                "Culture name",
                resolve: context => context.Source.LanguageCode);
        }
    }
}
