
using System.Threading.Tasks;

namespace PetsStoreClient
{
    /// <summary>
    /// Represents the abstraction that is being used for search pets in store
    /// </summary>
    public interface IPetsSearchService
    {
        /// <summary>
        /// Search pets in store by given query
        /// </summary>
        /// <param name="query">search criteria</param>
        /// <returns></returns>
        Task<SearchPetsResult> SearchPetsAsync(SearchPetsQuery query);

        Task<Pet> LoadByIdAsync(long id);
    }
}
