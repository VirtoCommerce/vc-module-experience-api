using System.Collections.Generic;
using System.Linq;
using GraphQL.Resolvers;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.XProfile.Extensions;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class ContactType : ExtendableGraphType<ContactAggregate>
    {
        private readonly IOrganizationAggregateRepository _organizationAggregateRepository;

        public ContactType(IOrganizationAggregateRepository organizationAggregateRepository, IDynamicPropertySearchService dynamicPropertySearchService)
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
            ExtendableField<ListGraphType<AddressType>>("addresses", resolve: context => context.Source.Contact.Addresses);
            ExtendableField<NonNullGraphType<ListGraphType<Core.Schemas.DynamicPropertyValueType>>>("dynamicProperties", resolve: context =>
                context.Source.Contact.LoadMemberDynamicPropertyValues(dynamicPropertySearchService));
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
