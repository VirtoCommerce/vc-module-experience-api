using System;

namespace VirtoCommerce.XPurchase.Models.Exceptions
{
    public class CartException : Exception
    {
        public string View { get; set; }

        public CartException(string message)
           : this(message, null)
        {
        }
        public CartException(string message, string view)
            : this(message, null, view)
        {
        }

        public CartException(string message, Exception innerException, string view)
            : base(message, innerException)
        {
            View = view;
        }
    }
}
