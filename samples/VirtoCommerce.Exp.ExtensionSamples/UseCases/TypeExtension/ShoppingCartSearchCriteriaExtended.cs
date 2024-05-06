using VirtoCommerce.CartModule.Core.Model.Search;

namespace VirtoCommerce.Exp.ExtensionSamples.UseCases.TypeExtension
{
    public class ShoppingCartSearchCriteriaExtended : ShoppingCartSearchCriteria
    {
        public string ContractId { get; set; }
    }
}
