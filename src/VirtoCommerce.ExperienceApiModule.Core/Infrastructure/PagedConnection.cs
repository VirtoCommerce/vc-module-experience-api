using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Types.Relay.DataObjects;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
{
    public class PagedConnection<TNode> : Connection<TNode>
    {
        public PagedConnection(IEnumerable<TNode> superset, int skip, int take, int totalCount)
        {
            Edges = superset
                 .Select((x, index) =>
                     new Edge<TNode>()
                     {
                         Cursor = (skip + index).ToString(),
                         Node = x,
                     })
                 .ToList();

            PageInfo = new PageInfo()
            {
                HasNextPage = totalCount > (skip + take),
                HasPreviousPage = skip > 0,
                StartCursor = skip.ToString(),
                EndCursor = Math.Min(totalCount, (skip + take)).ToString()
            };

            TotalCount = totalCount;
        }
    }
}
