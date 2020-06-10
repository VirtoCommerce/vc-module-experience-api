using System.Collections.Generic;
using Newtonsoft.Json;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common
{
    public partial class Address : CloneableValueObject
    {
        public AddressType Type { get; set; }

        public string Key { get; set; }

        public string Name { get; set; }

        public string Organization { get; set; }

        public string CountryCode { get; set; }

        public string CountryName { get; set; }

        public string City { get; set; }

        public string PostalCode { get; set; }

        public string Zip { get; set; }

        public string Line1 { get; set; }

        public string Line2 { get; set; }

        public string RegionId { get; set; }

        public string RegionName { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Id => Key;

        [JsonIgnore]
        public string Company => Organization;

        [JsonIgnore]
        public string Country => CountryName;

        [JsonIgnore]
        public string Address1 => Line1;

        [JsonIgnore]
        public string Address2 => Line2;

        [JsonIgnore]
        public string Street => string.Join(", ", Address1, Address2).Trim(',', ' ');

        [JsonIgnore]
        public string ProvinceCode => RegionId;

        [JsonIgnore]
        public string Province => RegionName;

        public override string ToString() => string.Join(" ", FirstName, LastName, Organization, Line1, City, RegionName, PostalCode, CountryName);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Key and Name properties don't participate in equality
            return new List<object>
            {
                Type,
                Organization,
                CountryCode,
                CountryName,
                PostalCode,
                Zip,
                Line1,
                Line2,
                RegionId,
                RegionName,
                FirstName,
                MiddleName,
                LastName,
                Phone,
                Email
            };
        }
    }
}
