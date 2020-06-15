﻿using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart.ValidationErrors
{
    public class PriceError : ValidationError
    {
        public PriceError(Money oldPrice, Money oldPriceWithTax, Money newPrice, Money newPriceWithTax)
        {
            OldPrice = oldPrice;
            OldPriceWithTax = oldPriceWithTax;
            NewPrice = newPrice;
            NewPriceWithTax = newPriceWithTax;
        }

        public Money OldPrice { get; private set; }
        public Money OldPriceWithTax { get; private set; }
        public Money NewPrice { get; private set; }
        public Money NewPriceWithTax { get; private set; }
    }
}
