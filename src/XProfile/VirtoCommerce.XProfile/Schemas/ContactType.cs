using System.Linq;
using GraphQL.Resolvers;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class ContactType : ObjectGraphType<ContactAggregate>
    {
        private readonly IOrganizationAggregateRepository _organizationAggregateRepository;

        public ContactType(IOrganizationAggregateRepository organizationAggregateRepository)
        {

            //this.AuthorizeWith(CustomerModule.Core.ModuleConstants.Security.Permissions.Read);

            Field(x => x.Contact.FirstName);
            Field(x => x.Contact.LastName);
            Field<DateGraphType>("birthDate", resolve: context => context.Source.Contact.BirthDate);
            Field(x => x.Contact.FullName);
            Field(x => x.Contact.Id);
            Field(x => x.Contact.MemberType);
            Field(x => x.Contact.MiddleName, true);
            Field(x => x.Contact.Name, true);
            Field(x => x.Contact.OuterId, true);
            Field<ListGraphType<AddressTypePro>>("addresses", resolve: context => context.Source.Contact.Addresses);
            Field<ListGraphType<StringGraphType>>("organizations", resolve: context => context.Source.Contact.Organizations);

            AddField(new FieldType
            {
                Name = "organization",
                Description = "Organization",
                Type = GraphTypeExtenstionHelper.GetActualType<OrganizationType>(),
                Resolver = new AsyncFieldResolver<ContactAggregate, OrganizationAggregate>(async context =>
                {
                    //if (context.Source.Contact.Organizations.IsNullOrEmpty())
                    //{
                    //    return default;
                    //}
                    //else
                    //{
                        return await _organizationAggregateRepository.GetOrganizationByIdAsync(context.Source.Contact.Organizations.FirstOrDefault());
                    //}
                })
            });
            _organizationAggregateRepository = organizationAggregateRepository;
        }
    }
}
