using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XProfile.Queries;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class ContactType : ExtendableGraphType<ContactAggregate>
    {
        private readonly IOrganizationAggregateRepository _organizationAggregateRepository;

        public ContactType(
            IOrganizationAggregateRepository organizationAggregateRepository,
            IDynamicPropertyResolverService dynamicPropertyResolverService,
            IMediator mediator,
            IMemberAggregateFactory memberAggregateFactory)
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

            ExtendableField<NonNullGraphType<ListGraphType<DynamicPropertyValueType>>>(
                "dynamicProperties",
                "Contact's dynamic property values",
                QueryArgumentPresets.GetArgumentForDynamicProperties(),
                context => dynamicPropertyResolverService.LoadDynamicPropertyValues(context.Source.Contact, context.GetArgumentOrValue<string>("cultureName")));
            Field<ListGraphType<UserType>>("securityAccounts", resolve: context => context.Source.Contact.SecurityAccounts);
            //TODO: remove later
            Field<StringGraphType>("organizationId", resolve: context => context.Source.Contact.Organizations?.FirstOrDefault());
            Field("organizationsIds", x => x.Contact.Organizations);
            Field("phones", x => x.Contact.Phones);

            var organizationsConnectionBuilder = GraphTypeExtenstionHelper.CreateConnection<OrganizationType, ContactAggregate>()
                .Name("organizations")
                .Argument<StringGraphType>("searchPhrase", "Free text search")
                .Argument<StringGraphType>("sort", "Sort expression")
                .Unidirectional()
                .PageSize(20);

            organizationsConnectionBuilder.ResolveAsync(async context => await ResolveOrganizationsConnectionAsync(mediator, memberAggregateFactory, context));
            AddField(organizationsConnectionBuilder.FieldType);

            var addressesConnectionBuilder = GraphTypeExtenstionHelper.CreateConnection<AddressType, ContactAggregate>()
                .Name("addresses")
                .Argument<StringGraphType>("sort", "Sort expression")
                .Unidirectional()
                .PageSize(20);

            addressesConnectionBuilder.Resolve(ResolveAddressesConnection);
            AddField(addressesConnectionBuilder.FieldType);
        }


        private static async Task<object> ResolveOrganizationsConnectionAsync(IMediator mediator, IMemberAggregateFactory factory, IResolveConnectionContext<ContactAggregate> context)
        {
            var first = context.First;
            var skip = Convert.ToInt32(context.After ?? 0.ToString());

            var query = new SearchMembersQuery
            {
                ObjectIds = context.Source.Contact.Organizations,
                Take = first ?? 20,
                Skip = skip,
                SearchPhrase = context.GetArgument<string>("searchPhrase"),
                Sort = context.GetArgument<string>("sort"),
                MemberType = nameof(Organization),
            };

            var response = await mediator.Send(query);

            return new PagedConnection<OrganizationAggregate>(response.Results.Select(x => factory.Create<OrganizationAggregate>(x)), query.Skip, query.Take, response.TotalCount);
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
