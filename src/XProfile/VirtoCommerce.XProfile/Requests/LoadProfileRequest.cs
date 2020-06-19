using System;
using System.Collections.Generic;
using MediatR;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Requests
{
    public class LoadProfileRequest : IRequest<LoadProfileResponse>, IHasIncludeFields
    {
        public string Id { get; set; }
        public IEnumerable<string> IncludeFields { get; set; } = Array.Empty<string>();
    }
}
