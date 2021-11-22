using GraphQL.Types;
using VirtoCommerce.ContentModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XCMS.Schemas
{
    public class MenuLinkType : ObjectGraphType<MenuLink>
    {
        public MenuLinkType()
        {
            Field(x => x.Title, nullable: true).Description("Menu item title");
            Field(x => x.Url, nullable: true).Description("Menu item url");
            Field(x => x.Priority, nullable: true).Description("Menu item priority");
            Field(x => x.AssociatedObjectId, nullable: true).Description("Menu item object ID");
            Field(x => x.AssociatedObjectName, nullable: true).Description("Menu item object name");
            Field(x => x.AssociatedObjectType, nullable: true).Description("Menu item type name");
            Field(x => x.OuterId, nullable: true).Description("Menu item outerID");
        }
    }
}
