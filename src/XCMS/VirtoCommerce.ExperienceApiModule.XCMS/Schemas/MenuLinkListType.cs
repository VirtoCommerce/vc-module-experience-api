using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XCMS.Schemas
{
    public class MenuLinkListType : ObjectGraphType<Menu>
    {
        public MenuLinkListType()
        {
            Field(x => x.Name, nullable: true).Description("Menu name");
            Field(x => x.OuterId, nullable: true).Description("Menu outer ID");
            Field<ListGraphType<MenuLinkType>>("items", resolve: context => context.Source.Items);
        }
    }
}
