using System.Linq;
using GraphQL.Authorization;
using GraphQL.Resolvers;
using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Schema;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class ContactType : ObjectGraphType<ContactAggregate>
    {
        private readonly IMemberService _memberService;

        public ContactType(IMemberService memberService)
        {
            _memberService = memberService;

            //this.AuthorizeWith(CustomerModule.Core.ModuleConstants.Security.Permissions.Read);

            Field(x => x.Contact.FirstName);
            Field(x => x.Contact.LastName);
            Field<DateGraphType>("birthDate", resolve: context => context.Source.Contact.BirthDate);
            Field(x => x.Contact.FullName);
            Field(x => x.Contact.Id);
            Field(x => x.Contact.MiddleName, true);
            Field(x => x.Contact.Name);
            Field(x => x.Contact.OuterId, true);
            Field<ListGraphType<AddressTypePro>>("addresses", resolve: context => context.Source.Contact.Addresses);
            Field<ListGraphType<StringGraphType>>("organizations", resolve: context => context.Source.Contact.Organizations);


            var organizationField = new FieldType
            {
                Name = "organization",
                Description = "Organization",
                Type = GraphTypeExtenstionHelper.GetActualType<OrganizationType>(),
                Resolver = new AsyncFieldResolver<ContactAggregate, object>(async context => await _memberService.GetByIdAsync(context.Source.Contact.Organizations.FirstOrDefault(), null, typeof(Organization).Name))
            };
            AddField(organizationField);



        }
    }
}
