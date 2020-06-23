using System.Threading.Tasks;
using GraphQL;
using GraphQL.Authorization;
using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class AddressMutation : ObjectGraphType
    {
        public AddressMutation(IMemberService memberService, IAuthorizationEvaluator authorizationEvaluator)
        {
            Name = "Mutation";
            FieldAsync<AddressType>(
              "addAddress",
              arguments: new QueryArguments(
                  new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "customerId" },
                  new QueryArgument<NonNullGraphType<AddressInputType>> { Name = "human" }
              ),
              resolve: async context =>
              {
                  var memberId = context.GetArgument<string>("customerId");
                  var item = context.GetArgument<Address>("address");

                  var member = await memberService.GetByIdAsync(memberId);
                  if (member != null)
                  {
                      // TODO: check auth with IAuthorizationEvaluator. Pseudo-code: 
                      //if (!(await AuthorizeAsync(member, ModuleConstants.Security.Permissions.Update)).Succeeded)
                      //{
                      //    return Unauthorized();
                      //}
                      member.Addresses.Add(item);
                      await memberService.SaveChangesAsync(new[] { member });
                  }

                  return Task.CompletedTask;
              });
        }
    }
}
