using System.Collections.Generic;

namespace PetsStoreClient
{
    /// <summary>
    /// Represents a pets search query result 
    /// </summary>
    public class SearchPetsResult
    {
        public IEnumerable<Pet> Pets { get; set; }
        public int TotalCount { get; set; }
    }
}
