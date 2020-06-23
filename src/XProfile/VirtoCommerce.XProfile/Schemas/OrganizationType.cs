using GraphQL.Types;
using MediatR;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class OrganizationType : ObjectGraphType<Organization>
    {
        public OrganizationType(IMediator mediator)
        {
            Name = "Organization";
            Description = "Organization info";

            Field(x => x.Description, nullable: true).Description("Description");
            Field(x => x.BusinessCategory, nullable: true).Description("Business category");
            Field(x => x.OwnerId, nullable: true).Description("Owner id");
            Field(x => x.ParentId, nullable: true).Description("Parent id");
            Field(x => x.Name).Description("Name");
            Field(x => x.MemberType).Description("Member type");
            Field(x => x.OuterId, nullable: true).Description("Outer id");
            Field<ListGraphType<AddressType>>("addresses", resolve: context => context.Source.Addresses);
            Field<ListGraphType<StringGraphType>>("phones", resolve: context => context.Source.Phones);
            Field<ListGraphType<StringGraphType>>("emails", resolve: context => context.Source.Emails);
            Field<ListGraphType<StringGraphType>>("groups", resolve: context => context.Source.Groups);
            Field(x => x.SeoObjectType).Description("SEO object type");


            //DynamicProperties

            //    SeoInfos

        }
    }
}
