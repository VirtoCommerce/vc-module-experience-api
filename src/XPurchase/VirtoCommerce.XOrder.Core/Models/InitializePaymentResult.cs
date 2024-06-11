using System.Collections.Generic;

namespace VirtoCommerce.XOrder.Core.Models
{
    public class InitializePaymentResult : PaymentResult
    {
        public string StoreId { get; set; }

        public string PaymentId { get; set; }

        public string OrderId { get; set; }

        public string OrderNumber { get; set; }

        public string PaymentMethodCode { get; set; }

        public string PaymentActionType { get; set; }

        public string ActionRedirectUrl { get; set; }

        public string ActionHtmlForm { get; set; }

        public Dictionary<string, string> PublicParameters { get; set; } = new();
    }
}
