using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Requests
{
    public class CreateUserInvitationsCommand : IRequest<IdentityResult>
    {
        public string Message { get; set; }
        public IList<string> Roles { get; set; }
        public IList<string> Emails { get; set; }

        public string OrganizationId { get; set; }
        public object StoreId { get; internal set; }
        public object StoreEmail { get; internal set; }
        public object Language { get; internal set; }
    }
}
