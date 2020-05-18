namespace PetsStoreClient
{
    /// <summary>
    /// Represents a query to search pets in pest store with given parameters 
    /// </summary>
    public class SearchPetsQuery
    {
        public string Keyword { get; set; }
        /// <summary>
        /// Search pets by multiple statuses
        /// </summary>
        public PetStatus[] Statuses { get; set; } = new[] { PetStatus.Available, PetStatus.Pending, PetStatus.Sold };

        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 20;
    }
}
