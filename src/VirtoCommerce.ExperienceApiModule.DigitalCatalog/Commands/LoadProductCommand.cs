using System;
using System.Collections.Generic;
using MediatR;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Requests
{
    public class LoadProductCommand : IRequest<LoadProductResponse>, IHasIncludeFields
    {
        public string[] Ids { get; set; }
        public IEnumerable<string> IncludeFields { get; set; } = Array.Empty<string>();
    }
}
