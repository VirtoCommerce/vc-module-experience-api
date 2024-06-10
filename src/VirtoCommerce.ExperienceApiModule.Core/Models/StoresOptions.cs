using System;
using System.Collections.Generic;

namespace VirtoCommerce.ExperienceApiModule.Core.Models
{
    public class StoresOptions
    {
        public string DefaultStore { get; set; }

        /// <summary>
        /// The Represents mapping of domains (key) with storeId (value).
        /// </summary>
        public IDictionary<string, string> Domains { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}
