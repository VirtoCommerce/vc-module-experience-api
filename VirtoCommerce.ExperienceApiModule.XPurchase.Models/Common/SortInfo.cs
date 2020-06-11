using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Enums;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Extensions;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common
{
    public sealed class SortInfo : ValueObject
    {
        public override string ToString()
        {
            return SortColumn + "-" + (SortDirection == SortDirection.Descending ? "desc" : "asc");
        }
        public static string ToString(IEnumerable<SortInfo> sortInfos)
        {
            if (!sortInfos.IsNullOrEmpty())
            {
                return string.Join(";", sortInfos);
            }
            return string.Empty;
        }
        public static IEnumerable<SortInfo> Parse(string sortExpr)
        {
            var retVal = new List<SortInfo>();
            if (String.IsNullOrEmpty(sortExpr))
                return retVal;

            var sortInfoStrings = sortExpr.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var sortInfoString in sortInfoStrings)
            {
                var parts = sortInfoString.Split(new[] { ':', '-' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Any())
                {
                    var sortInfo = new SortInfo
                    {
                        SortColumn = parts[0].Trim(),
                        SortDirection = SortDirection.Ascending
                    };
                    if (parts.Count() > 1)
                    {
                        sortInfo.SortDirection = parts[1].Trim().StartsWith("desc", StringComparison.InvariantCultureIgnoreCase) ? SortDirection.Descending : SortDirection.Ascending;
                    }
                    retVal.Add(sortInfo);
                }
            }
            return retVal;
        }

        public string SortColumn { get; set; }

        public SortDirection SortDirection { get; set; }

    }
}
