using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using VirtoCommerce.ExperienceApiModule.XOrder.Validators;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.PaymentModule.Model.Requests;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Model;
using Xunit;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Tests
{
    public class ProcessPaymentRequestValidatorTests
    {
        [Theory]
        [ClassData(typeof(PaymentsTestData))]
        public void CanValidatePayment(ProcessPaymentRequest request, bool isRequestValid)
        {
            var validator = AbstractTypeFactory<ProcessPaymentRequestValidator>.TryCreateInstance();
            var validationResult = validator.Validate(request);

            Assert.Equal(isRequestValid, validationResult.IsValid);
        }

        private class PaymentsTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    new ProcessPaymentRequest
                    {
                        BankCardInfo = new BankCardInfo()
                        {
                            BankCardCVV2 = "111",
                            BankCardMonth = 11,
                            BankCardYear = 22,
                            BankCardNumber = "370000000000002",
                            BankCardType = "Visa",
                            CardholderName = "John Smith"
                        },
                        Order = new CustomerOrder(),
                        Payment = new PaymentIn
                        {
                            PaymentMethod = new TestPaymentMethod("test")
                        },
                        Store = new Store()
                    }, true
                };
                yield return new object[]
                {
                    new ProcessPaymentRequest
                    {
                        BankCardInfo = new BankCardInfo()
                        {
                            BankCardCVV2 = "111",
                            BankCardMonth = 11,
                            BankCardYear = 22,
                            BankCardNumber = "wrong_data",
                            BankCardType = "Visa",
                            CardholderName = "John Smith"
                        },
                        Order = new CustomerOrder(),
                        Payment = new PaymentIn
                        {
                            PaymentMethod = new TestPaymentMethod("test")
                        },
                        Store = new Store()
                    }, false
                };
                yield return new object[]
                {
                    new ProcessPaymentRequest
                    {
                        BankCardInfo = new BankCardInfo()
                        {
                            BankCardCVV2 = "111",
                            BankCardMonth = 11,
                            BankCardYear = 22,
                            BankCardNumber = "",
                            BankCardType = "Visa",
                            CardholderName = "John Smith"
                        },
                        Order = new CustomerOrder(),
                        Payment = new PaymentIn
                        {
                            PaymentMethod = new TestPaymentMethod("test")
                        },
                        Store = new Store()
                    }, false
                };
                yield return new object[]
                {
                    new ProcessPaymentRequest
                    {
                        BankCardInfo = new BankCardInfo()
                        {
                            BankCardCVV2 = "",
                            BankCardMonth = 11,
                            BankCardYear = 22,
                            BankCardNumber = "370000000000002",
                            BankCardType = "Visa",
                            CardholderName = "John Smith"
                        },
                        Order = new CustomerOrder(),
                        Payment = new PaymentIn
                        {
                            PaymentMethod = new TestPaymentMethod("test")
                        },
                        Store = new Store()
                    }, false
                };
                yield return new object[]
                {
                    new ProcessPaymentRequest
                    {
                        BankCardInfo = new BankCardInfo()
                        {
                            BankCardCVV2 = "111",
                            BankCardMonth = 0,
                            BankCardYear = 22,
                            BankCardNumber = "370000000000002",
                            BankCardType = "Visa",
                            CardholderName = "John Smith"
                        },
                        Order = new CustomerOrder(),
                        Payment = new PaymentIn
                        {
                            PaymentMethod = new TestPaymentMethod("test")
                        },
                        Store = new Store()
                    }, false
                };
                yield return new object[]
                {
                    new ProcessPaymentRequest
                    {
                        BankCardInfo = new BankCardInfo()
                        {
                            BankCardCVV2 = "111",
                            BankCardMonth = 11,
                            BankCardYear = 0,
                            BankCardNumber = "370000000000002",
                            BankCardType = "Visa",
                            CardholderName = "John Smith"
                        },
                        Order = new CustomerOrder(),
                        Payment = new PaymentIn
                        {
                            PaymentMethod = new TestPaymentMethod("test")
                        },
                        Store = new Store()
                    }, false
                };
                yield return new object[]
                {
                    new ProcessPaymentRequest
                    {
                        BankCardInfo = new BankCardInfo()
                        {
                            BankCardCVV2 = "111",
                            BankCardMonth = 11,
                            BankCardYear = 22,
                            BankCardNumber = "370000000000002",
                            BankCardType = "",
                            CardholderName = "John Smith"
                        },
                        Order = new CustomerOrder(),
                        Payment = new PaymentIn
                        {
                            PaymentMethod = new TestPaymentMethod("test")
                        },
                        Store = new Store()
                    }, false
                };
                yield return new object[]
                {
                    new ProcessPaymentRequest
                    {
                        BankCardInfo = new BankCardInfo()
                        {
                            BankCardCVV2 = "111",
                            BankCardMonth = 11,
                            BankCardYear = 22,
                            BankCardNumber = "370000000000002",
                            BankCardType = "Visa",
                            CardholderName = ""
                        },
                        Order = new CustomerOrder(),
                        Payment = new PaymentIn
                        {
                            PaymentMethod = new TestPaymentMethod("test")
                        },
                        Store = new Store()
                    }, false
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class TestPaymentMethod : PaymentMethod
        {
            public TestPaymentMethod(string code) : base(code)
            {
            }

            public override ProcessPaymentRequestResult ProcessPayment(ProcessPaymentRequest request)
            {
                throw new NotImplementedException();
            }

            public override PostProcessPaymentRequestResult PostProcessPayment(PostProcessPaymentRequest request)
            {
                throw new NotImplementedException();
            }

            public override VoidPaymentRequestResult VoidProcessPayment(VoidPaymentRequest request)
            {
                throw new NotImplementedException();
            }

            public override CapturePaymentRequestResult CaptureProcessPayment(CapturePaymentRequest context)
            {
                throw new NotImplementedException();
            }

            public override RefundPaymentRequestResult RefundProcessPayment(RefundPaymentRequest context)
            {
                throw new NotImplementedException();
            }

            public override ValidatePostProcessRequestResult ValidatePostProcessRequest(NameValueCollection queryString)
            {
                throw new NotImplementedException();
            }

            public override PaymentMethodType PaymentMethodType { get; }
            public override PaymentMethodGroupType PaymentMethodGroupType { get; }
        }
    }
}
