using System.Collections.Generic;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Models
{
    public class InitializePaymentResult
    {
        public string StoreId { get; set; }

        public string PaymentId { get; set; }

        public string OrderId { get; set; }

        public string OrderNumber { get; set; }

        public string PaymentMethodCode { get; set; }

        public string PaymentActionType { get; set; }

        public string ActionRedirectUrl { get; set; }

        public string ActionHtmlForm { get; set; }

        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; }

        public Dictionary<string, string> PublicParameters { get; set; } = new();
    }
}
