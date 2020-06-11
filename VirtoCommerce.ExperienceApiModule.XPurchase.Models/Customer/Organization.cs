using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Customer
{
    public partial class Organization : Member
    {
        /// <summary>
        /// Organization contacts
        /// </summary>
        [JsonIgnore]
        [IgnoreDataMember]
        public IMutablePagedList<Contact> Contacts { get; set; }
    }
}
