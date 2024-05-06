using System;

namespace VirtoCommerce.ExperienceApiModule.Core.Models
{
    [Obsolete("Use VirtoCommerce.Platform.Core.Common.KeyValue", DiagnosticId = "VC0006", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
    public class KeyValuePair
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
