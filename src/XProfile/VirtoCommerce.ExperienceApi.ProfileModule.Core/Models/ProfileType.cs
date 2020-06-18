using System.Linq;
using GraphQL.Types;
using MediatR;

namespace VirtoCommerce.ExperienceApi.ProfileModule.Core.Models
{
    public class ProfileType : ObjectGraphType<Profile>
    {
        public ProfileType(IMediator mediator)
        {
            Name = "Profile";
            Description = "Currently logged-in customer information";

            Field(x => x.User.Id).Description("User ID");
            Field(x => x.User.StoreId, nullable: true).Description("User store ID (if any)");
            Field(x => x.User.UserName).Description("User login name");
            Field(x => x.User.PhoneNumber, nullable: true).Description("Phone Number");
            Field(x => x.User.PhoneNumberConfirmed).Description("Is Phone Number confirmed");
            Field(x => x.User.Email, nullable: true).Description("User email");
            Field("IsRegisteredUser", x => true).Description("Is User Registered");
            Field(x => x.User.IsAdministrator).Description("Is User Administrator");
            Field(d => d.User.UserType).Description("UserType");
            Field("Permissions", x => x.User.Roles.SelectMany(x => x.Permissions).Select(x => x.Name).Distinct().ToArray()).Description("All User's Permissions");

            //Field<ListGraphType<RoleType>>("roles", resolve: context => context.Source.User.Roles);
            Field<ContactType>("contact", "Customer object", resolve: context => context.Source.Contact);
        }
    }
}
