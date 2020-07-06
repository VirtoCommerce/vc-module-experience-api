using GraphQL.Authorization;
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
            //this.AuthorizeWith(CustomerModule.Core.ModuleConstants.Security.Permissions.Read);

            Field(x => x.Id).Description("Description");
            Field(x => x.Description, true).Description("Description");
            Field(x => x.BusinessCategory, true).Description("Business category");
            Field(x => x.OwnerId, true).Description("Owner id");
            Field(x => x.ParentId, true).Description("Parent id");
            Field(x => x.Name).Description("Name");
            Field(x => x.MemberType).Description("Member type");
            Field(x => x.OuterId, true).Description("Outer id");
            Field<NonNullGraphType<ListGraphType<AddressTypePro>>>("addresses", resolve: x => x.Source.Addresses);
            Field(x => x.Phones);
            Field(x => x.Emails);
            Field(x => x.Groups);
            Field(x => x.SeoObjectType).Description("SEO object type");

            // TODO:
            //DynamicProperties
            //    SeoInfos
        }
    }
}
