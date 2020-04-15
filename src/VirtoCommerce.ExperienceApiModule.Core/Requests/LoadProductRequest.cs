using System;
using System.Collections.Generic;
using MediatR;

namespace VirtoCommerce.ExperienceApiModule.Core.Requests
{
    public class LoadProductRequest : IRequest<LoadProductResponse>, IHasIncludeFields
    {
        public string[] Ids { get; set; }
        public string ResponseGroup { get; set; }
        public IEnumerable<string> IncludeFields { get; set; } = Array.Empty<string>();
    }
}
