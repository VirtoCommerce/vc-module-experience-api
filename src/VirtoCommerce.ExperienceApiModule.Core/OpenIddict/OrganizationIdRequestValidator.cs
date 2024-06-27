using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.Platform.Security.OpenIddict;

namespace VirtoCommerce.ExperienceApiModule.Core.OpenIddict;

public class OrganizationIdRequestValidator(IMemberService memberService) : ITokenRequestValidator
{
    public virtual int Priority { get; set; } = 50;

    private readonly IList<TokenResponse> _valid = [];

    public virtual async Task<IList<TokenResponse>> ValidateAsync(TokenRequestContext context)
    {
        var organizationId = GetOrganizationId(context);
        if (string.IsNullOrEmpty(organizationId))
        {
            return _valid;
        }

        var availableOrganizationIds = await GetAvailableOrganizationIds(context);
        if (availableOrganizationIds.Contains(organizationId))
        {
            return _valid;
        }

        return [ContactSecurityErrorDescriber.InvalidOrganizationId(organizationId)];
    }

    private static string GetOrganizationId(TokenRequestContext context)
    {
        return context.Request.GetParameter(Parameters.OrganizationId)?.Value?.ToString();
    }

    private async Task<IList<string>> GetAvailableOrganizationIds(TokenRequestContext context)
    {
        var memberId = context.User?.MemberId;

        if (string.IsNullOrEmpty(memberId))
        {
            return [];
        }

        var member = await memberService.GetByIdAsync(memberId);

        return member switch
        {
            Contact contact => contact.Organizations ?? [],
            Employee employee => employee.Organizations ?? [],
            _ => [],
        };
    }
}
