using GraphQL.Types;
using MediatR;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class OrganizationType : ObjectGraphType<OrganizationAggregate>
    {
        public OrganizationType()
        {
            Name = "Organization";
            Description = "Organization info";
            //this.AuthorizeWith(CustomerModule.Core.ModuleConstants.Security.Permissions.Read);

            Field(x => x.Organization.Id).Description("Description");
            Field(x => x.Organization.Description, true).Description("Description");
            Field(x => x.Organization.BusinessCategory, true).Description("Business category");
            Field(x => x.Organization.OwnerId, true).Description("Owner id");
            Field(x => x.Organization.ParentId, true).Description("Parent id");
            Field(x => x.Organization.Name).Description("Name");
            Field(x => x.Organization.MemberType).Description("Member type");
            Field(x => x.Organization.OuterId, true).Description("Outer id");
            Field<NonNullGraphType<ListGraphType<AddressTypePro>>>("addresses", resolve: x => x.Source.Organization.Addresses);
            Field(x => x.Organization.Phones);
            Field(x => x.Organization.Emails);
            Field(x => x.Organization.Groups);
            Field(x => x.Organization.SeoObjectType).Description("SEO object type");

            // TODO:
            //DynamicProperties
            //    SeoInfos
        }
    }
}
