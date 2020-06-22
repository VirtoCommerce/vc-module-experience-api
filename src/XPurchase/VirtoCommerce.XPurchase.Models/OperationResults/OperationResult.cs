using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.XPurchase.Models.Extensions;

namespace VirtoCommerce.XPurchase.Models.OperationResults
{
    public abstract class OperationResult
    {
        public bool IsSuccess { get; }
        public Dictionary<string, object> OperationContext { get; private set; }

        protected OperationResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
            OperationContext = new Dictionary<string, object>();
        }

        public T GetOrCreateContextValue<T>(string key) where T : class, new()
            => OperationContext.TryGetValue(key, out var value) && value is T valueT ? valueT : null;

        public object GetOrCreateContextValue(string key)
            => OperationContext.TryGetValue(key, out var value) ? value : null;

        /// <summary>
        /// Summ operation result outcome
        /// </summary>
        /// <param name="firstResult">First operation result</param>
        /// <param name="secondResult">Second operation result</param>
        /// <returns>New <seealso cref="OperationResult"/> with concated user context and concated error message</returns>
        /// <example>
        /// var operationResult = await _independentHandler.HandleAsync(some params...);
        /// operationResult += await _independentHandler2.HandleAsync(some params...);
        /// if (operationResult.IsSuccess)
        /// {
        ///     operationResult.OperationContext.Add(someKey, someSuccessValue);
        ///     return operationResult; // Type: SuccessResult
        /// }
        /// </example>
        public static OperationResult operator +(OperationResult firstResult, OperationResult secondResult)
            => (firstResult, secondResult) switch
            {
                (SuccessResult firstSuccessResult, SuccessResult secondSuccessResult)
                    => new SuccessResult(),

                (ErrorResult firstErrorResult, ErrorResult secondErrorResult)
                    => new ErrorResult((ErrorType)Math.Max((int)firstErrorResult.ErrorType, (int)secondErrorResult.ErrorType))
                    {
                        OperationContext = firstErrorResult.OperationContext.AddRange(secondErrorResult.OperationContext).ToDictionary(x => x.Key, x => x.Value),
                        Message = $"{firstErrorResult.Message} {secondErrorResult.Message}"
                    },

                (SuccessResult firstSuccessResult, ErrorResult secondErrorResult)
                    => new ErrorResult(secondErrorResult.ErrorType)
                    {
                        OperationContext = firstSuccessResult.OperationContext.AddRange(secondErrorResult.OperationContext).ToDictionary(x => x.Key, x => x.Value),
                        Message = secondErrorResult.Message
                    },

                (ErrorResult firstErrorResult, SuccessResult secondSuccessResult)
                    => new ErrorResult(firstErrorResult.ErrorType)
                    {
                        OperationContext = firstErrorResult.OperationContext.AddRange(secondSuccessResult.OperationContext).ToDictionary(x => x.Key, x => x.Value),
                        Message = firstErrorResult.Message
                    },

                _ => throw new ArgumentException($"Summ not allowed for {firstResult.GetType().Name} type and {secondResult.GetType().Name} type!")
            };
    }
}
