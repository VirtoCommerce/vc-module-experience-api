using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XCMS.Schemas
{
    public class MenuLinkType : ObjectGraphType<MenuItem>
    {
        public MenuLinkType()
        {
            Field(x => x.Link.Title, nullable: true).Description("Menu item title");
            Field(x => x.Link.Url, nullable: true).Description("Menu item url");
            Field(x => x.Link.Priority, nullable: true).Description("Menu item priority");
            Field(x => x.Link.AssociatedObjectId, nullable: true).Description("Menu item object ID");
            Field(x => x.Link.AssociatedObjectName, nullable: true).Description("Menu item object name");
            Field(x => x.Link.AssociatedObjectType, nullable: true).Description("Menu item type name");
            Field(x => x.Link.OuterId, nullable: true).Description("Menu item outerID");
            Field<ListGraphType<MenuLinkType>>(nameof(MenuItem.ChildItems), resolve: context => context.Source.ChildItems);
        }
    }
}
