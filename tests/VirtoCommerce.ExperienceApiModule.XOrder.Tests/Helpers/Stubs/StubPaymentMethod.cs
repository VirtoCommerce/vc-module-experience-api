using System.Collections.Specialized;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.PaymentModule.Model.Requests;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Tests.Helpers.Stubs
{
    public class StubPaymentMethod : PaymentMethod
    {
        public StubPaymentMethod(string code) : base(code)
        {
        }

        public override PaymentMethodType PaymentMethodType => throw new System.NotImplementedException();

        public override PaymentMethodGroupType PaymentMethodGroupType => throw new System.NotImplementedException();

        public override CapturePaymentRequestResult CaptureProcessPayment(CapturePaymentRequest context) => throw new System.NotImplementedException();

        public override PostProcessPaymentRequestResult PostProcessPayment(PostProcessPaymentRequest request) => throw new System.NotImplementedException();

        public override ProcessPaymentRequestResult ProcessPayment(ProcessPaymentRequest request) => throw new System.NotImplementedException();

        public override RefundPaymentRequestResult RefundProcessPayment(RefundPaymentRequest context) => throw new System.NotImplementedException();

        public override ValidatePostProcessRequestResult ValidatePostProcessRequest(NameValueCollection queryString) => throw new System.NotImplementedException();

        public override VoidPaymentRequestResult VoidProcessPayment(VoidPaymentRequest request) => throw new System.NotImplementedException();
    }
}
