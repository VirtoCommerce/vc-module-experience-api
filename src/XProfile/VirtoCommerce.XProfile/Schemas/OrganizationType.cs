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
    public class OrganizationType : ExtendableGraphType<OrganizationAggregate>
    {
        public OrganizationType(IMediator mediator, IDynamicPropertyResolverService dynamicPropertyResolverService, IMemberAggregateFactory factory)
        {
            Name = "Organization";
            Description = "Organization info";
            //this.AuthorizeWith(CustomerModule.Core.ModuleConstants.Security.Permissions.Read);

            Field(x => x.Organization.Id);
            Field(x => x.Organization.Description, true).Description("Description");
            Field(x => x.Organization.BusinessCategory, true).Description("Business category");
            Field(x => x.Organization.OwnerId, true).Description("Owner id");
            Field(x => x.Organization.ParentId, true).Description("Parent id");
            Field(x => x.Organization.Name, true).Description("Name");
            Field(x => x.Organization.MemberType).Description("Member type");
            Field(x => x.Organization.OuterId, true).Description("Outer id");
            Field(x => x.Organization.Phones, true);
            Field(x => x.Organization.Emails, true);
            Field(x => x.Organization.Groups, true);
            Field(x => x.Organization.SeoObjectType).Description("SEO object type");
            ExtendableField<NonNullGraphType<ListGraphType<DynamicPropertyValueType>>>(
               "dynamicProperties",
               "Organization's dynamic property values",
                QueryArgumentPresets.GetArgumentForDynamicProperties(),
                context => dynamicPropertyResolverService.LoadDynamicPropertyValues(context.Source.Organization, context.GetArgumentOrValue<string>("cultureName")));

            // TODO:
            //    SeoInfos

            var connectionBuilder = GraphTypeExtenstionHelper.CreateConnection<ContactType, OrganizationAggregate>()
               .Name("contacts")
               .Argument<StringGraphType>("searchPhrase", "Free text search")
               .Argument<StringGraphType>("sort", "Sort expression")
               .Unidirectional()
               .PageSize(20);

            connectionBuilder.ResolveAsync(async context => await ResolveConnectionAsync(mediator, factory, context));
            AddField(connectionBuilder.FieldType);

            var addressesConnectionBuilder = GraphTypeExtenstionHelper.CreateConnection<AddressType, OrganizationAggregate>()
                .Name("addresses")
                .Argument<StringGraphType>("sort", "Sort expression")
                .Unidirectional()
                .PageSize(20);
            addressesConnectionBuilder.Resolve(ResolveAddressesConnection);
            AddField(addressesConnectionBuilder.FieldType);
        }


        private async Task<object> ResolveConnectionAsync(IMediator mediator, IMemberAggregateFactory factory, IResolveConnectionContext<OrganizationAggregate> context)
        {
            var first = context.First;
            var skip = Convert.ToInt32(context.After ?? 0.ToString());

            var query = new SearchOrganizationMembersQuery
            {
                OrganizationId = context.Source.Organization.Id,
                Take = first ?? 20,
                Skip = skip,
                SearchPhrase = context.GetArgument<string>("searchPhrase"),
                Sort = context.GetArgument<string>("sort"),
            };

            var response = await mediator.Send(query);

            return new PagedConnection<ContactAggregate>(response.Results.Select(x => factory.Create<ContactAggregate>(x)), query.Skip, query.Take, response.TotalCount);
        }

        private static object ResolveAddressesConnection(IResolveConnectionContext<OrganizationAggregate> context)
        {
            var take = context.First ?? 20;
            var skip = Convert.ToInt32(context.After ?? 0.ToString());
            var sort = context.GetArgument<string>("sort");
            var addresses = context.Source.Organization.Addresses.AsEnumerable();

            if (!string.IsNullOrEmpty(sort))
            {
                var sortInfos = SortInfo.Parse(sort);
                addresses = addresses.AsQueryable()
                    .OrderBySortInfos(sortInfos);
            }

            return new PagedConnection<Address>(addresses.Skip(skip).Take(take), skip, take, addresses.Count());
        }
    }
}
