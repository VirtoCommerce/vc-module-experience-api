using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.Data.Index
{
    public class DocumentChangesProviderStub : IIndexDocumentChangesProvider
    {
        public Task<IList<IndexDocumentChange>> GetChangesAsync(DateTime? startDate, DateTime? endDate, long skip, long take)
        {
            return Task.FromResult<IList<IndexDocumentChange>>(Array.Empty<IndexDocumentChange>());
        }

        public Task<long> GetTotalChangesCountAsync(DateTime? startDate, DateTime? endDate)
        {
            return Task.FromResult(0L);
        }
    }
}
