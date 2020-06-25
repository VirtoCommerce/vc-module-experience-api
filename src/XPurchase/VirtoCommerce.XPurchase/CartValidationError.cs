using FluentValidation.Results;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase
{
    public class CartValidationError : ValidationFailure
    {
        public CartValidationError(IEntity entity, string error, string errorCode = null)
            : base(entity.ToString(), error)
        {
            ObjectType = entity.GetType().Name;
            ObjectId = entity.Id;
            ErrorMessage = error;
            ErrorCode = errorCode;
        }
        public string ObjectType { get; set; }
        public string ObjectId { get; set; }
    }
}
