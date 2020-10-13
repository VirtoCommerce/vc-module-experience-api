using VirtoCommerce.XPurchase.Schemas;

namespace VirtoCommerce.XPurchase.Queries
{
    public class SearchCartQuery : CartQueryBase<SearchCartResponse>
    {
        public string Filter { get; set; }
        public string Sort { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }


    }
}
