namespace VirtoCommerce.XPurchase.Models.OperationResults
{
    public class ErrorResult : OperationResult
    {
        public ErrorType ErrorType { get; private set; }
        public string Message { get; set; }

        public ErrorResult(ErrorType errorType) : base(false)
        {
            ErrorType = errorType;
        }

        public ErrorResult(ErrorType errorType, string message) : base(false)
        {
            ErrorType = errorType;
            Message = message;
        }
    }
}
