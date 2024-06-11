using VirtoCommerce.PaymentModule.Core.Model;

namespace VirtoCommerce.XOrder.Core.Validators
{
    public static class OrderErrorDescriber
    {
        public static string InvalidStatus(PaymentStatus status, PaymentStatus[] availStatuses)
        {
            return $"Unable to process due to invalid payment status: {status}. Only {string.Join(',', availStatuses)} statuses are available for processing.";
        }

        public static string InvalidStatus(PaymentStatus status)
        {
            return $"Unable to process due to invalid payment status: {status}";
        }

        public static string OrderNotFound()
        {
            return $"Can't find customer order";
        }

        public static string PaymentNotFound()
        {
            return $"Can't find payment in order";
        }

        public static string PaymentMethodNotFound(string gatewayCode)
        {
            return $"Can't find payment method with code {gatewayCode ?? "undef"}";
        }

        public static string StoreNotFound()
        {
            return $"Can't find a store";
        }

        public static string PaymentMethodUnavailable(string code)
        {
            return $"The payment method code:{code} unavailable";
        }
    }
}
