using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Models;

public class LocalizedSettingResponse
{
    public IList<KeyValue> Items { get; set; }
}
