using System.Linq;
using GraphQL.Authorization;
using GraphQL.Resolvers;
using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Schema;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class ContactType : ObjectGraphType<Contact>
    {
        private readonly IMemberService _memberService;

        public ContactType(IMemberService memberService)
        {
            _memberService = memberService;

            //this.AuthorizeWith(CustomerModule.Core.ModuleConstants.Security.Permissions.Read);

            Field(x => x.FirstName);
            Field(x => x.LastName);
            Field<ListGraphType<AddressType>>("addresses", resolve: context => context.Source.Addresses);
            Field<ListGraphType<StringGraphType>>("organizations", resolve: context => context.Source.Organizations);


            var organizationField = new FieldType
            {
                Name = "organization",
                Description = "Organization",
                Type = GraphTypeExtenstionHelper.GetActualType<OrganizationType>(),
                Resolver = new AsyncFieldResolver<Contact, object>(async context => await _memberService.GetByIdAsync(context.Source.Organizations.FirstOrDefault(), null, typeof(Organization).Name))
            };
            AddField(organizationField);



        }
    }
}
