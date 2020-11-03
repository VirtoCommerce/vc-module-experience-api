using System.Collections.Generic;
using System.Linq;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class ContactType : ObjectGraphType<ContactAggregate>
    {
        private readonly IOrganizationAggregateRepository _organizationAggregateRepository;

        public ContactType(IOrganizationAggregateRepository organizationAggregateRepository)
        {
            _organizationAggregateRepository = organizationAggregateRepository;

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
            Field<ListGraphType<MemberAddressType>>("addresses", resolve: context => context.Source.Contact.Addresses);
            Field<ListGraphType<UserType>>("securityAccounts", resolve: context => context.Source.Contact.SecurityAccounts);
            //TODO: remove later
            Field<StringGraphType>("organizationId", resolve: context => context.Source.Contact.Organizations?.FirstOrDefault());
            Field("organizationsIds", x => x.Contact.Organizations);
            Field("phones", x => x.Contact.Phones);


            AddField(new FieldType
            {
                Name = "Organizations",
                Description = "All contact's organizations",
                Type = GraphTypeExtenstionHelper.GetActualType<ListGraphType<OrganizationType>>(),
                Resolver = new AsyncFieldResolver<ContactAggregate, IEnumerable<OrganizationAggregate>>(async context =>
                {
                    if (context.Source.Contact.Organizations.IsNullOrEmpty())
                    {
                        return default;
                    }
                    else
                    {
                        var idsToTake = context.Source.Contact.Organizations.ToArray();
                        return await _organizationAggregateRepository.GetOrganizationsByIdsAsync(idsToTake);
                    }
                })
            });
        }
    }
}
