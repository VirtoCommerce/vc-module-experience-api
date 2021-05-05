using System.Linq;
using GraphQL.Resolvers;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class UserType : ObjectGraphType<ApplicationUser>
    {
        public UserType(IContactAggregateRepository contactAggregateRepository)
        {
            Field(x => x.AccessFailedCount);
            Field(x => x.CreatedBy, true);
            Field(x => x.CreatedDate, true);
            Field(x => x.Email, true);
            Field(x => x.EmailConfirmed);
            Field(x => x.Id);
            Field(x => x.IsAdministrator);
            Field(x => x.LockoutEnabled);
            Field<DateTimeGraphType>("lockoutEnd", resolve: x => x.Source.LockoutEnd);
            Field(x => x.MemberId, true);
            Field(x => x.ModifiedBy, true);
            Field(x => x.ModifiedDate, true);
            Field(x => x.NormalizedEmail, true);
            Field(x => x.NormalizedUserName, true);
            Field(x => x.PasswordExpired);
            Field(x => x.PhoneNumber, true);
            Field(x => x.PhoneNumberConfirmed);
            Field(x => x.PhotoUrl, true);
            Field<ListGraphType<RoleType>>("roles", resolve: x => x.Source.Roles);
            Field<ListGraphType<StringGraphType>>("permissions", resolve: x => x.Source.Roles?.SelectMany(r => r.Permissions?.Select(p=> p.Name)));
            Field(x => x.SecurityStamp);
            Field(x => x.StoreId, true);
            Field(x => x.TwoFactorEnabled);
            Field(x => x.UserName);
            Field(x => x.UserType, true);

            AddField(new FieldType
            {
                Name = "Contact",
                Description = "The associated contact info",
                Type = GraphTypeExtenstionHelper.GetActualType<ContactType>(),
                Resolver = new AsyncFieldResolver<ApplicationUser, ContactAggregate>(async context =>
                    (ContactAggregate) await contactAggregateRepository.GetMemberAggregateRootByIdAsync(context.Source.MemberId))
            });
        }
    }
}
