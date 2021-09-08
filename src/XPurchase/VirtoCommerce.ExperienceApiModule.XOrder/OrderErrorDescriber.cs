using VirtoCommerce.PaymentModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder
{
    public static class OrderErrorDescriber
    {
        public static string InvalidStatus(PaymentStatus status, PaymentStatus[] availStatuses)
        {
            return  $"Unable to process due to invalid payment status: {status}. Only {string.Join(',', availStatuses)} statuses are available for processing.";
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

    }
}
