using GraphQL.Types;
using VirtoCommerce.PaymentModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderBankCardInfoType : ObjectGraphType<BankCardInfo>
    {
        public OrderBankCardInfoType()
        {
            Field(x => x.BankCardNumber);
            Field(x => x.BankCardType);
            Field(x => x.BankCardMonth);
            Field(x => x.BankCardYear);
            Field(x => x.BankCardCVV2);
            Field(x => x.CardholderName);
        }
    }
}
