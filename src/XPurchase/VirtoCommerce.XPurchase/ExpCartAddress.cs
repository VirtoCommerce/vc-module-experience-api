using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Common;
using Address = VirtoCommerce.CartModule.Core.Model.Address;

namespace VirtoCommerce.XPurchase
{
    public sealed class ExpCartAddress
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
        public Optional<string> Description { get; set; }
        public Optional<int> AddressType { get; set; }

        public Address MapTo(Address address)
        {
            if (address == null)
            {
                address = AbstractTypeFactory<Address>.TryCreateInstance();
            }

            if (Key?.IsSpecified == true)
            {
                address.Key = Key.Value;
            }

            if (City?.IsSpecified == true)
            {
                address.City = City.Value;
            }

            if (CountryCode?.IsSpecified == true)
            {
                address.CountryCode = CountryCode.Value;
            }

            if (CountryName?.IsSpecified == true)
            {
                address.CountryName = CountryName.Value;
            }

            if (Email?.IsSpecified == true)
            {
                address.Email = Email.Value;
            }

            if (FirstName?.IsSpecified == true)
            {
                address.FirstName = FirstName.Value;
            }

            if (LastName?.IsSpecified == true)
            {
                address.LastName = LastName.Value;
            }

            if (Line1?.IsSpecified == true)
            {
                address.Line1 = Line1.Value;
            }

            if (Line2?.IsSpecified == true)
            {
                address.Line2 = Line2.Value;
            }

            if (MiddleName?.IsSpecified == true)
            {
                address.MiddleName = MiddleName.Value;
            }

            if (Name?.IsSpecified == true)
            {
                address.Name = Name.Value;
            }

            if (Organization?.IsSpecified == true)
            {
                address.Organization = Organization.Value;
            }

            if (Phone?.IsSpecified == true)
            {
                address.Phone = Phone.Value;
            }

            if (PostalCode?.IsSpecified == true)
            {
                address.PostalCode = PostalCode.Value;
            }

            if (RegionId?.IsSpecified == true)
            {
                address.RegionId = RegionId.Value;
            }

            if (RegionName?.IsSpecified == true)
            {
                address.RegionName = RegionName.Value;
            }

            if (Zip?.IsSpecified == true)
            {
                address.Zip = Zip.Value;
            }

            if (OuterId?.IsSpecified == true)
            {
                address.OuterId = OuterId.Value;
            }

            if (Description?.IsSpecified == true)
            {
                address.Description = Description.Value;
            }

            if (AddressType?.IsSpecified == true)
            {
                address.AddressType = (AddressType)AddressType.Value;
            }

            return address;
        }
    }
}
