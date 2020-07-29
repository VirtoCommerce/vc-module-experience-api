using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;
using MediatR;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.ExperienceApiModule.XProfile.Queries;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class OrganizationType : ObjectGraphType<OrganizationAggregate>
    {
        public OrganizationType(IMediator mediator)
        {
            Name = "Organization";
            Description = "Organization info";
            //this.AuthorizeWith(CustomerModule.Core.ModuleConstants.Security.Permissions.Read);

            Field(x => x.Organization.Id).Description("Description");
            Field(x => x.Organization.Description, true).Description("Description");
            Field(x => x.Organization.BusinessCategory, true).Description("Business category");
            Field(x => x.Organization.OwnerId, true).Description("Owner id");
            Field(x => x.Organization.ParentId, true).Description("Parent id");
            Field(x => x.Organization.Name, true).Description("Name");
            Field(x => x.Organization.MemberType).Description("Member type");
            Field(x => x.Organization.OuterId, true).Description("Outer id");
            Field<NonNullGraphType<ListGraphType<MemberAddressType>>>("addresses", resolve: x => x.Source.Organization.Addresses);
            Field(x => x.Organization.Phones, true);
            Field(x => x.Organization.Emails, true);
            Field(x => x.Organization.Groups, true);
            Field(x => x.Organization.SeoObjectType).Description("SEO object type");

            // TODO:
            //DynamicProperties
            //    SeoInfos

            var connectionBuilder = GraphTypeExtenstionHelper.CreateConnection<ContactType, OrganizationAggregate>()
               .Name("contacts")
               .Argument<StringGraphType>("searchPhrase", "Free text search")
               .Unidirectional()
               .PageSize(20);

            connectionBuilder.ResolveAsync(async context => await ResolveConnectionAsync(mediator, context));
            AddField(connectionBuilder.FieldType);
        }

        private async Task<object> ResolveConnectionAsync(IMediator mediator, IResolveConnectionContext<OrganizationAggregate> context)
        {
            var first = context.First;
            var skip = Convert.ToInt32(context.After ?? 0.ToString());

            var response = await mediator.Send(new SearchOrganizationMembersQuery
            {
                OrganizationId = context.Source.Organization.Id,
                Take = first ?? 20,
                Skip = skip,
                SearchPhrase = context.GetArgument<string>("searchPhrase")
            });

            var result = new Connection<ContactAggregate>()
            {
                Edges = response.Results.Select((x, index) =>
                        new Edge<ContactAggregate>
                        {
                            Cursor = (skip + index).ToString(),
                            Node = new ContactAggregate(x as Contact)
                        })
                    .ToList(),
                PageInfo = new PageInfo
                {
                    HasNextPage = response.TotalCount > (skip + first),
                    HasPreviousPage = skip > 0,
                    StartCursor = skip.ToString(),
                    EndCursor = Math.Min(response.TotalCount, (int)(skip + first)).ToString()
                },
                TotalCount = response.TotalCount,
            };

            return result;
        }
    }
}
