using System;
using VirtoCommerce.XPurchase.Domain.Models;
using VirtoCommerce.XPurchase.Models.Common;
using VirtoCommerce.XPurchase.Models.OperationResults;

namespace VirtoCommerce.XPurchase.Domain.Builders
{
    public class CartContextBuilder : ICartContextBuilder
    {
        private readonly ShoppingCartContext _context;

        protected CartContextBuilder(ShoppingCartContext prefilledContext)
            => _context = prefilledContext;

        public static CartContextBuilder Initialize(ShoppingCartContext prefilledContext)
            => new CartContextBuilder(prefilledContext);

        /// <summary>
        /// Get <seealso cref="ShoppingCartContext"/> from <seealso cref="CartContextBuilder"/>.
        /// </summary>
        /// <returns></returns>
        public ShoppingCartContext GetContext()
            => _context.InitializationStatus == ShoppingCartContext.ContextState.NotInitialized
                ? WithDefaults().GetContext()
                : _context;

        public ICartContextBuilder WithDefaults()
        {
            try
            {
                var language = new Language(_context.CultureName);
                _context.SetLanguage(language);
            }
            catch (Exception)
            {
                _context.InitializationErrors.Add(new ErrorResult(
                    ErrorType.Warning,
                    $"Culture name \"{_context.CultureName}\" is incorrect!")
                );
            }

            try
            {
                var currency = new Currency(_context.Language, _context.CurrencyCode);
                _context.SetCurrency(currency);
            }
            catch (Exception)
            {
                _context.InitializationErrors.Add(new ErrorResult(
                    ErrorType.Warning,
                    $"Currency code \"{_context.CurrencyCode}\" is incorrect!")
                );
            }

            _context.InitializationStatus = ShoppingCartContext.ContextState.Initialized;
            return this;
        }
    }
}
