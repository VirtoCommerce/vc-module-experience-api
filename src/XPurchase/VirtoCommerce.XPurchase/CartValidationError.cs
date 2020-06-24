using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase.Domain.CartAggregate
{
    public class CartValidationError
    {
        protected  CartValidationError()
        {
        }
        public string ObjectType { get; set; }
        public string ObjectId { get; set; }
        public string ErrorCode { get; set; }
        public string Error { get; set; }

        public static CartValidationError FromEntity(IEntity entity, string error, string errorCode = null)
        {
            return new CartValidationError
            {
                ObjectType = entity.GetType().Name,
                ObjectId = entity.Id,
                Error = error,
                ErrorCode = errorCode
            };
        }
    }
}
