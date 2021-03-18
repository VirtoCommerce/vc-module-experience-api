using VirtoCommerce.XPurchase.Commands;

namespace VirtoCommerce.Exp.ExtensionSamples.Commands
{
    public class RemoveCartCommandExtended : RemoveCartCommand
    {
        public string Reason { get; set; }
    };
}
