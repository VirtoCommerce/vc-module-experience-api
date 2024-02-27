using GraphQL.Types;
using Microsoft.AspNetCore.Identity;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas;

public class PasswordOptionsType : ObjectGraphType<PasswordOptions>
{
    public PasswordOptionsType()
    {
        Field(x => x.RequiredLength, nullable: false).Description("The minimum length a password must be.");
        Field(x => x.RequiredUniqueChars, nullable: false).Description("The minimum number of unique chars a password must comprised of.");
        Field(x => x.RequireNonAlphanumeric, nullable: false).Description("Require a non letter or digit character.");
        Field(x => x.RequireLowercase, nullable: false).Description("Require a lower case letter ('a' - 'z').");
        Field(x => x.RequireUppercase, nullable: false).Description("Require an upper case letter ('A' - 'Z').");
        Field(x => x.RequireDigit, nullable: false).Description("Require a digit ('0' - '9').");
    }
}
