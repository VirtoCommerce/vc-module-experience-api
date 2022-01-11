using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XPurchase.Schemas;

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
        public List<ErrorParameter> ErrorParameters =>
            FormattedMessagePlaceholderValues
                ?.Select(kvp => new ErrorParameter { Key = kvp.Key, Value = kvp.Value.ToString() })
                ?.ToList();
    }
}
