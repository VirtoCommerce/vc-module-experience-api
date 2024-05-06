using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.XPurchase.Tests.Helpers.Stubs
{
    public class StubShippingMethod : ShippingMethod
    {
        public StubShippingMethod(string code) : base(code)
        {
        }

        public override IEnumerable<ShippingRate> CalculateRates(IEvaluationContext context) => throw new System.NotImplementedException();
    }
}
