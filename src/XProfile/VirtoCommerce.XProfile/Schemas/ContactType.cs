using System;
using System.Linq;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Model.Search;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XProfile.Commands;
using VirtoCommerce.ExperienceApiModule.XProfile.Extensions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class ContactType : ExtendableGraphType<ContactAggregate>
    {
        [Obsolete("Remove after 1.38.0 version")]
        private readonly IOrganizationAggregateRepository _organizationAggregateRepository;

        public ContactType(
            IOrganizationAggregateRepository organizationAggregateRepository,
            IDynamicPropertyResolverService dynamicPropertyResolverService,
            IMediator mediator,
            IMemberAggregateFactory memberAggregateFactory)
        {
            _organizationAggregateRepository = organizationAggregateRepository;

            Field(x => x.Contact.FirstName);
            Field(x => x.Contact.LastName);
            Field<DateGraphType>("birthDate",
                "Contact birth date",
                resolve: context => context.Source.Contact.BirthDate.HasValue ? context.Source.Contact.BirthDate.Value.Date : (DateTime?)null);
            Field(x => x.Contact.FullName);
            Field(x => x.Contact.Id);
            Field(x => x.Contact.MemberType);
            Field(x => x.Contact.MiddleName, true);
            Field(x => x.Contact.Name, true);
            Field(x => x.Contact.OuterId, true);
            Field(x => x.Contact.Status, true).Description("Contact status");
            Field<ListGraphType<StringGraphType>>("emails", resolve: x => x.Source.Contact.Emails, description: "List of contact emails");

            ExtendableField<NonNullGraphType<ListGraphType<DynamicPropertyValueType>>>(
                "dynamicProperties",
                "Contact dynamic property values",
                QueryArgumentPresets.GetArgumentForDynamicProperties(),
                context => dynamicPropertyResolverService.LoadDynamicPropertyValues(context.Source.Contact, context.GetArgumentOrValue<string>("cultureName")));
            Field<ListGraphType<UserType>>("securityAccounts",
                "Security accounts",
                resolve: context => context.Source.Contact.SecurityAccounts);
            Field<StringGraphType>("organizationId",
                "Organization ID",
                resolve: context => context.Source.Contact.Organizations?.FirstOrDefault());
            Field("organizationsIds", x => x.Contact.Organizations);
            Field("phones", x => x.Contact.Phones);

            var organizationsConnectionBuilder = GraphTypeExtenstionHelper.CreateConnection<OrganizationType, ContactAggregate>()
                .Name("organizations")
                .Argument<StringGraphType>("searchPhrase", "Free text search")
                .Argument<StringGraphType>("sort", "Sort expression")
                .PageSize(20);

            organizationsConnectionBuilder.ResolveAsync(async context =>
            {
                var response = AbstractTypeFactory<MemberSearchResult>.TryCreateInstance();
                var query = context.GetSearchMembersQuery<SearchOrganizationsQuery>();

                // If user have no organizations, member search service would return all organizations
                // it means we don't need the search request when user's organization list is empty
                if (!context.Source.Contact.Organizations.IsNullOrEmpty())
                {
                    query.DeepSearch = true;
                    query.ObjectIds = context.Source.Contact.Organizations;
                    response = await mediator.Send(query);
                }

                return new PagedConnection<OrganizationAggregate>(response.Results.Select(x => memberAggregateFactory.Create<OrganizationAggregate>(x)), query.Skip, query.Take, response.TotalCount);
            });
            AddField(organizationsConnectionBuilder.FieldType);

            var addressesConnectionBuilder = GraphTypeExtenstionHelper.CreateConnection<MemberAddressType, ContactAggregate>()
                .Name("addresses")
                .Argument<StringGraphType>("sort", "Sort expression")
                .PageSize(20);

            addressesConnectionBuilder.Resolve(ResolveAddressesConnection);
            AddField(addressesConnectionBuilder.FieldType);
        }

        private static object ResolveAddressesConnection(IResolveConnectionContext<ContactAggregate> context)
        {
            var take = context.First ?? 20;
            var skip = Convert.ToInt32(context.After ?? 0.ToString());
            var sort = context.GetArgument<string>("sort");
            var addresses = context.Source.Contact.Addresses.AsEnumerable();

            if (!string.IsNullOrEmpty(sort))
            {
                var sortInfos = SortInfo.Parse(sort);
                addresses = addresses
                    .AsQueryable()
                    .OrderBySortInfos(sortInfos);
            }

            return new PagedConnection<Address>(addresses.Skip(skip).Take(take), skip, take, addresses.Count());
        }
    }
}
