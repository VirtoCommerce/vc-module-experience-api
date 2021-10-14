using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XPurchase
{
    public class AddressOptional
    {
        public Optional<string> Key { get; set; }
        public Optional<string> City { get; set; }
        public Optional<string> CountryCode { get; set; }
        public Optional<string> CountryName { get; set; }
        public Optional<string> Email { get; set; }
        public Optional<string> FirstName { get; set; }
        public Optional<string> LastName { get; set; }
        public Optional<string> MiddleName { get; set; }
        public Optional<string> Name { get; set; }
        public Optional<string> Line1 { get; set; }
        public Optional<string> Line2 { get; set; }
        public Optional<string> Organization { get; set; }
        public Optional<string> Phone { get; set; }
        public Optional<string> PostalCode { get; set; }
        public Optional<string> RegionId { get; set; }
        public Optional<string> RegionName { get; set; }
        public Optional<string> Zip { get; set; }
        public Optional<string> OuterId { get; set; }
        public Optional<int> AddressType { get; set; }
    }
}
