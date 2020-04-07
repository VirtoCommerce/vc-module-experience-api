namespace PetsStoreClient
{
    /// <summary>
    /// Represents a pet in a pet store
    /// </summary>
    public class Pet
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public PetStatus Status { get; set; }
    }
}
