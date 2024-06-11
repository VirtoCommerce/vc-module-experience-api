using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XPurchase.Core.Schemas;

namespace VirtoCommerce.XPurchase.Core.Models
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

        public CartValidationError(string type, string id, string error, string errorCode = null)
            : base(type, error)
        {
            ObjectType = type;
            ObjectId = id;
            ErrorMessage = error;
            ErrorCode = errorCode;
        }

        public string ObjectType { get; set; }
        public string ObjectId { get; set; }
        public List<ErrorParameter> ErrorParameters =>
            FormattedMessagePlaceholderValues
                ?.Select(kvp => new ErrorParameter { Key = kvp.Key, Value = kvp.Value.ToString() })
                .ToList();
    }
}
